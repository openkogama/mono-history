using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public static class ObscuredTypesConverter
{
	public static object CreateObscuredValue(object value)
	{
		if (value is int)
		{
			ObscuredInt obscuredInt = (int)value;
			return obscuredInt;
		}
		if (value is int[])
		{
			int[] array = (int[])value;
			ObscuredInt[] array2 = new ObscuredInt[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				ref ObscuredInt reference = ref array2[i];
				reference = array[i];
			}
			return array2;
		}
		if (value is float)
		{
			ObscuredFloat obscuredFloat = (float)value;
			return obscuredFloat;
		}
		if (value is float[])
		{
			float[] array3 = (float[])value;
			ObscuredFloat[] array4 = new ObscuredFloat[array3.Length];
			for (int j = 0; j < array3.Length; j++)
			{
				ref ObscuredFloat reference2 = ref array4[j];
				reference2 = array3[j];
			}
			return array4;
		}
		if (value is Dictionary<object, object>)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)value;
			Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
			{
				foreach (KeyValuePair<object, object> item in dictionary)
				{
					ObscuredString key = (string)item.Key;
					dictionary2[key] = CreateObscuredValue(item.Value);
				}
				return dictionary2;
			}
		}
		if (value is bool)
		{
			ObscuredBool obscuredBool = (bool)value;
			return obscuredBool;
		}
		if (value is bool[])
		{
			bool[] array5 = (bool[])value;
			ObscuredBool[] array6 = new ObscuredBool[array5.Length];
			for (int k = 0; k < array5.Length; k++)
			{
				ref ObscuredBool reference3 = ref array6[k];
				reference3 = array5[k];
			}
			return array6;
		}
		if (value is string)
		{
			return (ObscuredString)(string)value;
		}
		if (value is byte)
		{
			ObscuredByte obscuredByte = (byte)value;
			return obscuredByte;
		}
		if (value is long)
		{
			ObscuredLong obscuredLong = (long)value;
			return obscuredLong;
		}
		if (value is long[])
		{
			long[] array7 = (long[])value;
			ObscuredLong[] array8 = new ObscuredLong[array7.Length];
			for (int l = 0; l < array7.Length; l++)
			{
				ref ObscuredLong reference4 = ref array8[l];
				reference4 = array7[l];
			}
			return array8;
		}
		string arg = ((value == null) ? string.Empty : value.GetType().Name);
		throw new Exception($"Trying to write '{value}' of unsupported type '{arg}' in ObscureDictionary<object, object>");
	}

	public static object CreateUnObscuredValue(object obscuredValue)
	{
		if (obscuredValue is ObscuredInt)
		{
			int num = (ObscuredInt)obscuredValue;
			return num;
		}
		if (obscuredValue is ObscuredInt[])
		{
			ObscuredInt[] array = (ObscuredInt[])obscuredValue;
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i];
			}
			return array2;
		}
		if (obscuredValue is ObscuredFloat)
		{
			float num2 = (ObscuredFloat)obscuredValue;
			return num2;
		}
		if (obscuredValue is ObscuredFloat[])
		{
			ObscuredFloat[] array3 = (ObscuredFloat[])obscuredValue;
			float[] array4 = new float[array3.Length];
			for (int j = 0; j < array3.Length; j++)
			{
				array4[j] = array3[j];
			}
			return array4;
		}
		if (obscuredValue is Dictionary<object, object>)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)obscuredValue;
			Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
			{
				foreach (KeyValuePair<object, object> item in dictionary)
				{
					string key = (ObscuredString)item.Key;
					dictionary2[key] = CreateUnObscuredValue(item.Value);
				}
				return dictionary2;
			}
		}
		if (obscuredValue is ObscuredBool)
		{
			bool flag = (ObscuredBool)obscuredValue;
			return flag;
		}
		if (obscuredValue is ObscuredBool[])
		{
			ObscuredBool[] array5 = (ObscuredBool[])obscuredValue;
			bool[] array6 = new bool[array5.Length];
			for (int k = 0; k < array5.Length; k++)
			{
				array6[k] = array5[k];
			}
			return array6;
		}
		if (obscuredValue is ObscuredString)
		{
			return (string)(ObscuredString)obscuredValue;
		}
		if (obscuredValue is ObscuredByte)
		{
			byte b = (ObscuredByte)obscuredValue;
			return b;
		}
		if (obscuredValue is ObscuredLong)
		{
			long num3 = (ObscuredLong)obscuredValue;
			return num3;
		}
		if (obscuredValue is ObscuredLong[])
		{
			ObscuredLong[] array7 = (ObscuredLong[])obscuredValue;
			long[] array8 = new long[array7.Length];
			for (int l = 0; l < array7.Length; l++)
			{
				array8[l] = array7[l];
			}
			return array8;
		}
		throw new Exception($"Trying to to unobscure unknown type: {obscuredValue.GetType()}");
	}
}
