using System;
using UnityEngine;

[Serializable]
public class WeaponAnimEffectData
{
	public string animationName;

	public bool isLoop = true;

	[Tooltip("Запрещает перезапуск эффекта пока от играет")]
	public bool blockAtPlay = true;

	[Tooltip("Количество испускаемых частиц при старте анимации. Если количество частиц не указано, то испольхуется Play а не Emit")]
	public int EmitCount = -1;

	public ParticleSystem[] particleSystems;

	public float animationLength { get; set; }

	public bool isPlaying { get; set; }
}
