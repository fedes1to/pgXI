using UnityEngine;

namespace Rilisoft
{
	public class LeprechauntAnimatorStateBehaviour : StateMachineBehaviour
	{
		[SerializeField]
		private string _stateName;

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (!(LeprechauntLobbyView.Instance == null))
			{
				LeprechauntLobbyView.Instance.OnAnimatorStateExit(_stateName);
			}
		}
	}
}
