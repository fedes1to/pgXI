using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class InAppBonusLobbyController : MonoBehaviour
	{
		[SerializeField]
		[Header("[ scroll settings ]")]
		private PrefabHandler _scrollViewPrefabHandler;

		[SerializeField]
		private UIWrapContent _scrollViewsRoot;

		[SerializeField]
		private UIScrollView _scroll;

		[SerializeField]
		private UICenterOnChild _centerOnChild;

		[SerializeField]
		private float _rotateDelay = 2f;

		[SerializeField]
		[Header("[ buttons grid settings ]")]
		private PrefabHandler _buttonViewPrefabHandler;

		[SerializeField]
		private UIGrid _buttonsGrid;

		private readonly Dictionary<string, InAppBonusLobbyScrollView> _scrollViews = new Dictionary<string, InAppBonusLobbyScrollView>();

		private readonly Dictionary<string, InAppBonusLobbyButtonView> _buttonsViews = new Dictionary<string, InAppBonusLobbyButtonView>();

		private Vector3 _scrollStartPos;

		private Vector3 _gridStartPos;

		private bool _enabled = true;

		private float _rotateDelayLeft;

		private float _prevPanelPosX = -1f;

		private float _panelAcceleration;

		private float _loadBonusesTimeLeft;

		private List<Dictionary<string, object>> _cachedBonuses = new List<Dictionary<string, object>>();

		public static InAppBonusLobbyController Instance { get; private set; }

		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				if (value != _enabled)
				{
					if (value)
					{
						_centerOnChild.CenterOn(null);
						_scroll.transform.localPosition = Vector3.zero;
						_buttonsGrid.transform.localPosition = Vector3.zero;
						_scroll.gameObject.transform.localPosition = Vector2.zero;
						_scroll.gameObject.GetComponent<UIPanel>().clipOffset = Vector2.zero;
						_scrollViewsRoot.gameObject.transform.localPosition = Vector3.zero;
						UpdateViews();
						_scroll.ResetPosition();
						_scrollViewsRoot.WrapContent();
						_scrollViewsRoot.SortBasedOnScrollMovement();
						_buttonsGrid.Reposition();
					}
					else
					{
						_scroll.transform.localPosition = Vector3.right * 10000f;
						_buttonsGrid.transform.localPosition = Vector3.right * 10000f;
					}
					_enabled = value;
				}
			}
		}

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			_scrollStartPos = _scroll.transform.localPosition;
			_gridStartPos = _buttonsGrid.transform.localPosition;
		}

		private void Update()
		{
			if (!_enabled)
			{
				return;
			}
			_rotateDelayLeft -= Time.deltaTime;
			if (_rotateDelayLeft <= 0f)
			{
				_rotateDelayLeft = _rotateDelay;
				if (Mathf.Approximately(_panelAcceleration, 0f))
				{
					RotateScrollToNext();
				}
			}
			_panelAcceleration = Mathf.Abs(_scroll.transform.localPosition.x - _prevPanelPosX);
			_prevPanelPosX = _scroll.transform.localPosition.x;
			_loadBonusesTimeLeft -= Time.deltaTime;
			if (_loadBonusesTimeLeft <= 0f)
			{
				_loadBonusesTimeLeft = 1f;
				_cachedBonuses = BalanceController.GetCurrentInnapBonus();
			}
			UpdateViews();
		}

		private void OnDestroy()
		{
			Instance = null;
		}

		private void UpdateViews()
		{
			List<string> list = new List<string>();
			if (_cachedBonuses != null)
			{
				foreach (Dictionary<string, object> cachedBonuse in _cachedBonuses)
				{
					string text = Convert.ToString(cachedBonuse["ID"]);
					if (!text.IsNullOrEmpty())
					{
						list.Add(text);
						SetScrollView(text, cachedBonuse);
						SetButtonView(text, cachedBonuse);
					}
				}
			}
			List<string> list2 = _scrollViews.Keys.Except(list).ToList();
			if (!list2.Any())
			{
				return;
			}
			if (_scrollViews.Any())
			{
				foreach (string item in list2)
				{
					_scrollViews[item].gameObject.transform.SetParent(null);
					UnityEngine.Object.Destroy(_scrollViews[item].gameObject);
					_scrollViews.Remove(item);
				}
				_scroll.ResetPosition();
				_scrollViewsRoot.WrapContent();
				_scrollViewsRoot.SortBasedOnScrollMovement();
			}
			if (!_buttonsViews.Any())
			{
				return;
			}
			foreach (string item2 in list2)
			{
				_buttonsViews[item2].gameObject.transform.SetParent(null);
				UnityEngine.Object.Destroy(_buttonsViews[item2].gameObject);
				_buttonsViews.Remove(item2);
			}
			_buttonsGrid.Reposition();
		}

		private void SetScrollView(string id, Dictionary<string, object> bonusData)
		{
			if (_scrollViews.ContainsKey(id))
			{
				_scrollViews[id].SetData(bonusData);
			}
			else
			{
				if (_scrollViewPrefabHandler.Prefab == null)
				{
					return;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(_scrollViewPrefabHandler.Prefab);
				if (!(gameObject == null))
				{
					InAppBonusLobbyScrollView component = gameObject.GetComponent<InAppBonusLobbyScrollView>();
					if (component == null)
					{
						UnityEngine.Object.Destroy(gameObject);
						return;
					}
					component.SetData(bonusData);
					gameObject.transform.SetParent(_scrollViewsRoot.gameObject.transform);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = Vector3.one;
					_scrollViews.Add(id, component);
					_scroll.ResetPosition();
					_scrollViewsRoot.WrapContent();
					_scrollViewsRoot.SortBasedOnScrollMovement();
				}
			}
		}

		private void SetButtonView(string id, Dictionary<string, object> bonusData)
		{
			if (_buttonsViews.ContainsKey(id))
			{
				_buttonsViews[id].SetData(bonusData);
			}
			else
			{
				if (_buttonViewPrefabHandler.Prefab == null)
				{
					return;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(_buttonViewPrefabHandler.Prefab);
				if (!(gameObject == null))
				{
					InAppBonusLobbyButtonView component = gameObject.GetComponent<InAppBonusLobbyButtonView>();
					if (component == null)
					{
						UnityEngine.Object.Destroy(gameObject);
						return;
					}
					component.SetData(bonusData);
					gameObject.transform.SetParent(_buttonsGrid.gameObject.transform);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = Vector3.one;
					_buttonsViews.Add(id, component);
					_buttonsGrid.Reposition();
				}
			}
		}

		private void RotateScrollToNext()
		{
			if (_scrollViews.Count == 1)
			{
				_scroll.transform.localPosition = Vector3.zero;
				_scroll.GetComponent<UIPanel>().clipOffset = Vector3.zero;
			}
			else
			{
				if (_scrollViews.Count < 2 || !_centerOnChild.enabled)
				{
					return;
				}
				if (_centerOnChild.centeredObject == null)
				{
					_centerOnChild.CenterOn(_scrollViews.First().Value.gameObject.transform);
					return;
				}
				List<Transform> list = _centerOnChild.centeredObject.transform.Neighbors();
				if (!list.Any())
				{
					return;
				}
				if (list.Count == 1)
				{
					_centerOnChild.CenterOn(list.First());
					return;
				}
				List<Transform> source = list.Where((Transform n) => n.localPosition.x > _centerOnChild.centeredObject.transform.localPosition.x).ToList();
				if (source.Any())
				{
					Transform transform = source.OrderBy((Transform t) => t.transform.localPosition.x).First();
					if (transform != null)
					{
						_centerOnChild.CenterOn(transform);
					}
				}
			}
		}

		public void Click()
		{
			if (base.gameObject.activeSelf && MainMenuController.sharedController != null)
			{
				MainMenuController.sharedController.ShowBankWindow();
			}
		}
	}
}
