using System;
using System.Collections.Generic;
using UnityEngine;

public class AGSSyncable : IDisposable
{
	public enum SyncableMethod
	{
		getDeveloperString,
		getHighestNumber,
		getLowestNumber,
		getLatestNumber,
		getHighNumberList,
		getLowNumberList,
		getLatestNumberList,
		getAccumulatingNumber,
		getLatestString,
		getLatestStringList,
		getStringSet,
		getMap
	}

	public enum HashSetMethod
	{
		getDeveloperStringKeys,
		getHighestNumberKeys,
		getLowestNumberKeys,
		getLatestNumberKeys,
		getHighNumberListKeys,
		getLowNumberListKeys,
		getLatestNumberListKeys,
		getAccumulatingNumberKeys,
		getLatestStringKeys,
		getLatestStringListKeys,
		getStringSetKeys,
		getMapKeys
	}

	protected AmazonJavaWrapper javaObject;

	public AGSSyncable(AmazonJavaWrapper jo)
	{
		javaObject = jo;
	}

	public AGSSyncable(AndroidJavaObject jo)
	{
		javaObject = new AmazonJavaWrapper(jo);
	}

	public void Dispose()
	{
		if (javaObject != null)
		{
			javaObject.Dispose();
		}
	}

	protected AmazonJavaWrapper DictionaryToAndroidHashMap(Dictionary<string, string> dictionary)
	{
		AndroidJNI.PushLocalFrame(10);
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap");
		IntPtr methodID = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
		object[] array = new object[2];
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("java.lang.String", item.Key))
			{
				using (AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("java.lang.String", item.Value))
				{
					array[0] = androidJavaObject2;
					array[1] = androidJavaObject3;
					jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(array);
					AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, args);
				}
			}
		}
		AndroidJNI.PopLocalFrame(IntPtr.Zero);
		return new AmazonJavaWrapper(androidJavaObject);
	}

	protected T GetAGSSyncable<T>(SyncableMethod method)
	{
		return GetAGSSyncable<T>(method, null);
	}

	protected T GetAGSSyncable<T>(SyncableMethod method, string key)
	{
		AndroidJavaObject androidJavaObject = ((key == null) ? javaObject.Call<AndroidJavaObject>(method.ToString(), new object[0]) : javaObject.Call<AndroidJavaObject>(method.ToString(), new object[1] { key }));
		if (androidJavaObject != null)
		{
			return (T)Activator.CreateInstance(typeof(T), androidJavaObject);
		}
		return default(T);
	}

	protected HashSet<string> GetHashSet(HashSetMethod method)
	{
		AndroidJNI.PushLocalFrame(10);
		HashSet<string> hashSet = new HashSet<string>();
		AndroidJavaObject androidJavaObject = javaObject.Call<AndroidJavaObject>(method.ToString(), new object[0]);
		if (androidJavaObject == null)
		{
			return hashSet;
		}
		AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("iterator", new object[0]);
		if (androidJavaObject2 == null)
		{
			return hashSet;
		}
		while (androidJavaObject2.Call<bool>("hasNext", new object[0]))
		{
			string item = androidJavaObject2.Call<string>("next", new object[0]);
			hashSet.Add(item);
		}
		AndroidJNI.PopLocalFrame(IntPtr.Zero);
		return hashSet;
	}
}
