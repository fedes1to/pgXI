using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class AchievementsGUIController : MonoBehaviour
	{
		[SerializeField]
		private UIScrollView _scrollView;

		[SerializeField]
		private UIGrid _grid;

		[SerializeField]
		private AchievementView _viewPrefab;

		[ReadOnly]
		[SerializeField]
		private List<AchievementView> _views = new List<AchievementView>();

		[SerializeField]
		private AchievementInfoView _infoView;

		private void Awake()
		{
			StartCoroutine(PopulateViews());
			AchievementView.OnClicked += AchievementView_OnClicked;
		}

		private IEnumerator PopulateViews()
		{
			while (!Singleton<AchievementsManager>.Instance.IsReady)
			{
				yield return null;
			}
			foreach (Achievement ach in Singleton<AchievementsManager>.Instance.AvailableAchiements)
			{
				CreateView(ach);
				_grid.Reposition();
				_scrollView.ResetPosition();
			}
			ObservableList<Achievement> availableAchiements = Singleton<AchievementsManager>.Instance.AvailableAchiements;
			availableAchiements.OnItemInserted = (Action<int, Achievement>)Delegate.Combine(availableAchiements.OnItemInserted, new Action<int, Achievement>(OnAchievementAdded));
			ObservableList<Achievement> availableAchiements2 = Singleton<AchievementsManager>.Instance.AvailableAchiements;
			availableAchiements2.OnItemRemoved = (Action<int, Achievement>)Delegate.Combine(availableAchiements2.OnItemRemoved, new Action<int, Achievement>(OnAchievementRemoved));
		}

		private AchievementView CreateView(Achievement ach)
		{
			AchievementView achievementView = UnityEngine.Object.Instantiate(_viewPrefab);
			achievementView.Achievement = ach;
			_views.Add(achievementView);
			achievementView.gameObject.transform.SetParent(_grid.gameObject.transform);
			achievementView.gameObject.transform.localPosition = Vector3.zero;
			achievementView.gameObject.transform.localScale = Vector3.one;
			return achievementView;
		}

		private void OnAchievementAdded(int pos, Achievement ach)
		{
			AchievementView achievementView = CreateView(ach);
			achievementView.gameObject.transform.SetSiblingIndex(pos);
			_grid.Reposition();
		}

		private void OnAchievementRemoved(int pos, Achievement ach)
		{
			AchievementView achievementView = _views.FirstOrDefault((AchievementView v) => v.Achievement == ach);
			if (achievementView != null)
			{
				_views.Remove(achievementView);
				achievementView.gameObject.transform.SetParent(base.gameObject.transform);
				achievementView.gameObject.SetActive(false);
				UnityEngine.Object.Destroy(achievementView);
			}
		}

		private void AchievementView_OnClicked(AchievementView obj)
		{
			_infoView.Show(obj.Achievement);
		}

		private void OnDestroy()
		{
			AchievementView.OnClicked -= AchievementView_OnClicked;
		}
	}
}
