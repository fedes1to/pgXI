using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Rilisoft;
using Rilisoft.DictionaryExtensions;
using Rilisoft.MiniJson;
using Rilisoft.NullExtensions;
using UnityEngine;

internal sealed class QuestSystem : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CGetQuestConfigCoroutine_003Ec__Iterator1BF : IDisposable, IEnumerator, IEnumerator<object>
	{
		internal WWW _003Cresponse_003E__0;

		internal TaskCompletionSource<string> tcs;

		internal string _003CresponseText_003E__1;

		internal int _0024PC;

		internal object _0024current;

		internal TaskCompletionSource<string> _003C_0024_003Etcs;

		internal QuestSystem _003C_003Ef__this;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _0024current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _0024current;
			}
		}

		public bool MoveNext()
		{
			//Discarded unreachable code: IL_00f9
			uint num = (uint)_0024PC;
			_0024PC = -1;
			switch (num)
			{
			case 0u:
				_003Cresponse_003E__0 = Tools.CreateWwwIfNotConnected(URLs.QuestConfig);
				if (_003Cresponse_003E__0 == null)
				{
					tcs.TrySetException(new InvalidOperationException("Skipped quest config request because the player is connected."));
					break;
				}
				_0024current = _003Cresponse_003E__0;
				_0024PC = 1;
				return true;
			case 1u:
				try
				{
					if (string.IsNullOrEmpty(_003Cresponse_003E__0.error))
					{
						_003CresponseText_003E__1 = ((_003Cresponse_003E__0.text == null) ? string.Empty : URLs.Sanitize(_003Cresponse_003E__0));
						tcs.TrySetResult(_003CresponseText_003E__1);
					}
					else
					{
						tcs.TrySetException(new InvalidOperationException(_003Cresponse_003E__0.error));
					}
				}
				finally
				{
					_003C_003E__Finally0();
				}
				_0024PC = -1;
				break;
			}
			return false;
		}

		[DebuggerHidden]
		public void Dispose()
		{
			_0024PC = -1;
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}

		private void _003C_003E__Finally0()
		{
			if (Application.isEditor)
			{
				UnityEngine.Debug.Log("<color=teal>QuestSystem.GetQuestConfigCoroutine(): response.Dispose()</color>");
			}
			_003Cresponse_003E__0.Dispose();
		}
	}

	[CompilerGenerated]
	private sealed class _003CGetConfigUpdateCoroutine_003Ec__Iterator1C0 : IDisposable, IEnumerator, IEnumerator<object>
	{
		internal WWWForm _003Cform_003E__0;

		internal WWW _003Cresponse_003E__1;

		internal TaskCompletionSource<string> tcs;

		internal string _003CresponseText_003E__2;

		internal int _0024PC;

		internal object _0024current;

		internal TaskCompletionSource<string> _003C_0024_003Etcs;

		internal QuestSystem _003C_003Ef__this;

		private static Func<FriendsController, string> _003C_003Ef__am_0024cache8;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _0024current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _0024current;
			}
		}

		public bool MoveNext()
		{
			//Discarded unreachable code: IL_01d0
			uint num = (uint)_0024PC;
			_0024PC = -1;
			switch (num)
			{
			case 0u:
			case 1u:
				if (string.IsNullOrEmpty(FriendsController.sharedController.Map((FriendsController fc) => fc.id)))
				{
					_0024current = null;
					_0024PC = 1;
					break;
				}
				_003Cform_003E__0 = new WWWForm();
				_003Cform_003E__0.AddField("action", "get_quest_version_info");
				_003Cform_003E__0.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
				_003Cform_003E__0.AddField("uniq_id", FriendsController.sharedController.id);
				_003Cform_003E__0.AddField("auth", FriendsController.Hash("get_quest_version_info"));
				_003Cresponse_003E__1 = Tools.CreateWwwIfNotConnected(FriendsController.actionAddress, _003Cform_003E__0, string.Empty);
				if (_003Cresponse_003E__1 == null)
				{
					tcs.TrySetException(new InvalidOperationException("Cannot send request while connected."));
					goto default;
				}
				_0024current = _003Cresponse_003E__1;
				_0024PC = 2;
				break;
			case 2u:
				try
				{
					if (string.IsNullOrEmpty(_003Cresponse_003E__1.error))
					{
						_003CresponseText_003E__2 = ((_003Cresponse_003E__1.text == null) ? string.Empty : URLs.Sanitize(_003Cresponse_003E__1));
						tcs.TrySetResult(_003CresponseText_003E__2);
					}
					else
					{
						tcs.TrySetException(new InvalidOperationException(_003Cresponse_003E__1.error));
					}
				}
				finally
				{
					_003C_003E__Finally0();
				}
				_0024PC = -1;
				goto default;
			default:
				return false;
			}
			return true;
		}

		[DebuggerHidden]
		public void Dispose()
		{
			_0024PC = -1;
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}

		private static string _003C_003Em__505(FriendsController fc)
		{
			return fc.id;
		}

		private void _003C_003E__Finally0()
		{
			if (Application.isEditor)
			{
				UnityEngine.Debug.Log("<color=teal>QuestSystem.GetConfigUpdateCoroutine(): response.Dispose()</color>");
			}
			_003Cresponse_003E__1.Dispose();
		}
	}

	[CompilerGenerated]
	private sealed class _003CGetTutorialQuestsConfigOnceCoroutine_003Ec__Iterator1C1 : IDisposable, IEnumerator, IEnumerator<object>
	{
		internal WWW _003Cresponse_003E__0;

		internal string _003CresponseText_003E__1;

		internal Dictionary<string, object> _003Cconfig_003E__2;

		internal List<object> _003CtutorialQuestJsons_003E__3;

		internal int _0024PC;

		internal object _0024current;

		internal QuestSystem _003C_003Ef__this;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _0024current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _0024current;
			}
		}

		public bool MoveNext()
		{
			//Discarded unreachable code: IL_01a9
			uint num = (uint)_0024PC;
			_0024PC = -1;
			switch (num)
			{
			case 0u:
				_003Cresponse_003E__0 = Tools.CreateWwwIfNotConnected(URLs.TutorialQuestConfig);
				if (_003Cresponse_003E__0 == null)
				{
					break;
				}
				_0024current = _003Cresponse_003E__0;
				_0024PC = 1;
				return true;
			case 1u:
				{
					try
					{
						if (!string.IsNullOrEmpty(_003Cresponse_003E__0.error))
						{
							UnityEngine.Debug.LogWarningFormat("Failed to load tutorial quests: {0}", _003Cresponse_003E__0.error);
							break;
						}
						_003CresponseText_003E__1 = ((_003Cresponse_003E__0.text == null) ? string.Empty : URLs.Sanitize(_003Cresponse_003E__0));
						_003Cconfig_003E__2 = Json.Deserialize(_003CresponseText_003E__1) as Dictionary<string, object>;
						if (_003Cconfig_003E__2 == null)
						{
							UnityEngine.Debug.LogWarningFormat("Failed to parse config: '{0}'", _003CresponseText_003E__1);
							break;
						}
						_003CtutorialQuestJsons_003E__3 = _003Cconfig_003E__2.TryGet("quests") as List<object>;
						if (_003C_003Ef__this._questProgress != null && !TutorialQuestManager.Instance.Received)
						{
							if (_003CtutorialQuestJsons_003E__3 != null)
							{
								TutorialQuestManager.Instance.SetReceived();
							}
							_003C_003Ef__this._questProgress.FillTutorialQuests(_003CtutorialQuestJsons_003E__3);
							_003C_003Ef__this.Updated.Do(delegate(EventHandler handler)
							{
								handler(_003C_003Ef__this, EventArgs.Empty);
							});
							_003C_003Ef__this.SaveQuestProgressIfDirty();
							TutorialQuestManager.Instance.SaveIfDirty();
						}
						goto IL_019e;
					}
					finally
					{
						_003C_003E__Finally0();
					}
				}
				IL_019e:
				_0024PC = -1;
				break;
			}
			return false;
		}

		[DebuggerHidden]
		public void Dispose()
		{
			_0024PC = -1;
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}

		internal void _003C_003Em__506(EventHandler handler)
		{
			handler(_003C_003Ef__this, EventArgs.Empty);
		}

		private void _003C_003E__Finally0()
		{
			if (Application.isEditor)
			{
				UnityEngine.Debug.Log("<color=teal>QuestSystem.GetTutorialQuestsConfigOnceCoroutine(): response.Dispose()</color>");
			}
			_003Cresponse_003E__0.Dispose();
		}
	}

	internal const string QuestProgressKey = "QuestProgress";

	internal const string DefaultAvailabilityKey = "QuestSystem.DefaultAvailability";

	private const int _questConfigClientVersion = 28;

	private bool _enabled;

	private static readonly Lazy<QuestSystem> _instance = new Lazy<QuestSystem>(InitializeInstance);

	private Coroutine _getConfigLoopCoroutine;

	private Coroutine _getTutorialQuestsConfigLoopCoroutine;

	private QuestProgress _questProgress;

	private DateTime? _startupTimeAccordingToServer;

	public static QuestSystem Instance
	{
		get
		{
			return _instance.Value;
		}
	}

	public QuestProgress QuestProgress
	{
		get
		{
			return _questProgress;
		}
	}

	internal bool Enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			PlayerPrefs.SetInt("QuestSystem.DefaultAvailability", Convert.ToInt32(value));
			if (_enabled != value)
			{
				_enabled = value;
				if (value)
				{
					InitializeQuestProgress();
				}
				else if (_questProgress != null)
				{
					_questProgress.Dispose();
					_questProgress = null;
				}
				EventHandler updated = this.Updated;
				if (updated != null)
				{
					updated(this, EventArgs.Empty);
				}
			}
		}
	}

	public bool AnyActiveQuest
	{
		get
		{
			return Enabled && QuestProgress != null && QuestProgress.AnyActiveQuest;
		}
	}

	internal int QuestConfigClientVersion
	{
		get
		{
			return 28;
		}
	}

	public event EventHandler Updated;

	public event EventHandler<QuestCompletedEventArgs> QuestCompleted;

	public void Initialize()
	{
	}

	private void Start()
	{
		if (!Enabled)
		{
			UnityEngine.Debug.Log("QuestSystem.Start(): disabled");
		}
		else
		{
			InitializeQuestProgress();
		}
	}

	private void InitializeQuestProgress()
	{
		_questProgress = LoadQuestProgress();
		if (_questProgress != null)
		{
			_questProgress.QuestCompleted += HandleQuestCompleted;
			if (!TutorialQuestManager.Instance.Received)
			{
				_getTutorialQuestsConfigLoopCoroutine.Do(base.StopCoroutine);
				_getTutorialQuestsConfigLoopCoroutine = StartCoroutine(GetTutorialQuestConfigLoopCoroutine());
			}
		}
		this.Updated.Do(delegate(EventHandler handler)
		{
			handler(this, EventArgs.Empty);
		});
		_getConfigLoopCoroutine = StartCoroutine(GetConfigLoopCoroutine(false));
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!Enabled)
		{
			UnityEngine.Debug.LogFormat("QuestSystem.OnApplicationPause({0}): disabled", pauseStatus);
		}
		else if (pauseStatus)
		{
			SaveQuestProgress(_questProgress);
			TutorialQuestManager.Instance.SaveIfDirty();
		}
		else
		{
			_getConfigLoopCoroutine.Do(base.StopCoroutine);
			_getConfigLoopCoroutine = StartCoroutine(GetConfigLoopCoroutine(true));
		}
	}

	internal void DebugDecrementDay()
	{
		if (Enabled)
		{
			if (_questProgress != null)
			{
				_questProgress.DebugDecrementDay();
			}
			this.Updated.Do(delegate(EventHandler handler)
			{
				handler(this, EventArgs.Empty);
			});
		}
	}

	internal void ForceGetConfig()
	{
		_getConfigLoopCoroutine.Do(base.StopCoroutine);
		_getConfigLoopCoroutine = StartCoroutine(GetConfigLoopCoroutine(false));
	}

	private void HandleQuestCompleted(object sender, QuestCompletedEventArgs e)
	{
		if (!Enabled)
		{
			UnityEngine.Debug.LogFormat("QuestSystem.HandleQuestCompleted('{0}'): disabled", e.Quest.Id);
			return;
		}
		SaveQuestProgressIfDirty();
		TutorialQuestManager.Instance.SaveIfDirty();
		this.QuestCompleted.Do(delegate(EventHandler<QuestCompletedEventArgs> handler)
		{
			handler(sender, e);
		});
	}

	private Task<string> GetQuestConfig()
	{
		TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
		StartCoroutine(GetQuestConfigCoroutine(taskCompletionSource));
		return taskCompletionSource.Task;
	}

	private IEnumerator GetQuestConfigCoroutine(TaskCompletionSource<string> tcs)
	{
		WWW response = Tools.CreateWwwIfNotConnected(URLs.QuestConfig);
		if (response == null)
		{
			tcs.TrySetException(new InvalidOperationException("Skipped quest config request because the player is connected."));
			yield break;
		}
		yield return response;
		try
		{
			if (string.IsNullOrEmpty(response.error))
			{
				string responseText = ((response.text == null) ? string.Empty : URLs.Sanitize(response));
				tcs.TrySetResult(responseText);
			}
			else
			{
				tcs.TrySetException(new InvalidOperationException(response.error));
			}
		}
		finally
		{
		}
	}

	private Task<string> GetConfigUpdate()
	{
		TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
		StartCoroutine(GetConfigUpdateCoroutine(taskCompletionSource));
		return taskCompletionSource.Task;
	}

	private IEnumerator GetConfigUpdateCoroutine(TaskCompletionSource<string> tcs)
	{
		while (string.IsNullOrEmpty(FriendsController.sharedController.Map((FriendsController fc) => fc.id)))
		{
			yield return null;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "get_quest_version_info");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("uniq_id", FriendsController.sharedController.id);
		form.AddField("auth", FriendsController.Hash("get_quest_version_info"));
		WWW response = Tools.CreateWwwIfNotConnected(FriendsController.actionAddress, form, string.Empty);
		if (response == null)
		{
			tcs.TrySetException(new InvalidOperationException("Cannot send request while connected."));
			yield break;
		}
		yield return response;
		try
		{
			if (string.IsNullOrEmpty(response.error))
			{
				string responseText = ((response.text == null) ? string.Empty : URLs.Sanitize(response));
				tcs.TrySetResult(responseText);
			}
			else
			{
				tcs.TrySetException(new InvalidOperationException(response.error));
			}
		}
		finally
		{
		}
	}

	private IEnumerator GetTutorialQuestsConfigOnceCoroutine()
	{
		WWW response = Tools.CreateWwwIfNotConnected(URLs.TutorialQuestConfig);
		if (response == null)
		{
			yield break;
		}
		yield return response;
		try
		{
			if (!string.IsNullOrEmpty(response.error))
			{
				UnityEngine.Debug.LogWarningFormat("Failed to load tutorial quests: {0}", response.error);
				yield break;
			}
			string responseText = ((response.text == null) ? string.Empty : URLs.Sanitize(response));
			Dictionary<string, object> config = Json.Deserialize(responseText) as Dictionary<string, object>;
			if (config == null)
			{
				UnityEngine.Debug.LogWarningFormat("Failed to parse config: '{0}'", responseText);
				yield break;
			}
			List<object> tutorialQuestJsons = config.TryGet("quests") as List<object>;
			if (_questProgress != null && !TutorialQuestManager.Instance.Received)
			{
				if (tutorialQuestJsons != null)
				{
					TutorialQuestManager.Instance.SetReceived();
				}
				_questProgress.FillTutorialQuests(tutorialQuestJsons);
				this.Updated.Do(delegate(EventHandler handler)
				{
					handler(((_003CGetTutorialQuestsConfigOnceCoroutine_003Ec__Iterator1C1)(object)this)._003C_003Ef__this, EventArgs.Empty);
				});
				SaveQuestProgressIfDirty();
				TutorialQuestManager.Instance.SaveIfDirty();
			}
		}
		finally
		{
		}
	}

	private IEnumerator GetConfigOnceCoroutine(bool resumed)
	{
		if (!Enabled)
		{
			UnityEngine.Debug.LogFormat("QuestSystem.GetConfigOnceCoroutine({0}): disabled", resumed);
			yield break;
		}
		Task<string> configUpdateRequest = GetConfigUpdate();
		while (!configUpdateRequest.IsCompleted)
		{
			yield return null;
		}
		float responceReceivedTime = Time.realtimeSinceStartup;
		if (configUpdateRequest.IsFaulted)
		{
			UnityEngine.Debug.LogWarning(configUpdateRequest.Exception.InnerException ?? configUpdateRequest.Exception);
			yield break;
		}
		Dictionary<string, object> response = Json.Deserialize(configUpdateRequest.Result) as Dictionary<string, object>;
		if (response == null)
		{
			UnityEngine.Debug.LogWarning("GetConfigOnceCoroutine(): Bad update response: " + configUpdateRequest.Result);
			yield break;
		}
		string version2 = string.Empty;
		long day2 = 0L;
		float timeLeftSeconds2 = 0f;
		DateTime timestamp2 = default(DateTime);
		try
		{
			int serverVersion = Convert.ToInt32(response["version"]);
			version2 = string.Format(arg1: QuestConfigClientVersion, format: "{0}.{1}", arg0: serverVersion);
			day2 = Convert.ToInt64(response["day"]);
			timeLeftSeconds2 = (float)Convert.ToDouble(response["timeLeftSeconds"], CultureInfo.InvariantCulture);
			long timestampUnix = Convert.ToInt64(response["timestamp"], CultureInfo.InvariantCulture);
			timestamp2 = Tools.GetCurrentTimeByUnixTime(timestampUnix);
			_startupTimeAccordingToServer = timestamp2 - TimeSpan.FromSeconds(responceReceivedTime);
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			UnityEngine.Debug.LogException(ex);
			yield break;
		}
		if (_questProgress != null && _questProgress.ConfigVersion == version2 && _questProgress.Day == day2)
		{
			yield break;
		}
		if (!Enabled)
		{
			UnityEngine.Debug.LogFormat("QuestSystem.GetConfigOnceCoroutine({0}): disabled", resumed);
			yield break;
		}
		Task<string> questConfigRequest = GetQuestConfig();
		while (!questConfigRequest.IsCompleted)
		{
			yield return null;
		}
		if (questConfigRequest.IsFaulted)
		{
			UnityEngine.Debug.LogWarning(questConfigRequest.Exception);
			yield break;
		}
		Dictionary<string, object> rawQuests = Json.Deserialize(questConfigRequest.Result) as Dictionary<string, object>;
		if (rawQuests == null)
		{
			UnityEngine.Debug.LogWarning("GetConfigOnceCoroutine(): Bad config response: " + questConfigRequest.Result);
			yield break;
		}
		List<Difficulty> allowedDifficulties = new List<Difficulty>
		{
			Difficulty.Easy,
			Difficulty.Normal,
			Difficulty.Hard
		};
		if (ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel < 3 && allowedDifficulties.Remove(Difficulty.Hard))
		{
			allowedDifficulties.Add(Difficulty.Normal);
		}
		Lazy<IDictionary<int, List<QuestBase>>> newQuests = new Lazy<IDictionary<int, List<QuestBase>>>(() => QuestProgress.CreateQuests(rawQuests, day2, allowedDifficulties.ToArray()));
		if (_questProgress == null)
		{
			_questProgress = new QuestProgress(version2, day2, timestamp2, timeLeftSeconds2);
			_getTutorialQuestsConfigLoopCoroutine.Do(base.StopCoroutine);
			_getTutorialQuestsConfigLoopCoroutine = StartCoroutine(GetTutorialQuestConfigLoopCoroutine());
			_questProgress.QuestCompleted += HandleQuestCompleted;
			_questProgress.PopulateQuests(newQuests.Value, null);
			this.Updated.Do(delegate(EventHandler handler)
			{
				handler(this, EventArgs.Empty);
			});
		}
		else if (!_questProgress.ConfigVersion.Equals(version2, StringComparison.Ordinal))
		{
			_questProgress.Dispose();
			_questProgress.QuestCompleted -= HandleQuestCompleted;
			_questProgress = new QuestProgress(version2, day2, timestamp2, timeLeftSeconds2, _questProgress);
			_getTutorialQuestsConfigLoopCoroutine.Do(base.StopCoroutine);
			_getTutorialQuestsConfigLoopCoroutine = StartCoroutine(GetTutorialQuestConfigLoopCoroutine());
			_questProgress.QuestCompleted += HandleQuestCompleted;
			_questProgress.PopulateQuests(newQuests.Value, null);
			this.Updated.Do(delegate(EventHandler handler)
			{
				handler(this, EventArgs.Empty);
			});
		}
		else if (_questProgress.Day < day2)
		{
			_questProgress.UpdateQuests(day2, rawQuests, newQuests.Value);
			this.Updated.Do(delegate(EventHandler handler)
			{
				handler(this, EventArgs.Empty);
			});
		}
		SaveQuestProgressIfDirty();
		TutorialQuestManager.Instance.SaveIfDirty();
	}

	public void SaveQuestProgressIfDirty()
	{
		if (_questProgress == null || !_questProgress.IsDirty())
		{
			return;
		}
		try
		{
			SaveQuestProgress(_questProgress);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	private IEnumerator GetConfigLoopCoroutine(bool resumed)
	{
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage < TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
		{
			yield return null;
		}
		float delaySeconds = ((!Application.isEditor) ? 600f : 30f);
		Coroutine configCoroutine = null;
		while (Enabled)
		{
			if (configCoroutine != null)
			{
				StopCoroutine(configCoroutine);
			}
			configCoroutine = StartCoroutine(GetConfigOnceCoroutine(resumed));
			yield return new WaitForRealSeconds(delaySeconds);
		}
		UnityEngine.Debug.LogFormat("QuestSystem.GetConfigLoopCoroutine({0}): disabled", resumed);
	}

	private IEnumerator GetTutorialQuestConfigLoopCoroutine()
	{
		float delaySeconds = ((!Application.isEditor) ? 600f : 30f);
		Coroutine configCoroutine = null;
		while (_questProgress == null || !TutorialQuestManager.Instance.Received)
		{
			if (!Enabled)
			{
				UnityEngine.Debug.Log("QuestSystem.GetTutorialQuestConfigLoopCoroutine({0}): disabled");
				break;
			}
			if (configCoroutine != null)
			{
				StopCoroutine(configCoroutine);
			}
			configCoroutine = StartCoroutine(GetTutorialQuestsConfigOnceCoroutine());
			yield return new WaitForRealSeconds(delaySeconds);
		}
	}

	private QuestProgress LoadQuestProgress()
	{
		if (!Storager.hasKey("QuestProgress"))
		{
			return null;
		}
		string @string = Storager.getString("QuestProgress", false);
		if (string.IsNullOrEmpty(@string))
		{
			return null;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary == null)
		{
			return null;
		}
		if (dictionary.Count == 0)
		{
			return null;
		}
		Func<string, Version> createVersion = delegate(string v)
		{
			//Discarded unreachable code: IL_000c, IL_0021
			try
			{
				return new Version(v);
			}
			catch
			{
				return new Version(0, 0, 0, 0);
			}
		};
		string text = ((dictionary.Count != 1) ? dictionary.Keys.Select((string k) => new KeyValuePair<string, Version>(k, createVersion(k))).Aggregate((KeyValuePair<string, Version> l, KeyValuePair<string, Version> r) => (!(l.Value > r.Value)) ? r : l).Key : dictionary.Keys.First());
		Dictionary<string, object> dictionary2 = dictionary[text] as Dictionary<string, object>;
		if (dictionary2 == null)
		{
			return null;
		}
		object value;
		if (!dictionary2.TryGetValue("day", out value))
		{
			return null;
		}
		object value2;
		if (!dictionary2.TryGetValue("timeLeftSeconds", out value2))
		{
			return null;
		}
		object value3;
		if (!dictionary2.TryGetValue("timestamp", out value3))
		{
			return null;
		}
		QuestProgress questProgress = null;
		try
		{
			long day = Convert.ToInt64(value, CultureInfo.InvariantCulture);
			DateTime timestamp = Convert.ToDateTime(value3, CultureInfo.InvariantCulture);
			float timeLeftSeconds = (float)Convert.ToDouble(value2, CultureInfo.InvariantCulture);
			questProgress = new QuestProgress(text, day, timestamp, timeLeftSeconds);
			Dictionary<string, object> dictionary3 = dictionary2["currentQuests"] as Dictionary<string, object>;
			if (dictionary3 == null)
			{
				return questProgress;
			}
			Dictionary<string, object> dictionary4 = dictionary2["previousQuests"] as Dictionary<string, object>;
			if (dictionary4 == null)
			{
				return questProgress;
			}
			IDictionary<int, List<QuestBase>> currentQuests = QuestProgress.RestoreQuests(dictionary3);
			IDictionary<int, List<QuestBase>> previousQuests = QuestProgress.RestoreQuests(dictionary4);
			questProgress.PopulateQuests(currentQuests, previousQuests);
			List<object> questJsons = dictionary2.TryGet("tutorialQuests") as List<object>;
			questProgress.FillTutorialQuests(questJsons);
			return questProgress;
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
			return questProgress;
		}
	}

	private static void SaveQuestProgress(QuestProgress questProgress)
	{
		if (questProgress != null)
		{
			Dictionary<string, object> value = questProgress.ToJson();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(questProgress.ConfigVersion, value);
			Dictionary<string, object> obj = dictionary;
			string text = Json.Serialize(obj);
			if (questProgress.Count == 0)
			{
				UnityEngine.Debug.LogWarning("SaveQuestProgress(): Bad progress: " + text);
				Storager.setString("QuestProgress", "{}", false);
			}
			else
			{
				Storager.setString("QuestProgress", text, false);
				questProgress.SetClean();
			}
		}
	}

	private static QuestSystem InitializeInstance()
	{
		QuestSystem questSystem = UnityEngine.Object.FindObjectOfType<QuestSystem>();
		if (questSystem != null)
		{
			UnityEngine.Object.DontDestroyOnLoad(questSystem.gameObject);
			return questSystem;
		}
		GameObject gameObject = new GameObject("Rilisoft.QuestSystem");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		QuestSystem questSystem2 = gameObject.AddComponent<QuestSystem>();
		int @int = PlayerPrefs.GetInt("QuestSystem.DefaultAvailability", 1);
		questSystem2._enabled = Convert.ToBoolean(@int);
		return questSystem2;
	}
}
