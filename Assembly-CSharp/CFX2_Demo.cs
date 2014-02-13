using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class CFX2_Demo : MonoBehaviour
{
	public bool orderedSpawns = true;

	public float step = 1f;

	public float range = 5f;

	private float order = -5f;

	public Material groundMat;

	public Material waterMat;

	public GameObject[] ParticleExamples;

	private int exampleIndex;

	private string randomSpawnsDelay = "0.5";

	private bool randomSpawns;

	private bool slowMo;

	private void OnMouseDown()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit val = default;
		if (((Component)this).collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), ref val, 9999f))
		{
			GameObject val2 = spawnParticle();
			val2.transform.position = val.point + val2.transform.position;
		}
	}

	private GameObject spawnParticle()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected Obj, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = (GameObject)Object.Instantiate((Object)(object)ParticleExamples[exampleIndex]);
		val.transform.position = new Vector3(0f, val.transform.position.y, 0f);
		val.SetActiveRecursively(true);
		return val;
	}

	private void OnGUI()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginArea(new Rect(5f, 20f, (float)(Screen.width - 10), 60f));
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Effect", new GUILayoutOption[0]);
		if (GUILayout.Button("<", new GUILayoutOption[0]))
		{
			prevParticle();
		}
		GUILayout.Label(((Object)ParticleExamples[exampleIndex]).name, new GUILayoutOption[1] { GUILayout.Width(210f) });
		if (GUILayout.Button(">", new GUILayoutOption[0]))
		{
			nextParticle();
		}
		GUILayout.Label("Click on the ground to spawn selected particles", new GUILayoutOption[1] { GUILayout.Width(150f) });
		if (GUILayout.Button((!CFX_Demo_RotateCamera.rotating) ? "Rotate Camera" : "Pause Camera", new GUILayoutOption[0]))
		{
			CFX_Demo_RotateCamera.rotating = !CFX_Demo_RotateCamera.rotating;
		}
		if (GUILayout.Button((!randomSpawns) ? "Start Random Spawns" : "Stop Random Spawns", new GUILayoutOption[1] { GUILayout.Width(140f) }))
		{
			randomSpawns = !randomSpawns;
			if (randomSpawns)
			{
				((MonoBehaviour)this).StartCoroutine("RandomSpawnsCoroutine");
			}
			else
			{
				((MonoBehaviour)this).StopCoroutine("RandomSpawnsCoroutine");
			}
		}
		randomSpawnsDelay = GUILayout.TextField(randomSpawnsDelay, 10, new GUILayoutOption[1] { GUILayout.Width(42f) });
		randomSpawnsDelay = Regex.Replace(randomSpawnsDelay, "[^0-9.]", string.Empty);
		if (GUILayout.Button((!((Component)this).renderer.enabled) ? "Show Ground" : "Hide Ground", new GUILayoutOption[1] { GUILayout.Width(90f) }))
		{
			((Component)this).renderer.enabled = !((Component)this).renderer.enabled;
		}
		if (GUILayout.Button((!slowMo) ? "Slow Motion" : "Normal Speed", new GUILayoutOption[1] { GUILayout.Width(100f) }))
		{
			slowMo = !slowMo;
			if (slowMo)
			{
				Time.timeScale = 0.33f;
			}
			else
			{
				Time.timeScale = 1f;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private IEnumerator RandomSpawnsCoroutine()
	{
		while (true)
		{
			GameObject particles = spawnParticle();
			if (orderedSpawns)
			{
				particles.transform.position = ((Component)this).transform.position + new Vector3(order, particles.transform.position.y, 0f);
				order -= step;
				if (order < 0f - range)
				{
					order = range;
				}
			}
			else
			{
				particles.transform.position = ((Component)this).transform.position + new Vector3(Random.Range(0f - range, range), 0f, Random.Range(0f - range, range)) + new Vector3(0f, particles.transform.position.y, 0f);
			}
			yield return (object)new WaitForSeconds(float.Parse(randomSpawnsDelay));
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown((KeyCode)276))
		{
			prevParticle();
		}
		else if (Input.GetKeyDown((KeyCode)275))
		{
			nextParticle();
		}
	}

	private void prevParticle()
	{
		exampleIndex--;
		if (exampleIndex < 0)
		{
			exampleIndex = ParticleExamples.Length - 1;
		}
		if (((Object)ParticleExamples[exampleIndex]).name.Contains("Splash") || ((Object)ParticleExamples[exampleIndex]).name.Contains("Skim"))
		{
			((Component)this).renderer.material = waterMat;
		}
		else
		{
			((Component)this).renderer.material = groundMat;
		}
	}

	private void nextParticle()
	{
		exampleIndex++;
		if (exampleIndex >= ParticleExamples.Length)
		{
			exampleIndex = 0;
		}
		if (((Object)ParticleExamples[exampleIndex]).name.Contains("Splash") || ((Object)ParticleExamples[exampleIndex]).name.Contains("Skim"))
		{
			((Component)this).renderer.material = waterMat;
		}
		else
		{
			((Component)this).renderer.material = groundMat;
		}
	}
}
