using System;
using System.Collections;
using Rilisoft;
using Unity.Linq;
using UnityEngine;

namespace RilisoftBot
{
	public class BotHpIndicator : MonoBehaviour
	{
		internal class HealthProvider
		{
			private readonly Func<float> _healthGetter;

			private readonly Func<float> _baseHealthGetter;

			public float Health
			{
				get
				{
					return _healthGetter();
				}
			}

			public float BaseHealth
			{
				get
				{
					return _baseHealthGetter();
				}
			}

			public HealthProvider(Func<float> healthGetter, Func<float> baseHealthGetter)
			{
				_healthGetter = healthGetter ?? ((Func<float>)(() => 0f));
				_baseHealthGetter = baseHealthGetter ?? ((Func<float>)(() => 0f));
			}
		}

		[SerializeField]
		private GameObject _frame;

		private float _currShowTime;

		[SerializeField]
		private Transform _healthBar;

		[SerializeField]
		[ReadOnly]
		private float _currentScale;

		private float _prevScale = 1f;

		private HealthProvider _hp;

		private IEnumerator Start()
		{
			_frame.SetActive(false);
			yield return new WaitForSeconds(0.2f);
			yield return WaitHpOwner();
			yield return UpdateIndicator();
		}

		private IEnumerator UpdateIndicator()
		{
			while (true)
			{
				if (_hp == null || _healthBar == null || Math.Abs(_hp.BaseHealth) < 0.0001f)
				{
					yield return null;
				}
				if (_hp.Health <= 0f && _frame.activeInHierarchy)
				{
					_frame.SetActive(false);
					yield return null;
				}
				_currentScale = _hp.Health / _hp.BaseHealth;
				if (Math.Abs(_currentScale - _prevScale) > 0.0001f)
				{
					_frame.SetActive(true);
					_currShowTime = 2f;
					_healthBar.localScale = new Vector3(_currentScale, _healthBar.localScale.y, _healthBar.localScale.z);
				}
				_prevScale = _currentScale;
				if (_currShowTime > 0f)
				{
					_currShowTime -= Time.deltaTime;
				}
				else
				{
					_currShowTime = 0f;
					_frame.SetActive(false);
				}
				yield return null;
			}
		}

		private IEnumerator WaitHpOwner()
		{
			bool setted = false;
			while (!setted)
			{
				foreach (GameObject pr2 in base.gameObject.AncestorsAndSelf())
				{
					BaseBot bot = pr2.GetComponent<BaseBot>();
					if (bot != null)
					{
						_hp = new HealthProvider(() => bot.health, () => bot.baseHealth);
						setted = true;
						break;
					}
				}
				foreach (GameObject pr in base.gameObject.AncestorsAndSelf())
				{
					TrainingEnemy dummy = pr.GetComponent<TrainingEnemy>();
					if (dummy != null)
					{
						_hp = new HealthProvider(() => dummy.hitPoints, () => dummy.baseHitPoints);
						setted = true;
						break;
					}
				}
				yield return null;
			}
		}
	}
}
