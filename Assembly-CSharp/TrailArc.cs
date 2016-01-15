using System;
using UnityEngine;

public class TrailArc : MonoBehaviour
{
	private int savedIndex;

	private int pointIndex;

	public Material material;

	private bool Emit = true;

	private bool emittingDone;

	public int maxPointsDrawn;

	public int pointsStored = 60;

	public float minVel = 10f;

	public bool faceCamera = true;

	public bool twist = true;

	private float time;

	public float lifetime = 1f;

	private float lifeTimeRatio = 1f;

	private float fadeOutRatio;

	public Color[] colors;

	public float[] widths;

	public float pointDistance = 0.5f;

	private float pointSqrDistance;

	public int segmentsPerPoint = 4;

	private float tRatio;

	public bool printResults;

	public bool printSavedPoints;

	public bool printSegmentPoints;

	private GameObject trail;

	private Renderer mRenderer;

	private Mesh mesh;

	private Material trailMaterial;

	private Vector3[] saved;

	private Vector3[] savedUp;

	private int savedCnt;

	private Vector3[] points;

	private Vector3[] pointsUp;

	private int pointCnt;

	private int displayCnt;

	private float lastPointCreationTime;

	private float averageCreationTime;

	private float averageInsertionTime;

	private float elapsedInsertionTime;

	private bool initialized;

	public bool emit
	{
		get
		{
			return Emit;
		}
		set
		{
			Emit = value;
		}
	}

	private void Start()
	{
		saved = new Vector3[pointsStored];
		savedUp = new Vector3[saved.Length];
		points = new Vector3[saved.Length * segmentsPerPoint];
		pointsUp = new Vector3[points.Length];
		tRatio = 1f / (float)segmentsPerPoint;
		pointSqrDistance = pointDistance * pointDistance;
		trail = new GameObject("Trail");
		trail.transform.position = Vector3.zero;
		trail.transform.rotation = Quaternion.identity;
		trail.transform.localScale = Vector3.one;
		MeshFilter meshFilter = trail.AddComponent<MeshFilter>();
		mRenderer = trail.AddComponent<MeshRenderer>();
		mesh = meshFilter.mesh;
		trailMaterial = new Material(material);
		fadeOutRatio = trailMaterial.GetColor("_TintColor").a;
		mRenderer.material = trailMaterial;
	}

	private void printPoints()
	{
		if (savedCnt != 0)
		{
			string text = "Saved Points at time " + Time.time + ":\n";
			for (int i = 0; i < savedCnt; i++)
			{
				string text2 = text;
				text = string.Concat(text2, "Index: ", i, "\tPos: ", saved[i], "\n");
			}
			MonoBehaviour.print(text);
		}
	}

	private void printAllPoints()
	{
		if (pointCnt != 0)
		{
			string text = "Points at time " + Time.time + ":\n";
			for (int i = 0; i < pointCnt; i++)
			{
				string text2 = text;
				text = string.Concat(text2, "Index: ", i, "\tPos: ", points[i], "\n");
			}
			MonoBehaviour.print(text);
		}
	}

	private void findCoordinates(int index)
	{
		if (index != 0 && index < savedCnt - 2)
		{
			Vector3 vector = saved[index - 1];
			Vector3 vector2 = saved[index];
			Vector3 vector3 = saved[index + 1];
			Vector3 vector4 = saved[index + 2];
			Vector3 vector5 = 0.5f * (vector3 - vector);
			Vector3 vector6 = 0.5f * (vector4 - vector2);
			int num = index * segmentsPerPoint;
			for (int i = num; i < num + segmentsPerPoint; i++)
			{
				float num2 = (float)(i - num) * tRatio;
				float num3 = num2 * num2;
				float num4 = num3 * num2;
				float num5 = 2f * num4 - 3f * num3 + 1f;
				float num6 = 3f * num3 - 2f * num4;
				float num7 = num4 - 2f * num3 + num2;
				float num8 = num4 - num3;
				int num9 = i - segmentsPerPoint;
				ref Vector3 reference = ref points[num9];
				reference = num5 * vector2 + num6 * vector3 + num7 * vector5 + num8 * vector6;
				ref Vector3 reference2 = ref pointsUp[num9];
				reference2 = Vector3.Lerp(savedUp[index], savedUp[index + 1], num2);
			}
			pointCnt = num;
		}
	}

	private void Update()
	{
		try
		{
			Vector3 position = transform.position;
			if (!initialized && Emit)
			{
				ref Vector3 reference = ref saved[savedCnt];
				reference = transform.TransformPoint(0f, 0f, 0f - pointDistance);
				ref Vector3 reference2 = ref savedUp[savedCnt];
				reference2 = transform.up;
				savedCnt++;
				saved[savedCnt] = position;
				ref Vector3 reference3 = ref savedUp[savedCnt];
				reference3 = transform.up;
				savedCnt++;
				lastPointCreationTime = Time.time;
				initialized = true;
			}
			if (printSavedPoints)
			{
				printPoints();
			}
			if (printSegmentPoints)
			{
				printAllPoints();
			}
			if (!Emit)
			{
				if (!emittingDone && pointCnt > 0)
				{
					ref Vector3 reference4 = ref saved[savedCnt];
					reference4 = transform.TransformPoint(0f, 0f, pointDistance);
					ref Vector3 reference5 = ref savedUp[savedCnt];
					reference5 = transform.up;
					savedCnt++;
					findCoordinates(savedCnt - 3);
					ref Vector3 reference6 = ref saved[savedCnt];
					reference6 = transform.TransformPoint(0f, 0f, pointDistance * 2f);
					ref Vector3 reference7 = ref savedUp[savedCnt];
					reference7 = transform.up;
					savedCnt++;
					findCoordinates(savedCnt - 3);
				}
				emittingDone = true;
			}
			if (emittingDone)
			{
				Emit = false;
			}
			if (Emit && (saved[savedCnt - 1] - position).sqrMagnitude > pointSqrDistance)
			{
				if (savedCnt > saved.Length - 1)
				{
					saved = new Vector3[pointsStored];
					savedUp = new Vector3[saved.Length];
					points = new Vector3[saved.Length * segmentsPerPoint];
					pointsUp = new Vector3[points.Length];
					savedCnt = 0;
					displayCnt = 0;
				}
				saved[savedCnt] = position;
				ref Vector3 reference8 = ref savedUp[savedCnt];
				reference8 = transform.up;
				savedCnt++;
				if (averageCreationTime == 0f)
				{
					averageCreationTime = Time.time - lastPointCreationTime;
				}
				else
				{
					float num = Time.time - lastPointCreationTime;
					averageCreationTime = (averageCreationTime + num) * 0.5f;
				}
				averageInsertionTime = averageCreationTime * tRatio;
				lastPointCreationTime = Time.time;
				if (savedCnt > 3)
				{
					findCoordinates(savedCnt - 3);
				}
			}
			if (!Emit && displayCnt == pointCnt)
			{
				Color color = trailMaterial.GetColor("_TintColor");
				color.a -= fadeOutRatio * lifeTimeRatio * Time.deltaTime;
				if (color.a > 0f)
				{
					trailMaterial.SetColor("_TintColor", color);
					return;
				}
				if (printResults)
				{
					MonoBehaviour.print("Trail effect ending with a segment count of: " + pointCnt);
				}
				UnityEngine.Object.Destroy(trail);
				UnityEngine.Object.Destroy(gameObject);
				return;
			}
			if (displayCnt < pointCnt)
			{
				elapsedInsertionTime += Time.deltaTime;
				while (elapsedInsertionTime > averageInsertionTime)
				{
					if (displayCnt < pointCnt)
					{
						displayCnt++;
					}
					elapsedInsertionTime -= averageInsertionTime;
				}
			}
			if (displayCnt < 2 || maxPointsDrawn == 1)
			{
				mRenderer.enabled = false;
				return;
			}
			mRenderer.enabled = true;
			lifeTimeRatio = 1f / lifetime;
			int num2 = displayCnt;
			if (num2 > maxPointsDrawn && maxPointsDrawn > 0)
			{
				num2 = maxPointsDrawn;
			}
			Vector3[] array = new Vector3[num2 * 2];
			Vector2[] array2 = new Vector2[num2 * 2];
			int[] array3 = new int[(num2 - 1) * 6];
			Color[] array4 = new Color[num2 * 2];
			float num3 = 1f / (float)(num2 - 1);
			Vector3 position2 = Camera.main.transform.position;
			for (int i = 0; i < num2; i++)
			{
				Vector3 vector = points[i + displayCnt - num2];
				float num4 = (float)i * num3;
				Color color2;
				if (colors.Length == 0)
				{
					color2 = Color.Lerp(Color.clear, Color.white, num4);
				}
				else if (colors.Length == 1)
				{
					color2 = Color.Lerp(Color.clear, colors[0], num4);
				}
				else if (colors.Length == 2)
				{
					color2 = Color.Lerp(colors[1], colors[0], num4);
				}
				else
				{
					float num5 = (float)(colors.Length - 1) - num4 * (float)(colors.Length - 1);
					if (num5 == (float)(colors.Length - 1))
					{
						color2 = colors[colors.Length - 1];
					}
					else
					{
						int num6 = (int)Mathf.Floor(num5);
						float t = num5 - (float)num6;
						color2 = Color.Lerp(colors[num6], colors[num6 + 1], t);
					}
				}
				array4[i * 2] = color2;
				array4[i * 2 + 1] = color2;
				float num7;
				if (widths.Length == 0)
				{
					num7 = 1f;
				}
				else if (widths.Length == 1)
				{
					num7 = widths[0];
				}
				else if (widths.Length == 2)
				{
					num7 = Mathf.Lerp(widths[1], widths[0], num4);
				}
				else
				{
					float num8 = (float)(widths.Length - 1) - num4 * (float)(widths.Length - 1);
					if (num8 == (float)(widths.Length - 1))
					{
						num7 = widths[widths.Length - 1];
					}
					else
					{
						int num9 = (int)Mathf.Floor(num8);
						float t2 = num8 - (float)num9;
						num7 = Mathf.Lerp(widths[num9], widths[num9 + 1], t2);
					}
				}
				if (faceCamera)
				{
					Vector3 vector2 = ((i != num2 - 1) ? vector : points[i - 1 + displayCnt - num2]);
					Vector3 vector3 = ((i != num2 - 1) ? points[i + 1 + displayCnt - num2] : vector);
					Vector3 lhs = vector3 - vector2;
					Vector3 rhs = position2 - vector;
					Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
					ref Vector3 reference9 = ref array[i * 2];
					reference9 = vector + normalized * num7 * 0.5f;
					ref Vector3 reference10 = ref array[i * 2 + 1];
					reference10 = vector - normalized * num7 * 0.5f;
				}
				else if (twist)
				{
					time += Time.deltaTime;
					Vector3 vector4 = ((i != num2 - 1) ? vector : points[i - 1 + displayCnt - num2]);
					Vector3 vector5 = ((i != num2 - 1) ? points[i + 1 + displayCnt - num2] : vector);
					Vector3 axis = vector5 - vector4;
					Quaternion quaternion = Quaternion.AngleAxis(Mathf.Sin(time), axis);
					Vector3 vector6 = quaternion * Vector3.up;
					vector6 = Vector3.up;
					ref Vector3 reference11 = ref array[i * 2];
					reference11 = vector + vector6 * num7 * 0.5f;
					ref Vector3 reference12 = ref array[i * 2 + 1];
					reference12 = vector - vector6 * num7 * 0.5f;
				}
				else
				{
					ref Vector3 reference13 = ref array[i * 2];
					reference13 = vector + pointsUp[i + displayCnt - num2] * num7 * 0.5f;
					ref Vector3 reference14 = ref array[i * 2 + 1];
					reference14 = vector - pointsUp[i + displayCnt - num2] * num7 * 0.5f;
				}
				ref Vector2 reference15 = ref array2[i * 2];
				reference15 = new Vector2(num4, 0f);
				ref Vector2 reference16 = ref array2[i * 2 + 1];
				reference16 = new Vector2(num4, 1f);
				if (i > 0)
				{
					int num10 = (i - 1) * 6;
					int num11 = i * 2;
					array3[num10] = num11 - 2;
					array3[num10 + 1] = num11 - 1;
					array3[num10 + 2] = num11;
					array3[num10 + 3] = num11;
					array3[num10 + 4] = num11 - 1;
					array3[num10 + 5] = num11 + 1;
				}
			}
			trail.transform.position = Vector3.zero;
			trail.transform.rotation = Quaternion.identity;
			mesh.Clear();
			mesh.vertices = array;
			mesh.colors = array4;
			mesh.uv = array2;
			mesh.triangles = array3;
		}
		catch (Exception message)
		{
			MonoBehaviour.print(message);
		}
	}
}
