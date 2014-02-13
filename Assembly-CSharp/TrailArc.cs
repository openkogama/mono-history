using System;
using UnityEngine;

public class TrailArc : MonoBehaviour
{
	private int savedIndex;

	private int pointIndex;

	public Material material;

	private bool Emit = true;

	private bool emittingDone;

	public float minVel = 10f;

	public bool faceCamera = true;

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
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected Obj, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected Obj, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected Obj, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		saved = new Vector3[60];
		savedUp = new Vector3[saved.Length];
		points = new Vector3[saved.Length * segmentsPerPoint];
		pointsUp = new Vector3[points.Length];
		tRatio = 1f / (float)segmentsPerPoint;
		pointSqrDistance = pointDistance * pointDistance;
		trail = new GameObject("Trail");
		trail.transform.position = Vector3.zero;
		trail.transform.rotation = Quaternion.identity;
		trail.transform.localScale = Vector3.one;
		MeshFilter val = (MeshFilter)trail.AddComponent(typeof(MeshFilter));
		mesh = val.mesh;
		trail.AddComponent(typeof(MeshRenderer));
		trailMaterial = new Material(material);
		fadeOutRatio = trailMaterial.GetColor("_TintColor").a;
		trail.renderer.material = trailMaterial;
	}

	private void printPoints()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (savedCnt != 0)
		{
			string text = "Saved Points at time " + Time.time + ":\n";
			for (int i = 0; i < savedCnt; i++)
			{
				string text2 = text;
				text = string.Concat(new object[6]
				{
					text2,
					"Index: ",
					i,
					"\tPos: ",
					saved[i],
					"\n"
				});
			}
			MonoBehaviour.print((object)text);
		}
	}

	private void printAllPoints()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (pointCnt != 0)
		{
			string text = "Points at time " + Time.time + ":\n";
			for (int i = 0; i < pointCnt; i++)
			{
				string text2 = text;
				text = string.Concat(new object[6]
				{
					text2,
					"Index: ",
					i,
					"\tPos: ",
					points[i],
					"\n"
				});
			}
			MonoBehaviour.print((object)text);
		}
	}

	private void findCoordinates(int index)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		if (index != 0 && index < savedCnt - 2)
		{
			Vector3 val = saved[index - 1];
			Vector3 val2 = saved[index];
			Vector3 val3 = saved[index + 1];
			Vector3 val4 = saved[index + 2];
			Vector3 val5 = 0.5f * (val3 - val);
			Vector3 val6 = 0.5f * (val4 - val2);
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
				reference = num5 * val2 + num6 * val3 + num7 * val5 + num8 * val6;
				ref Vector3 reference2 = ref pointsUp[num9];
				reference2 = Vector3.Lerp(savedUp[index], savedUp[index + 1], num2);
			}
			pointCnt = num;
		}
	}

	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0942: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0838: Unknown result type (might be due to invalid IL or missing references)
		//IL_083d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		//IL_086a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0781: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Vector3 position = ((Component)this).transform.position;
			if (!initialized && Emit)
			{
				ref Vector3 reference = ref saved[savedCnt];
				reference = ((Component)this).transform.TransformPoint(0f, 0f, 0f - pointDistance);
				ref Vector3 reference2 = ref savedUp[savedCnt];
				reference2 = ((Component)this).transform.up;
				savedCnt++;
				saved[savedCnt] = position;
				ref Vector3 reference3 = ref savedUp[savedCnt];
				reference3 = ((Component)this).transform.up;
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
					reference4 = ((Component)this).transform.TransformPoint(0f, 0f, pointDistance);
					ref Vector3 reference5 = ref savedUp[savedCnt];
					reference5 = ((Component)this).transform.up;
					savedCnt++;
					findCoordinates(savedCnt - 3);
					ref Vector3 reference6 = ref saved[savedCnt];
					reference6 = ((Component)this).transform.TransformPoint(0f, 0f, pointDistance * 2f);
					ref Vector3 reference7 = ref savedUp[savedCnt];
					reference7 = ((Component)this).transform.up;
					savedCnt++;
					findCoordinates(savedCnt - 3);
				}
				emittingDone = true;
			}
			if (emittingDone)
			{
				Emit = false;
			}
			if (Emit)
			{
				Vector3 val = saved[savedCnt - 1] - position;
				if (val.sqrMagnitude > pointSqrDistance)
				{
					saved[savedCnt] = position;
					ref Vector3 reference8 = ref savedUp[savedCnt];
					reference8 = ((Component)this).transform.up;
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
					MonoBehaviour.print((object)("Trail effect ending with a segment count of: " + pointCnt));
				}
				Object.Destroy((Object)(object)trail);
				Object.Destroy((Object)(object)((Component)this).gameObject);
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
			if (displayCnt < 2)
			{
				trail.renderer.enabled = false;
				return;
			}
			trail.renderer.enabled = true;
			lifeTimeRatio = 1f / lifetime;
			Vector3[] array = new Vector3[displayCnt * 2];
			Vector2[] array2 = new Vector2[displayCnt * 2];
			int[] array3 = new int[(displayCnt - 1) * 6];
			Color[] array4 = new Color[displayCnt * 2];
			float num2 = 1f / (float)(displayCnt - 1);
			Vector3 position2 = ((Component)Camera.main).transform.position;
			for (int i = 0; i < displayCnt; i++)
			{
				Vector3 val2 = points[i];
				float num3 = (float)i * num2;
				Color val3;
				if (colors.Length == 0)
				{
					val3 = Color.Lerp(Color.clear, Color.white, num3);
				}
				else if (colors.Length == 1)
				{
					val3 = Color.Lerp(Color.clear, colors[0], num3);
				}
				else if (colors.Length == 2)
				{
					val3 = Color.Lerp(colors[1], colors[0], num3);
				}
				else
				{
					float num4 = (float)(colors.Length - 1) - num3 * (float)(colors.Length - 1);
					if (num4 == (float)(colors.Length - 1))
					{
						val3 = colors[colors.Length - 1];
					}
					else
					{
						int num5 = (int)Mathf.Floor(num4);
						float num6 = num4 - (float)num5;
						val3 = Color.Lerp(colors[num5], colors[num5 + 1], num6);
					}
				}
				array4[i * 2] = val3;
				array4[i * 2 + 1] = val3;
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
					num7 = Mathf.Lerp(widths[1], widths[0], num3);
				}
				else
				{
					float num8 = (float)(widths.Length - 1) - num3 * (float)(widths.Length - 1);
					if (num8 == (float)(widths.Length - 1))
					{
						num7 = widths[widths.Length - 1];
					}
					else
					{
						int num9 = (int)Mathf.Floor(num8);
						float num10 = num8 - (float)num9;
						num7 = Mathf.Lerp(widths[num9], widths[num9 + 1], num10);
					}
				}
				if (faceCamera)
				{
					Vector3 val4 = ((i != displayCnt - 1) ? val2 : points[i - 1]);
					Vector3 val5 = ((i != displayCnt - 1) ? points[i + 1] : val2);
					Vector3 val6 = val5 - val4;
					Vector3 val7 = position2 - val2;
					Vector3 val8 = Vector3.Cross(val6, val7);
					Vector3 normalized = val8.normalized;
					ref Vector3 reference9 = ref array[i * 2];
					reference9 = val2 + normalized * num7 * 0.5f;
					ref Vector3 reference10 = ref array[i * 2 + 1];
					reference10 = val2 - normalized * num7 * 0.5f;
				}
				else
				{
					ref Vector3 reference11 = ref array[i * 2];
					reference11 = val2 + pointsUp[i] * num7 * 0.5f;
					ref Vector3 reference12 = ref array[i * 2 + 1];
					reference12 = val2 - pointsUp[i] * num7 * 0.5f;
				}
				ref Vector2 reference13 = ref array2[i * 2];
				reference13 = new Vector2(num3, 0f);
				ref Vector2 reference14 = ref array2[i * 2 + 1];
				reference14 = new Vector2(num3, 1f);
				if (i > 0)
				{
					int num11 = (i - 1) * 6;
					int num12 = i * 2;
					array3[num11] = num12 - 2;
					array3[num11 + 1] = num12 - 1;
					array3[num11 + 2] = num12;
					array3[num11 + 3] = num12;
					array3[num11 + 4] = num12 - 1;
					array3[num11 + 5] = num12 + 1;
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
		catch (Exception ex)
		{
			MonoBehaviour.print((object)ex);
		}
	}
}
