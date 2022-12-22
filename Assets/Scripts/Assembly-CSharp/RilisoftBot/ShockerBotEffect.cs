using UnityEngine;

namespace RilisoftBot
{
	public class ShockerBotEffect : MonoBehaviour
	{
		[Header("Shocker damage settings")]
		public float timeToRadiusDamage = 0.5f;

		public float damageInRadius = 4f;

		public float damageValue = 0.5f;

		private float nextHitTime;

		private BaseBot myBot;

		private void Awake()
		{
			myBot = GetComponent<BaseBot>();
		}

		private void Update()
		{
			if (WeaponManager.sharedManager.myPlayer == null || !(nextHitTime < Time.time))
			{
				return;
			}
			nextHitTime = Time.time + timeToRadiusDamage;
			Initializer.TargetsList targetsList = new Initializer.TargetsList(WeaponManager.sharedManager.myPlayerMoveC, true);
			foreach (Transform item in targetsList)
			{
				if ((base.transform.position + Vector3.up - item.transform.position).sqrMagnitude <= damageInRadius * damageInRadius)
				{
					IDamageable component = item.GetComponent<IDamageable>();
					if (component != null && !component.Equals(myBot))
					{
						component.ApplyDamage(damageValue, myBot, Player_move_c.TypeKills.mob);
					}
				}
			}
		}
	}
}
