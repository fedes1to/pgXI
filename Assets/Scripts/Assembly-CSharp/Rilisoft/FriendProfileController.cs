using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class FriendProfileController : IDisposable
	{
		private enum AccessoriesType
		{
			cape,
			hat,
			boots,
			armor,
			mask
		}

		public static string currentFriendId;

		private bool _disposed;

		private FriendProfileView _friendProfileView;

		private GameObject _friendProfileViewGo;

		private IFriendsGUIController _friendsGuiController;

		private string _friendId = string.Empty;

		private ProfileWindowType _windowType;

		private bool _needUpdateFriendList;

		private bool _isPlayerOurFriend;

		private Action<bool> OnCloseEvent;

		public GameObject FriendProfileGo
		{
			get
			{
				return _friendProfileViewGo;
			}
		}

		public FriendProfileController(IFriendsGUIController friendsGuiController, bool oldInterface = true)
		{
			Initialize(friendsGuiController, oldInterface);
		}

		public FriendProfileController(Action<bool> onCloseEvent)
		{
			Initialize(null, false);
			OnCloseEvent = onCloseEvent;
		}

		private void SetTitle(string playerId, ProfileWindowType type)
		{
			bool flag = FriendsController.IsPlayerOurClanMember(playerId);
			if (_isPlayerOurFriend && flag)
			{
				if (type == ProfileWindowType.clan)
				{
					_friendProfileView.SetTitle(LocalizationStore.Get("Key_1527"));
				}
				else
				{
					_friendProfileView.SetTitle(LocalizationStore.Get("Key_1526"));
				}
			}
			else if (_isPlayerOurFriend)
			{
				_friendProfileView.SetTitle(LocalizationStore.Get("Key_1526"));
			}
			else if (flag)
			{
				_friendProfileView.SetTitle(LocalizationStore.Get("Key_1527"));
			}
			else
			{
				_friendProfileView.SetTitle(LocalizationStore.Get("Key_1525"));
			}
		}

		private void SetupStateBottomButtons(string playerId, ProfileWindowType type)
		{
			bool flag = FriendsController.IsPlayerOurClanMember(playerId);
			bool flag2 = FriendsController.IsSelfClanLeader();
			bool flag3 = FriendsController.IsMyPlayerId(playerId);
			bool flag4 = FriendsController.IsAlreadySendInvitePlayer(playerId);
			bool flag5 = FriendsController.IsAlreadySendClanInvitePlayer(playerId);
			bool flag6 = FriendsController.IsFriendsMax();
			bool flag7 = FriendsController.IsMaxClanMembers();
			bool activeChatButton = _isPlayerOurFriend && type == ProfileWindowType.friend && !flag3;
			bool flag8 = !flag && flag2 && !flag3 && !flag7;
			bool flag9 = !_isPlayerOurFriend && !flag3 && !flag6;
			bool activeRemoveButton = _isPlayerOurFriend && !flag3;
			_friendProfileView.SetActiveAddButton(flag9 && !flag4);
			_friendProfileView.SetActiveAddButtonSent(flag9 && flag4);
			_friendProfileView.SetActiveInviteButton(flag8 && !flag5);
			_friendProfileView.SetActiveAddClanButtonSent(flag8 && flag5);
			_friendProfileView.SetActiveChatButton(activeChatButton);
			_friendProfileView.SetActiveRemoveButton(activeRemoveButton);
		}

		private void SetWindowStateByFriendAndClanData(string playerId, ProfileWindowType type)
		{
			SetTitle(playerId, type);
			SetupStateBottomButtons(playerId, type);
		}

		private void Initialize(IFriendsGUIController friendsGuiController, bool oldInterface = true)
		{
			_friendsGuiController = friendsGuiController;
			string path = ((!oldInterface) ? "FriendProfileView(UI)" : "FriendProfileView");
			_friendProfileViewGo = UnityEngine.Object.Instantiate(Resources.Load(path)) as GameObject;
			if (_friendProfileViewGo == null)
			{
				_disposed = true;
				return;
			}
			_friendProfileViewGo.SetActive(false);
			_friendProfileView = _friendProfileViewGo.GetComponent<FriendProfileView>();
			if (_friendProfileView == null)
			{
				UnityEngine.Object.DestroyObject(_friendProfileViewGo);
				_friendProfileViewGo = null;
				_disposed = true;
				return;
			}
			FriendPreviewClicker.FriendPreviewClicked += HandleProfileClicked;
			_friendProfileView.BackButtonClickEvent += HandleBackClicked;
			_friendProfileView.JoinButtonClickEvent += HandleJoinClicked;
			_friendProfileView.CopyMyIdButtonClickEvent += HandleCopyMyIdClicked;
			_friendProfileView.ChatButtonClickEvent += HandleChatClicked;
			_friendProfileView.AddButtonClickEvent += HandleAddFriendClicked;
			_friendProfileView.RemoveButtonClickEvent += HandleRemoveFriendClicked;
			_friendProfileView.InviteToClanButtonClickEvent += HandleInviteToClanClicked;
			_friendProfileView.UpdateRequested += HandleUpdateRequested;
			FriendsController.FullInfoUpdated += HandleUpdateRequested;
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				FriendPreviewClicker.FriendPreviewClicked -= HandleProfileClicked;
				_friendProfileView.BackButtonClickEvent -= HandleBackClicked;
				_friendProfileView.JoinButtonClickEvent -= HandleJoinClicked;
				_friendProfileView.CopyMyIdButtonClickEvent -= HandleCopyMyIdClicked;
				_friendProfileView.ChatButtonClickEvent -= HandleChatClicked;
				_friendProfileView.AddButtonClickEvent -= HandleAddFriendClicked;
				_friendProfileView.RemoveButtonClickEvent -= HandleRemoveFriendClicked;
				_friendProfileView.InviteToClanButtonClickEvent -= HandleInviteToClanClicked;
				_friendProfileView.UpdateRequested -= HandleUpdateRequested;
				FriendsController.FullInfoUpdated -= HandleUpdateRequested;
				_friendProfileView = null;
				UnityEngine.Object.DestroyObject(_friendProfileViewGo);
				_friendProfileViewGo = null;
				_disposed = true;
			}
		}

		private void SetDefaultStateProfile()
		{
		}

		private void UpdateAllData(string friendId)
		{
			Dictionary<string, object> fullPlayerDataById = FriendsController.GetFullPlayerDataById(friendId);
			if (fullPlayerDataById == null)
			{
				return;
			}
			UpdatePlayer(fullPlayerDataById);
			UpdateScores(fullPlayerDataById);
			UpdateAccessories(fullPlayerDataById);
			FriendsController sharedController = FriendsController.sharedController;
			if (sharedController != null)
			{
				Dictionary<string, Dictionary<string, string>> onlineInfo = sharedController.onlineInfo;
				if (onlineInfo.ContainsKey(friendId))
				{
					UpdateOnline(onlineInfo[friendId]);
				}
				else if (_isPlayerOurFriend)
				{
					_friendProfileView.Online = OnlineState.offline;
				}
				else
				{
					_friendProfileView.Online = OnlineState.none;
				}
			}
		}

		private void Update()
		{
			if (!string.IsNullOrEmpty(_friendId))
			{
				UpdateAllData(_friendId);
			}
		}

		private void UpdateAccessories(Dictionary<string, object> playerInfo)
		{
			object value;
			if (playerInfo == null || playerInfo.Count == 0 || !playerInfo.TryGetValue("accessories", out value))
			{
				return;
			}
			List<object> list = value as List<object>;
			if (list == null)
			{
				return;
			}
			IEnumerable<Dictionary<string, object>> enumerable = list.OfType<Dictionary<string, object>>();
			foreach (Dictionary<string, object> item in enumerable)
			{
				string text = string.Empty;
				object value2;
				if (item.TryGetValue("name", out value2))
				{
					text = (value2 as string) ?? string.Empty;
				}
				object value3;
				int result;
				if (!item.TryGetValue("type", out value3) || !int.TryParse(value3 as string, out result))
				{
					continue;
				}
				switch ((AccessoriesType)result)
				{
				case AccessoriesType.cape:
					if (text.Equals("cape_Custom", StringComparison.Ordinal))
					{
						object value4;
						if (item.TryGetValue("skin", out value4))
						{
							string text2 = value4 as string;
							if (!string.IsNullOrEmpty(text2))
							{
								byte[] customCape = Convert.FromBase64String(text2);
								_friendProfileView.SetCustomCape(customCape);
							}
						}
					}
					else
					{
						_friendProfileView.SetStockCape(text);
					}
					break;
				case AccessoriesType.hat:
					_friendProfileView.SetHat(text);
					break;
				case AccessoriesType.boots:
					_friendProfileView.SetBoots(text);
					break;
				case AccessoriesType.armor:
					_friendProfileView.SetArmor(text);
					break;
				case AccessoriesType.mask:
					_friendProfileView.SetMask(text);
					break;
				}
			}
		}

		private void UpdateOnline(Dictionary<string, string> onlineInfo)
		{
			FriendsController.ResultParseOnlineData resultParseOnlineData = FriendsController.ParseOnlineData(onlineInfo);
			if (resultParseOnlineData == null)
			{
				_friendProfileView.Online = OnlineState.none;
				return;
			}
			_friendProfileView.Online = resultParseOnlineData.GetOnlineStatus();
			_friendProfileView.FriendGameMode = resultParseOnlineData.GetGameModeName();
			_friendProfileView.FriendLocation = resultParseOnlineData.GetMapName();
			_friendProfileView.IsCanConnectToFriend = resultParseOnlineData.IsCanConnect;
			_friendProfileView.NotConnectCondition = resultParseOnlineData.GetNotConnectConditionString();
		}

		private void UpdatePlayer(Dictionary<string, object> playerInfo)
		{
			if (playerInfo == null || playerInfo.Count == 0)
			{
				Debug.LogWarning("playerInfo == null || playerInfo.Count == 0");
				return;
			}
			Dictionary<string, object> dictionary = null;
			dictionary = playerInfo["player"] as Dictionary<string, object>;
			if (dictionary == null)
			{
				return;
			}
			object value;
			int result;
			if (dictionary.TryGetValue("friends", out value) && int.TryParse(value as string, out result))
			{
				_friendProfileView.FriendCount = result;
			}
			else
			{
				_friendProfileView.FriendCount = -1;
			}
			object value2;
			if (dictionary.TryGetValue("nick", out value2))
			{
				_friendProfileView.FriendName = value2 as string;
			}
			object value3;
			int result2;
			if (dictionary.TryGetValue("rank", out value3) && int.TryParse(Convert.ToString(value3), out result2))
			{
				_friendProfileView.Rank = result2;
			}
			object value4;
			if (dictionary.TryGetValue("skin", out value4))
			{
				string text = value4 as string;
				if (!string.IsNullOrEmpty(text))
				{
					byte[] array = Convert.FromBase64String(text);
					if (array != null && array.Length > 0)
					{
						_friendProfileView.SetSkin(array);
					}
				}
			}
			object value5;
			if (dictionary.TryGetValue("clan_name", out value5))
			{
				_friendProfileView.clanName.gameObject.SetActive(true);
				string text2 = value5 as string;
				if (!string.IsNullOrEmpty(text2))
				{
					int num = 10000;
					if (text2 != null && text2.Length > num)
					{
						text2 = string.Format("{0}..{1}", text2.Substring(0, (num - 2) / 2), text2.Substring(text2.Length - (num - 2) / 2, (num - 2) / 2));
					}
					_friendProfileView.clanName.text = text2 ?? string.Empty;
				}
				else
				{
					_friendProfileView.clanName.gameObject.SetActive(false);
				}
			}
			object value6;
			if (dictionary.TryGetValue("clan_logo", out value6))
			{
				string text3 = value6 as string;
				if (!string.IsNullOrEmpty(text3))
				{
					_friendProfileView.clanLogo.gameObject.SetActive(true);
					byte[] array2 = Convert.FromBase64String(text3);
					if (array2 != null && array2.Length > 0)
					{
						try
						{
							Texture2D texture2D = new Texture2D(Defs.LogoWidth, Defs.LogoHeight, TextureFormat.ARGB32, false);
							texture2D.LoadImage(array2);
							texture2D.filterMode = FilterMode.Point;
							texture2D.Apply();
							Texture mainTexture = _friendProfileView.clanLogo.mainTexture;
							_friendProfileView.clanLogo.mainTexture = texture2D;
							if (mainTexture != null)
							{
								UnityEngine.Object.DestroyImmediate(mainTexture, true);
							}
						}
						catch (Exception)
						{
							Texture mainTexture2 = _friendProfileView.clanLogo.mainTexture;
							_friendProfileView.clanLogo.mainTexture = null;
							if (mainTexture2 != null)
							{
								UnityEngine.Object.DestroyImmediate(mainTexture2, true);
							}
						}
					}
				}
				else
				{
					_friendProfileView.clanLogo.gameObject.SetActive(false);
				}
			}
			string playerNameOrDefault = ProfileController.GetPlayerNameOrDefault();
			_friendProfileView.Username = playerNameOrDefault;
			object value7;
			if (dictionary.TryGetValue("wins", out value7))
			{
				int winCount = Convert.ToInt32(value7);
				_friendProfileView.WinCount = winCount;
			}
			else
			{
				_friendProfileView.WinCount = -1;
			}
			object value8;
			if (dictionary.TryGetValue("total_wins", out value8))
			{
				int result3;
				if (int.TryParse(value8 as string, out result3))
				{
					_friendProfileView.TotalWinCount = result3;
				}
				else
				{
					Debug.LogWarning("Can not parse “total_wins” field: " + value8);
				}
			}
			else
			{
				_friendProfileView.TotalWinCount = -1;
			}
		}

		private void UpdateScores(Dictionary<string, object> playerInfo)
		{
			object value;
			if (playerInfo == null || playerInfo.Count == 0 || !playerInfo.TryGetValue("scores", out value))
			{
				return;
			}
			List<object> list = value as List<object>;
			if (list == null)
			{
				return;
			}
			IEnumerable<Dictionary<string, object>> source = list.OfType<Dictionary<string, object>>();
			if (!source.Any())
			{
				return;
			}
			Dictionary<string, object> dictionary = source.FirstOrDefault((Dictionary<string, object> d) => d.ContainsKey("game") && d["game"].Equals("0"));
			if (dictionary != null)
			{
				object value2;
				int result;
				if (dictionary.TryGetValue("max_score", out value2) && int.TryParse(value2 as string, out result))
				{
					_friendProfileView.SurvivalScore = result;
				}
				else
				{
					_friendProfileView.SurvivalScore = -1;
				}
			}
		}

		internal void HandleProfileClicked(string id)
		{
			HandleProfileClickedCore(id, ProfileWindowType.other, null);
		}

		internal void HandleProfileClickedCore(string id, ProfileWindowType type, Action<bool> onCloseEvent)
		{
			if (!_disposed)
			{
				OnCloseEvent = onCloseEvent;
				_needUpdateFriendList = false;
				_friendId = id;
				_friendProfileView.FriendId = id;
				_windowType = type;
				currentFriendId = id;
				_friendProfileView.Reset();
				_isPlayerOurFriend = FriendsController.IsPlayerOurFriend(id);
				Update();
				if (_friendsGuiController != null)
				{
					_friendsGuiController.Hide(true);
				}
				FriendsController.sharedController.StartRefreshingInfo(_friendId);
				_friendProfileViewGo.SetActive(true);
				SetWindowStateByFriendAndClanData(_friendId, type);
			}
		}

		public void HandleBackClicked()
		{
			_friendProfileView.Reset();
			_friendProfileViewGo.SetActive(false);
			FriendsController.sharedController.StopRefreshingInfo();
			if (_friendsGuiController != null)
			{
				_friendsGuiController.Hide(false);
			}
			else if (OnCloseEvent != null)
			{
				OnCloseEvent(_needUpdateFriendList);
			}
		}

		private void HandleJoinClicked()
		{
			ButtonClickSound.TryPlayClick();
			if (!_friendProfileView.IsCanConnectToFriend)
			{
				InfoWindowController.ShowInfoBox(_friendProfileView.NotConnectCondition);
			}
			else if (FriendsController.sharedController.onlineInfo.ContainsKey(_friendId))
			{
				int game_mode = int.Parse(FriendsController.sharedController.onlineInfo[_friendId]["game_mode"]);
				string room_name = FriendsController.sharedController.onlineInfo[_friendId]["room_name"];
				string text = FriendsController.sharedController.onlineInfo[_friendId]["map"];
				SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(text));
				if (infoScene != null)
				{
					JoinRoomFromFrends.sharedJoinRoomFromFrends.ConnectToRoom(game_mode, room_name, text);
				}
			}
		}

		private void HandleCopyMyIdClicked()
		{
			FriendsController.CopyPlayerIdToClipboard(_friendId);
		}

		private void HandleChatClicked()
		{
			HandleBackClicked();
			if (_windowType == ProfileWindowType.friend)
			{
				FriendsWindowController instance = FriendsWindowController.Instance;
				if (instance != null)
				{
					instance.SetActiveChatTab(_friendId);
				}
			}
		}

		private void OnCompleteAddOrDeleteResponse(bool isComplete, bool isRequestExist, bool isAddRequest)
		{
			if (isAddRequest)
			{
				_friendProfileView.SetEnableAddButton(true);
			}
			else
			{
				_friendProfileView.SetEnableRemoveButton(true);
			}
			InfoWindowController.CheckShowRequestServerInfoBox(isComplete, isRequestExist);
			if (isComplete)
			{
				_needUpdateFriendList = true;
				_isPlayerOurFriend = FriendsController.IsPlayerOurFriend(_friendId);
				SetWindowStateByFriendAndClanData(_friendId, _windowType);
			}
		}

		public void CallbackRequestDeleteFriend(bool isComplete)
		{
			AnalyticsFacade.SendCustomEvent("Social", new Dictionary<string, object> { { "Deleted Friends", "Delete" } });
			OnCompleteAddOrDeleteResponse(isComplete, false, false);
		}

		public void CallbackFriendAddRequest(bool isComplete, bool isRequestExist)
		{
			OnCompleteAddOrDeleteResponse(isComplete, isRequestExist, true);
		}

		private void HandleAddFriendClicked()
		{
			_friendProfileView.SetEnableAddButton(false);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Added Friends", "Profile");
			dictionary.Add("Deleted Friends", "Add");
			Dictionary<string, object> socialEventParameters = dictionary;
			FriendsController.SendFriendshipRequest(_friendId, socialEventParameters, CallbackFriendAddRequest);
		}

		private void HandleRemoveFriendClicked()
		{
			_friendProfileView.SetEnableRemoveButton(false);
			FriendsController.DeleteFriend(_friendId, CallbackRequestDeleteFriend);
		}

		public void CallbackClanInviteRequest(bool isComplete, bool isRequestExist)
		{
			_friendProfileView.SetEnableInviteClanButton(true);
			InfoWindowController.CheckShowRequestServerInfoBox(isComplete, isRequestExist);
			if (isComplete)
			{
				SetWindowStateByFriendAndClanData(_friendId, _windowType);
			}
		}

		private void HandleInviteToClanClicked()
		{
			_friendProfileView.SetEnableInviteClanButton(false);
			FriendsController.SendPlayerInviteToClan(_friendId, CallbackClanInviteRequest);
		}

		private void HandleUpdateRequested()
		{
			Update();
		}
	}
}
