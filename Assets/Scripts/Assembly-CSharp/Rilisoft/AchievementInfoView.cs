using System;
using UnityEngine;

namespace Rilisoft
{
	public class AchievementInfoView : MonoBehaviour
	{
		[SerializeField]
		private UITexture _textureAchievementsBg;

		[SerializeField]
		private UISprite _spriteIcon;

		[SerializeField]
		private TextGroup _textName;

		[SerializeField]
		private UILabel _labelDesc;

		private IDisposable _backSubscription;

		private void OnEnable()
		{
			if (_backSubscription != null)
			{
				_backSubscription.Dispose();
			}
			_backSubscription = BackSystem.Instance.Register(Hide, GetType().Name);
		}

		private void OnDisable()
		{
			if (_backSubscription != null)
			{
				_backSubscription.Dispose();
				_backSubscription = null;
			}
		}

		public void Show(Achievement ach)
		{
			base.gameObject.SetActive(true);
			_textureAchievementsBg.mainTexture = AchievementView.BackgroundTextureFor(ach);
			_spriteIcon.spriteName = ach.Data.Icon;
			_textName.Text = LocalizationStore.Get(ach.Data.LKeyName);
			string text = string.Empty;
			if (ach.Type == AchievementType.Common || ach.Type == AchievementType.Openable)
			{
				text = ((ach.Stage >= ach.MaxStage) ? string.Format("[00ff00]{0}", ach.Points) : string.Format("{0}/{1}", ach.Points, ach.ToNextStagePointsTotal));
			}
			if (ach.Type == AchievementType.Openable && ach.IsCompleted && ach.Data.Thresholds[0] == 1)
			{
				text = string.Empty;
			}
			_labelDesc.text = ((!text.IsNullOrEmpty()) ? string.Format("{0}{1}{2}", LocalizationStore.Get(ach.Data.LKeyDesc), Environment.NewLine, text) : LocalizationStore.Get(ach.Data.LKeyDesc));
		}

		public void Hide()
		{
			base.gameObject.SetActive(false);
		}
	}
}
