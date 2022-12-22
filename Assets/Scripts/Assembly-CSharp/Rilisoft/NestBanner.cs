using System;
using UnityEngine;

namespace Rilisoft
{
	public class NestBanner : MonoBehaviour
	{
		[SerializeField]
		private Renderer _eggRenderer;

		[SerializeField]
		private TextGroup _headerText;

		[SerializeField]
		private TextGroup _eggNameText;

		[SerializeField]
		private TextGroup _conditionText;

		[SerializeField]
		private GameObject _window;

		[SerializeField]
		private GameObject _touchBlock;

		private Egg _egg;

		private IDisposable _backSubscription;

		public bool IsVisible
		{
			get
			{
				return _window.activeInHierarchy;
			}
		}

		public event Action OnClose;

		public void Show(Egg egg)
		{
			if (_backSubscription == null)
			{
				_backSubscription = BackSystem.Instance.Register(Hide, "Nest Banner");
			}
			_egg = egg;
			_window.SetActiveSafe(true);
			_eggRenderer.material.mainTexture = Resources.Load<Texture>(_egg.GetRelativeMeshTexturePath());
			_headerText.Text = LocalizationStore.Get("Key_2675");
			string term = string.Empty;
			switch (_egg.Data.Rare)
			{
			case EggRarity.Simple:
				term = "Key_2534";
				break;
			case EggRarity.Ancient:
				term = "Key_2535";
				break;
			case EggRarity.Magical:
				term = "Key_2536";
				break;
			case EggRarity.Champion:
				term = "Key_2537";
				break;
			}
			_eggNameText.Text = LocalizationStore.Get(term);
			string text;
			switch (_egg.HatchedType)
			{
			case EggHatchedType.Time:
				UpdateTimedEggText();
				return;
			case EggHatchedType.Wins:
				text = string.Format("{0} {1}", LocalizationStore.Get("Key_2676"), _egg.Data.Wins);
				break;
			case EggHatchedType.Rating:
				text = string.Format("{0} {1}", LocalizationStore.Get("Key_2731"), _egg.Data.Rating);
				break;
			default:
				text = string.Empty;
				break;
			}
			_conditionText.Text = text;
		}

		private void Update()
		{
			UpdateTimedEggText();
		}

		private void UpdateTimedEggText()
		{
			if (_egg != null && _egg.HatchedType == EggHatchedType.Time)
			{
				TextGroup conditionText = _conditionText;
				long? incubationTimeLeft = _egg.IncubationTimeLeft;
				conditionText.Text = ((!incubationTimeLeft.HasValue || incubationTimeLeft.Value <= 0) ? LocalizationStore.Get("Key_2564") : string.Format("{0} {1}", LocalizationStore.Get("Key_2698"), EggHatchingConditionFormatter.TextForConditionOfEgg(_egg)));
			}
		}

		public void Hide()
		{
			if (_backSubscription != null)
			{
				_backSubscription.Dispose();
				_backSubscription = null;
			}
			_egg = null;
			_window.SetActiveSafe(false);
			if (this.OnClose != null)
			{
				this.OnClose();
			}
		}

		public void EnableTouchBlocker(bool enabled)
		{
			_touchBlock.SetActiveSafe(enabled);
		}
	}
}
