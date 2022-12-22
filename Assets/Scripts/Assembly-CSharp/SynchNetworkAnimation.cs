using System.Collections;
using UnityEngine;

public class SynchNetworkAnimation : MonoBehaviour
{
	private Animation anim;

	private AnimationState currState;

	private void Start()
	{
		if (Defs.isMulti && Defs.isInet)
		{
			anim = GetComponent<Animation>();
			currState = anim[anim.clip.name];
			currState.normalizedTime = (float)(PhotonNetwork.time % (double)anim.clip.length) / anim.clip.length;
			anim.Play();
			StartCoroutine(UpdateState());
		}
	}

	private IEnumerator UpdateState()
	{
		while (true)
		{
			currState.normalizedTime = (float)(PhotonNetwork.time % (double)anim.clip.length) / anim.clip.length;
			yield return new WaitForSeconds(10f);
		}
	}
}
