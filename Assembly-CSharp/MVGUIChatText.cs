using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIChatText : MonoBehaviour
{
	public delegate void OnDisappearDelegate(MVGUIChatText guiChatText);

	private bool isInitialized;

	private UXText text;

	private Queue<Vector3> positions = new Queue<Vector3>();

	private float speed = 4f;

	public OnDisappearDelegate OnDisappear;

	private float fadeTime = 1f;

	private float stayTime = 6f;

	public string Text
	{
		get
		{
			return text.Text;
		}
		set
		{
			text.Text = value;
		}
	}

	private void Awake()
	{
		Initialize();
	}

	private void Start()
	{
		((MonoBehaviour)this).StartCoroutine(AppearDissapear());
	}

	private void Initialize()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (!isInitialized)
		{
			Object val = Object.Instantiate(Resources.Load("Prefabs/UX/Text"));
			GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
			text = val2.GetComponent<UXText>();
			text.horizontalAlign = UXHorizontal.Left;
			((Component)text).gameObject.transform.parent = ((Component)this).transform;
			((Component)text).gameObject.transform.localScale = Vector3.one;
			((Component)text).gameObject.transform.localPosition = Vector3.zero;
		}
	}

	private IEnumerator AppearDissapear()
	{
		text.Alpha = 0f;
		yield return ((MonoBehaviour)this).StartCoroutine(pTween.To(fadeTime, (float t) =>
		{
			text.Alpha = t;
		}));
		yield return (object)new WaitForSeconds(stayTime);
		yield return ((MonoBehaviour)this).StartCoroutine(pTween.To(fadeTime, 1f, 0f, (float t) =>
		{
			text.Alpha = t;
		}));
		((MonoBehaviour)this).StopAllCoroutines();
		if (OnDisappear != null)
		{
			OnDisappear(this);
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public void MoveToPosition(Vector3 pos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		positions.Enqueue(pos);
		if (positions.Count == 1)
		{
			((MonoBehaviour)this).StartCoroutine(Move());
		}
	}

	private IEnumerator Move()
	{
		while (positions.Count > 0)
		{
			Vector3 to = positions.Peek();
			Vector3 val = to - ((Component)this).transform.localPosition;
			float distance = val.magnitude;
			float duration = distance / speed;
			Vector3 from = ((Component)this).transform.localPosition;
			yield return ((MonoBehaviour)this).StartCoroutine(pTween.To(duration, (float t) =>
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				((Component)this).transform.localPosition = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
			}));
			positions.Dequeue();
		}
	}
}
