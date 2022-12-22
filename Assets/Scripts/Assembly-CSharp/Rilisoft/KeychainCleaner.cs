using System.Collections;
using UnityEngine;

namespace Rilisoft
{
	[DisallowMultipleComponent]
	internal sealed class KeychainCleaner : MonoBehaviour
	{
		[SerializeField]
		private GUIStyle _resetKeychainButtonStyle;

		private bool _lock;

		private KeychainCleaner()
		{
		}

		internal void AcquireLock()
		{
			_lock = true;
		}

		internal bool LockAcquired()
		{
			return _lock;
		}

		internal void ReleaseLock()
		{
			_lock = false;
		}

		private IEnumerator QuitFromEditorCoroutine()
		{
			if (Application.isEditor)
			{
				for (int i = 0; i != 2; i++)
				{
					yield return null;
				}
			}
		}

		private void Quit()
		{
			if (!Application.isEditor)
			{
				Application.Quit();
			}
			else
			{
				StartCoroutine(QuitFromEditorCoroutine());
			}
		}

		private void Clear()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
			if (!Application.isEditor)
			{
			}
		}

		private void OnResetButtonClicked()
		{
		}

		private void DrawResetKeychainButton()
		{
			Rect position = new Rect((float)Screen.width * 0.7f, 0f, (float)Screen.width * 0.3f, (float)Screen.height * 0.2f);
			_resetKeychainButtonStyle.fontSize = Mathf.RoundToInt((float)Screen.height * 0.05f);
			if (GUI.Button(position, "Начать заново", _resetKeychainButtonStyle))
			{
				OnResetButtonClicked();
			}
		}

		private void OnApplicationQuit()
		{
		}
	}
}
