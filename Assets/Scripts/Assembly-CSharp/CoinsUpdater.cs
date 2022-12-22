using System.Collections;
using UnityEngine;

internal sealed class CoinsUpdater : MonoBehaviour
{
	public static readonly string trainCoinsStub = "999";

	private UILabel coinsLabel;

	private string _trainingMsg = "0";

	private bool _disposed;

	private void Start()
	{
		coinsLabel = GetComponent<UILabel>();
		CoinsMessage.CoinsLabelDisappeared += _ReplaceMsgForTraining;
		string text = Storager.getInt("Coins", false).ToString();
		if (coinsLabel != null)
		{
			coinsLabel.text = text;
		}
	}

	private void OnEnable()
	{
		BankController.onUpdateMoney += UpdateMoney;
		StartCoroutine(UpdateCoinsLabel());
	}

	private void OnDisable()
	{
		BankController.onUpdateMoney -= UpdateMoney;
	}

	private void _ReplaceMsgForTraining(bool isGems, int count)
	{
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			_trainingMsg = trainCoinsStub;
		}
	}

	private IEnumerator UpdateCoinsLabel()
	{
		while (!_disposed)
		{
			if (!BankController.canShowIndication)
			{
				yield return null;
				continue;
			}
			UpdateMoney();
			yield return StartCoroutine(MyWaitForSeconds(1f));
		}
	}

	private void UpdateMoney()
	{
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			if (coinsLabel != null)
			{
				if (ShopNGUIController.sharedShop != null && ShopNGUIController.GuiActive)
				{
					coinsLabel.text = "999";
				}
				else
				{
					coinsLabel.text = _trainingMsg;
				}
			}
		}
		else
		{
			string text = Storager.getInt("Coins", false).ToString();
			if (coinsLabel != null)
			{
				coinsLabel.text = text;
			}
		}
	}

	public IEnumerator MyWaitForSeconds(float tm)
	{
		float startTime = Time.realtimeSinceStartup;
		do
		{
			yield return null;
		}
		while (Time.realtimeSinceStartup - startTime < tm);
	}

	private void OnDestroy()
	{
		CoinsMessage.CoinsLabelDisappeared -= _ReplaceMsgForTraining;
		_disposed = true;
	}
}
