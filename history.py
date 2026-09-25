import argparse
import datetime
import hashlib
import io
import json
import lzma
import os
import re
import shutil
import struct
import subprocess
import tarfile
import tempfile
import urllib.request
import zipfile

SOURCE = "https://cdn.openkogama.org/versions.json"
SKIP = re.compile(
    r"^(UnityEngine|UnityEditor|Unity\.|System|mscorlib|Mono\.|Boo\.|UnityScript|JsonFx|Photon|websocket-sharp"
    r"|Newtonsoft|Fyber|Ionic|log4net|DOTween|TextMeshPro|nunit|Microsoft\.|netstandard|ICSharpCode|Accessibility)",
    re.I,
)


def versions():
    req = urllib.request.Request(SOURCE, headers={"User-Agent": "mono-history"})
    with urllib.request.urlopen(req, timeout=120) as r:
        entries = [e for e in json.load(r)["versions"] if not e["il2cpp"]]
    return sorted(entries, key=lambda e: (e["timestamp"], e["version"], e["sha256"]))


def fetch(entry):
    err = None
    for url in entry["urls"]:
        for attempt in range(3):
            try:
                req = urllib.request.Request(url, headers={"User-Agent": "mono-history"})
                with urllib.request.urlopen(req, timeout=300) as r:
                    data = r.read()
                if hashlib.sha256(data).hexdigest() != entry["sha256"]:
                    raise RuntimeError("sha256 mismatch")
                return data
            except Exception as e:
                err = e
    raise RuntimeError(err)


def cstr(b, o):
    e = b.index(b"\0", o)
    return b[o:e].decode("utf-8", "replace"), e + 1


def unityweb(raw):
    sig, o = cstr(raw, 0)
    if sig != "UnityWeb":
        return {}
    o += 4
    _, o = cstr(raw, o)
    _, o = cstr(raw, o)
    o += 4
    header = struct.unpack_from(">i", raw, o)[0]
    data = lzma.LZMADecompressor(format=lzma.FORMAT_ALONE).decompress(raw[header:])
    o = 4
    out = {}
    for _ in range(struct.unpack_from(">i", data, 0)[0]):
        name, o = cstr(data, o)
        start, size = struct.unpack_from(">ii", data, o)
        o += 8
        out[name] = data[start : start + size]
    return out


def assemblies(data):
    z = zipfile.ZipFile(io.BytesIO(data))
    dlls = {os.path.basename(n): z.read(n) for n in z.namelist() if n.endswith(".dll") and re.search(r"(^|/)Managed/", n)}
    if not dlls:
        for n in z.namelist():
            if not n.endswith("/"):
                dlls.update({k: v for k, v in unityweb(z.read(n)).items() if k.endswith(".dll")})
    return dlls


def decompile(entry, out):
    work = tempfile.mkdtemp()
    try:
        dlls = assemblies(fetch(entry))
        for name, blob in dlls.items():
            with open(os.path.join(work, name), "wb") as f:
                f.write(blob)
        own = sorted(n for n in dlls if not SKIP.match(n))
        tree = os.path.join(work, "src")
        for name in own:
            dest = os.path.join(tree, name[:-4])
            subprocess.run(
                ["ilspycmd", "-p", "-o", dest, os.path.join(work, name)],
                check=True,
                stdout=subprocess.DEVNULL,
                stderr=subprocess.DEVNULL,
            )
            for p in os.listdir(dest):
                if p.endswith(".csproj"):
                    os.remove(os.path.join(dest, p))
        with open(os.path.join(tree, "build.json"), "w", encoding="utf-8") as f:
            info = {k: entry[k] for k in ("id", "version", "unityVersion", "timestamp", "sha256")}
            info["assemblies"] = sorted(dlls)
            json.dump(info, f, indent=4)
            f.write("\n")
        with tarfile.open(os.path.join(out, entry["sha256"] + ".tar.gz"), "w:gz") as t:
            t.add(tree, arcname=".")
        return own
    finally:
        shutil.rmtree(work, ignore_errors=True)


def run_decompile(args):
    everything = versions()
    mine = [e for i, e in enumerate(everything) if i % args.shards == args.shard]
    if args.limit:
        mine = mine[: args.limit]
    os.makedirs(args.out, exist_ok=True)
    print(f"shard {args.shard}/{args.shards}: {len(mine)} of {len(everything)}", flush=True)
    failed = []
    for i, e in enumerate(mine, 1):
        if os.path.exists(os.path.join(args.out, e["sha256"] + ".tar.gz")):
            continue
        try:
            own = decompile(e, args.out)
            print(f"[{i}/{len(mine)}] {e['version'] or e['id']} {' '.join(own)}", flush=True)
        except Exception as ex:
            failed.append({"id": e["id"], "version": e["version"], "error": str(ex)})
            print(f"[{i}/{len(mine)}] {e['version'] or e['id']} failed: {ex}", flush=True)
    with open(os.path.join(args.out, f"failed-{args.shard}.json"), "w", encoding="utf-8") as f:
        json.dump(failed, f, indent=1)


def git(repo, *cmd, env=None):
    return subprocess.run(["git", "-C", repo, *cmd], check=True, capture_output=True, text=True, env=env).stdout


def run_assemble(args):
    everything = versions()
    if args.limit:
        everything = everything[: args.limit]
    if not os.path.exists(os.path.join(args.repo, ".git")):
        os.makedirs(args.repo, exist_ok=True)
        git(args.repo, "init", "-q", "-b", "main")
    done = set(git(args.repo, "log", "--format=%(trailers:key=Build-Sha256,valueonly)", "--all").split()) if git(args.repo, "rev-list", "--all", "--max-count=1").strip() else set()
    commits = missing = 0
    for e in everything:
        if e["sha256"] in done:
            continue
        archive = os.path.join(args.src, e["sha256"] + ".tar.gz")
        if not os.path.exists(archive):
            missing += 1
            print(f"missing {e['version'] or e['id']}", flush=True)
            continue
        for p in os.listdir(args.repo):
            if p != ".git":
                shutil.rmtree(os.path.join(args.repo, p)) if os.path.isdir(os.path.join(args.repo, p)) else os.remove(os.path.join(args.repo, p))
        with tarfile.open(archive) as t:
            t.extractall(args.repo, filter="data")
        when = datetime.datetime.fromtimestamp(e["timestamp"], datetime.UTC).strftime("%Y-%m-%dT%H:%M:%SZ")
        env = {**os.environ, "GIT_AUTHOR_DATE": when, "GIT_COMMITTER_DATE": when}
        title = f"{e['version'] or 'unversioned'} ({e['unityVersion']})"
        body = f"Build-Id: {e['id']}\nBuild-Sha256: {e['sha256']}"
        git(args.repo, "add", "-A")
        git(args.repo, "commit", "-q", "--allow-empty", "-m", title, "-m", body, env=env)
        tag = f"v{e['version']}-{e['sha256'][:8]}" if e["version"] else f"unversioned-{e['sha256'][:8]}"
        git(args.repo, "tag", "-f", tag)
        commits += 1
    print(f"{commits} commits, {missing} missing")


def main():
    p = argparse.ArgumentParser()
    sub = p.add_subparsers(dest="cmd", required=True)
    d = sub.add_parser("decompile")
    d.add_argument("--shard", type=int, default=0)
    d.add_argument("--shards", type=int, default=1)
    d.add_argument("--limit", type=int, default=0)
    d.add_argument("--out", default="out")
    a = sub.add_parser("assemble")
    a.add_argument("--src", default="out")
    a.add_argument("--repo", default="history")
    a.add_argument("--limit", type=int, default=0)
    args = p.parse_args()
    run_decompile(args) if args.cmd == "decompile" else run_assemble(args)


if __name__ == "__main__":
    main()
