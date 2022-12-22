using System;
using UnityEngine;

public class TableGearController : MonoBehaviour
{
	private enum TypeGear
	{
		Turret,
		Mech,
		Jetpack,
		InvisibilityPotion
	}

	public static TableGearController sharedController;

	public TimePotionUpdate[] potionLables;

	public UITable table;

	public UILabel activatePotionLabel;

	private string[] keysForLabel = new string[4] { "Key_1813", "Key_1810", "Key_1812", "Key_1811" };

	private string[] keysForLabelDater = new string[4] { "Key_1853", "Key_1851", "Key_1854", "Key_1852" };

	private float timerShowLabel = -1f;

	private void Start()
	{
		sharedController = this;
	}

	private void OnDestroy()
	{
		sharedController = null;
	}

	private void Update()
	{
		if (timerShowLabel > 0f)
		{
			timerShowLabel -= Time.deltaTime;
			if (timerShowLabel < 0f)
			{
				activatePotionLabel.gameObject.SetActive(false);
			}
		}
		for (int i = 0; i < potionLables.Length; i++)
		{
			if (!PotionsController.sharedController.PotionIsActive(potionLables[i].myPotionName))
			{
				if (potionLables[i].gameObject.activeSelf)
				{
					potionLables[i].gameObject.SetActive(false);
					potionLables[i].myLabel.text = string.Empty;
					table.Reposition();
				}
				continue;
			}
			if (!potionLables[i].gameObject.activeSelf)
			{
				potionLables[i].transform.GetChild(0).GetComponent<TweenScale>().enabled = true;
				potionLables[i].gameObject.SetActive(true);
				ReNameLabelObjects();
				table.Reposition();
				string myPotionName = potionLables[i].myPotionName;
				TypeGear typeGear = (TypeGear)(int)Enum.Parse(typeof(TypeGear), myPotionName);
				int num = (int)typeGear;
				activatePotionLabel.text = LocalizationStore.Get((!Defs.isDaterRegim) ? keysForLabel[num] : keysForLabelDater[num]);
				activatePotionLabel.gameObject.SetActive(true);
				timerShowLabel = 2f;
			}
			potionLables[i].timerUpdate -= Time.deltaTime;
			if (potionLables[i].timerUpdate < 0f)
			{
				potionLables[i].timerUpdate = 0.25f;
				potionLables[i].UpdateTime();
			}
		}
	}

	private void ReNameLabelObjects()
	{
		for (int i = 0; i < PotionsController.sharedController.activePotionsList.Count; i++)
		{
			string value = PotionsController.sharedController.activePotionsList[i];
			TypeGear typeGear = (TypeGear)(int)Enum.Parse(typeof(TypeGear), value);
			int num = (int)typeGear;
			potionLables[num].name = i.ToString();
		}
	}

	public void ReactivatePotion(string _potion)
	{
		TypeGear typeGear = (TypeGear)(int)Enum.Parse(typeof(TypeGear), _potion);
		int num = (int)typeGear;
		potionLables[num].transform.GetChild(0).GetComponent<TweenScale>().enabled = true;
		ReNameLabelObjects();
		table.Reposition();
		activatePotionLabel.text = LocalizationStore.Get((!Defs.isDaterRegim) ? keysForLabel[num] : keysForLabelDater[num]);
		activatePotionLabel.gameObject.SetActive(true);
		timerShowLabel = 2f;
	}
}
