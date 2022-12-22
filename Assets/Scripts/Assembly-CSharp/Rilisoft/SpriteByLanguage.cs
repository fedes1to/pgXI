using UnityEngine;

namespace Rilisoft
{
	[RequireComponent(typeof(UISprite))]
	public class SpriteByLanguage : MonoBehaviour
	{
		private string _prevLng = string.Empty;

		private void Update()
		{
			if (_prevLng != LocalizationStore.CurrentLanguage)
			{
				_prevLng = LocalizationStore.CurrentLanguage;
				SetLang(_prevLng);
			}
		}

		private void SetLang(string l)
		{
			if (!l.IsNullOrEmpty())
			{
				string empty = string.Empty;
				switch (l)
				{
				default:
					return;
				case "Russian":
					empty = "flag_rus";
					break;
				case "English":
					empty = "flag_us";
					break;
				case "French":
					empty = "flag_fr";
					break;
				case "German":
					empty = "flag_ger";
					break;
				case "Japanese":
					empty = "flag_jp";
					break;
				case "Spanish":
					empty = "flag_esp";
					break;
				case "Chinese (chinese)":
					empty = "flag_ch";
					break;
				case "Korean":
					empty = "flag_kr";
					break;
				case "Portuguese (brazil)":
					empty = "flag_br";
					break;
				}
				UISprite component = GetComponent<UISprite>();
				component.spriteName = empty;
			}
		}
	}
}
