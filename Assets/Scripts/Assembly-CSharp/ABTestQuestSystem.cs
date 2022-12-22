public class ABTestQuestSystem : ABTestBase
{
	public override string currentFolder
	{
		get
		{
			return "QuestSystem";
		}
	}

	protected override void ApplyState(ABTestController.ABTestCohortsType _state, object settingsB)
	{
		base.ApplyState(_state, settingsB);
		QuestSystem.Instance.Enabled = _state != ABTestController.ABTestCohortsType.B;
	}
}
