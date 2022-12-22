using System.Collections.Generic;
using UnityEngine;

public class LoadAsyncTool
{
	public class ObjectRequest
	{
		private ResourceRequest request;

		private bool done;

		private string assetPath;

		private Object _asset;

		public Object asset
		{
			get
			{
				if (_asset == null && !isDone)
				{
					LoadImmediately();
				}
				return _asset;
			}
			set
			{
				_asset = value;
			}
		}

		public bool isDone
		{
			get
			{
				if (done)
				{
					return true;
				}
				if (request.isDone)
				{
					Debug.Log("<color=#5555FF>Request done: " + assetPath + "</color>");
					asset = request.asset;
					request = null;
					done = true;
					return true;
				}
				return false;
			}
		}

		public ObjectRequest(string path, bool loadImmediately)
		{
			assetPath = path;
			if (!loadImmediately)
			{
				Debug.Log("<color=#5555FF>Load: " + assetPath + "</color>");
				request = Resources.LoadAsync(assetPath);
			}
			else
			{
				LoadImmediately();
			}
		}

		public void LoadImmediately()
		{
			if (request != null)
			{
				request = null;
			}
			asset = Resources.Load(assetPath);
			done = true;
		}
	}

	private static Dictionary<string, ObjectRequest> bufferDict = new Dictionary<string, ObjectRequest>();

	private static string[] keyBuffer = new string[70];

	private static int currentIndex;

	public static ObjectRequest Get(string path, bool loadImmediately = false)
	{
		ObjectRequest objectRequest = ((!Device.isPixelGunLow) ? GetFromBuffer(path) : null);
		if (objectRequest == null)
		{
			objectRequest = new ObjectRequest(path, loadImmediately);
			if (!Device.isPixelGunLow)
			{
				AddToBuffer(path, objectRequest);
			}
		}
		return objectRequest;
	}

	private static void AddToBuffer(string key, ObjectRequest value)
	{
		if (bufferDict.ContainsKey(key))
		{
			return;
		}
		if (keyBuffer[currentIndex] != null)
		{
			bufferDict[keyBuffer[currentIndex]].asset = null;
			bufferDict.Remove(keyBuffer[currentIndex]);
		}
		keyBuffer[currentIndex] = key;
		bufferDict.Add(key, value);
		currentIndex++;
		if (currentIndex >= keyBuffer.Length)
		{
			if (Device.isPixelGunLow)
			{
				Resources.UnloadUnusedAssets();
				Debug.Log("<color=#FF5555>Resources.UnloadUnusedAssets</color>");
			}
			currentIndex = 0;
		}
	}

	private static ObjectRequest GetFromBuffer(string key)
	{
		if (bufferDict.ContainsKey(key))
		{
			return bufferDict[key];
		}
		return null;
	}
}
