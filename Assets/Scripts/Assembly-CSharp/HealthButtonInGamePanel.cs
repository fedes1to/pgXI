using UnityEngine;

public sealed class HealthButtonInGamePanel : MonoBehaviour
{
	public GameObject fullLabel;

	public UIButton myButton;

	public UILabel priceLabel;

	public InGameGUI inGameGui;

	private void Start()
	{
		priceLabel.text = Defs.healthInGamePanelPrice.ToString();
	}

	private void Update()
	{
		UpdateState(true);
	}

	private void UpdateState(bool isDelta = true)
	{
		if (!(inGameGui.playerMoveC == null))
		{
			bool flag = inGameGui.playerMoveC.CurHealth == inGameGui.playerMoveC.MaxHealth;
			if (fullLabel.activeSelf != flag)
			{
				fullLabel.SetActive(flag);
			}
			myButton.isEnabled = !flag;
			if (priceLabel.gameObject.activeSelf != !flag)
			{
				priceLabel.gameObject.SetActive(!flag);
			}
		}
	}

	private void OnEnable()
	{
		UpdateState(false);
	}

	private void OnClick()
	{
		if (ButtonClickSound.Instance != null)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			if (inGameGui.playerMoveC != null)
			{
				inGameGui.playerMoveC.CurHealth = inGameGui.playerMoveC.MaxHealth;
			}
		}
		else
		{
			if (inGameGui.playerMoveC.CurHealth <= 0f)
			{
				return;
			}
			ShopNGUIController.TryToBuy(inGameGui.gameObject, new ItemPrice(Defs.healthInGamePanelPrice, "Coins"), delegate
			{
				if (inGameGui.playerMoveC != null)
				{
					inGameGui.playerMoveC.CurHealth = inGameGui.playerMoveC.MaxHealth;
					inGameGui.playerMoveC.ShowBonuseParticle(Player_move_c.TypeBonuses.Health);
					inGameGui.playerMoveC.timeBuyHealth = Time.time;
				}
			}, JoystickController.leftJoystick.Reset);
		}
	}
}
