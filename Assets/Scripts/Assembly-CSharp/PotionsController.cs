using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PotionsController : MonoBehaviour
{
	public const string InvisibilityPotion = "InvisibilityPotion";

	public static string HastePotion;

	public static string RegenerationPotion;

	public static string MightPotion;

	public static int MaxNumOFPotions;

	public static PotionsController sharedController;

	public static Dictionary<string, KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>> potionMethods;

	public static Dictionary<string, float> potionDurations;

	public static string[] potions;

	public Dictionary<string, float> activePotions = new Dictionary<string, float>();

	public List<string> activePotionsList = new List<string>();

	private List<string> _stepPotionsToRemove;

	private List<string> _stepActivePotionKeys;

	public static float AntiGravityMult
	{
		get
		{
			return 0.75f;
		}
	}

	public static event Action<string> PotionActivated;

	public static event Action<string> PotionDisactivated;

	static PotionsController()
	{
		HastePotion = "HastePotion";
		RegenerationPotion = "RegenerationPotion";
		MightPotion = "MightPotion";
		MaxNumOFPotions = 1000000;
		sharedController = null;
		potionMethods = new Dictionary<string, KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>>();
		potionDurations = new Dictionary<string, float>();
		potions = new string[4] { HastePotion, MightPotion, RegenerationPotion, "InvisibilityPotion" };
		potionMethods.Add(HastePotion, new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(HastePotionActivation, HastePotionDeactivation));
		potionMethods.Add(MightPotion, new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(MightPotionActivation, MightPotionDeactivation));
		potionMethods.Add("InvisibilityPotion", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(InvisibilityPotionActivation, InvisibilityPotionDeactivation));
		potionMethods.Add(RegenerationPotion, new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(RegenerationPotionActivation, RegenerationPotionDeactivation));
		potionMethods.Add(GearManager.Jetpack, new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(MightPotionActivation, MightPotionDeactivation));
		potionMethods.Add(GearManager.Turret, new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(RegenerationPotionActivation, TurretPotionDeactivation));
		potionMethods.Add(GearManager.Mech, new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(MechActivation, MechDeactivation));
		potionDurations.Add(HastePotion, 180f);
		potionDurations.Add(MightPotion, 60f);
		potionDurations.Add(RegenerationPotion, 300f);
		potionDurations.Add("InvisibilityPotion0", 30f);
		potionDurations.Add(GearManager.Turret + "0", 60f);
		potionDurations.Add(GearManager.Jetpack + "0", 60f);
		potionDurations.Add(GearManager.Mech + "0", 30f);
		potionDurations.Add("InvisibilityPotion1", 30f);
		potionDurations.Add(GearManager.Turret + "1", 60f);
		potionDurations.Add(GearManager.Jetpack + "1", 60f);
		potionDurations.Add(GearManager.Mech + "1", 30f);
		potionDurations.Add("InvisibilityPotion2", 30f);
		potionDurations.Add(GearManager.Turret + "2", 60f);
		potionDurations.Add(GearManager.Jetpack + "2", 60f);
		potionDurations.Add(GearManager.Mech + "2", 30f);
		potionDurations.Add("InvisibilityPotion3", 30f);
		potionDurations.Add(GearManager.Turret + "3", 60f);
		potionDurations.Add(GearManager.Jetpack + "3", 60f);
		potionDurations.Add(GearManager.Mech + "3", 30f);
		potionDurations.Add("InvisibilityPotion4", 30f);
		potionDurations.Add(GearManager.Turret + "4", 60f);
		potionDurations.Add(GearManager.Jetpack + "4", 60f);
		potionDurations.Add(GearManager.Mech + "4", 30f);
		potionDurations.Add("InvisibilityPotion5", 30f);
		potionDurations.Add(GearManager.Turret + "5", 60f);
		potionDurations.Add(GearManager.Jetpack + "5", 60f);
		potionDurations.Add(GearManager.Mech + "5", 30f);
	}

	public bool PotionIsActive(string nm)
	{
		return nm != null && activePotions != null && activePotions.ContainsKey(nm);
	}

	public static void HastePotionActivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
		if ((bool)move_c._player && move_c._player != null)
		{
			FirstPersonControlSharp component = move_c._player.GetComponent<FirstPersonControlSharp>();
			if ((bool)component && component != null)
			{
				component.gravityMultiplier *= AntiGravityMult;
			}
		}
	}

	public static void HastePotionDeactivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
		if ((bool)move_c._player && move_c._player != null)
		{
			FirstPersonControlSharp component = move_c._player.GetComponent<FirstPersonControlSharp>();
			if ((bool)component && component != null)
			{
				component.gravityMultiplier /= AntiGravityMult;
			}
		}
	}

	public static void MightPotionActivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
		GameObject gameObject = null;
		gameObject = ((!Defs.isMulti) ? GameObject.FindGameObjectWithTag("Player") : WeaponManager.sharedManager.myPlayer);
		if (gameObject != null)
		{
			gameObject.GetComponent<SkinName>().playerMoveC.SetJetpackEnabled(true);
		}
	}

	public static void MightPotionDeactivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
		GameObject gameObject = null;
		gameObject = ((!Defs.isMulti) ? GameObject.FindGameObjectWithTag("Player") : WeaponManager.sharedManager.myPlayer);
		if (gameObject != null)
		{
			gameObject.GetComponent<SkinName>().playerMoveC.SetJetpackEnabled(false);
		}
	}

	public static void RegenerationPotionActivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
	}

	public static void RegenerationPotionDeactivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
	}

	public static void TurretPotionDeactivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
		Player_move_c player_move_c = null;
		if (Defs.isMulti)
		{
			player_move_c = WeaponManager.sharedManager.myPlayerMoveC;
		}
		else
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
			if (gameObject != null)
			{
				player_move_c = gameObject.GetComponent<SkinName>().playerMoveC;
			}
		}
		if (player_move_c == null || player_move_c.currentTurret == null)
		{
			return;
		}
		if (Defs.isMulti)
		{
			if (Defs.isInet)
			{
				PhotonNetwork.Destroy(player_move_c.currentTurret);
			}
			else
			{
				Network.Destroy(player_move_c.currentTurret);
			}
		}
		else
		{
			UnityEngine.Object.Destroy(player_move_c.currentTurret);
		}
	}

	public static void NightVisionPotionActivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
		if ((bool)move_c.inGameGUI && move_c.inGameGUI.nightVisionEffect != null)
		{
			move_c.inGameGUI.nightVisionEffect.SetActive(true);
		}
	}

	public static void NightVisionPotionDeactivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
		if ((bool)move_c.inGameGUI && move_c.inGameGUI.nightVisionEffect != null)
		{
			move_c.inGameGUI.nightVisionEffect.SetActive(false);
		}
	}

	public static void InvisibilityPotionActivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
		move_c.SetInvisible(true);
	}

	public static void InvisibilityPotionDeactivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
		move_c.SetInvisible(false);
	}

	public static void AntiGravityPotionActivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
	}

	public static void AntiGravityPotionDeactivation(Player_move_c move_c, Dictionary<string, object> pars)
	{
	}

	private static void MechActivation(Player_move_c arg1, Dictionary<string, object> arg2)
	{
		Debug.Log("Mech ON");
		arg1.ActivateMech(string.Empty);
	}

	private static void MechDeactivation(Player_move_c arg1, Dictionary<string, object> arg2)
	{
		Debug.Log("Mech OFF");
		arg1.DeactivateMech();
	}

	public float RemainDuratioForPotion(string potion)
	{
		if (potion == null || !activePotions.ContainsKey(potion))
		{
			return 0f;
		}
		return activePotions[potion] + EffectsController.AddingForPotionDuration(potion);
	}

	public void ReactivatePotions(Player_move_c move_c, Dictionary<string, object> pars)
	{
		foreach (string key in activePotions.Keys)
		{
			ActivatePotion(key, move_c, pars);
		}
	}

	public void ActivatePotion(string potion, Player_move_c move_c, Dictionary<string, object> pars, bool isAddTimeOnActive = false)
	{
		if (!activePotions.ContainsKey(potion))
		{
			activePotions.Add(potion, (!Defs.isDaterRegim) ? potionDurations[potion + GearManager.CurrentNumberOfUphradesForGear(potion)] : 180f);
			activePotionsList.Add(potion);
		}
		else if (isAddTimeOnActive)
		{
			activePotions.Remove(potion);
			activePotions.Add(potion, (!Defs.isDaterRegim) ? potionDurations[potion + GearManager.CurrentNumberOfUphradesForGear(potion)] : 180f);
			activePotionsList.Remove(potion);
			activePotionsList.Add(potion);
			if (TableGearController.sharedController != null)
			{
				TableGearController.sharedController.ReactivatePotion(potion);
			}
		}
		if (potionMethods.ContainsKey(potion))
		{
			potionMethods[potion].Key(move_c, pars);
		}
		if (PotionsController.PotionActivated != null)
		{
			PotionsController.PotionActivated(potion);
		}
	}

	public void Step(float tm, Player_move_c p)
	{
		if (_stepPotionsToRemove == null)
		{
			_stepPotionsToRemove = new List<string>();
		}
		else
		{
			_stepPotionsToRemove.Clear();
		}
		if (_stepActivePotionKeys == null)
		{
			_stepActivePotionKeys = new List<string>();
		}
		else
		{
			_stepActivePotionKeys.Clear();
		}
		Dictionary<string, float>.Enumerator enumerator = activePotions.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, float> current = enumerator.Current;
			_stepActivePotionKeys.Add(current.Key);
		}
		enumerator.Dispose();
		int count = _stepActivePotionKeys.Count;
		for (int i = 0; i < count; i++)
		{
			string text = _stepActivePotionKeys[i];
			Dictionary<string, float> dictionary;
			Dictionary<string, float> dictionary2 = (dictionary = activePotions);
			string key;
			string key2 = (key = text);
			float num = dictionary[key];
			dictionary2[key2] = num - tm;
			if (RemainDuratioForPotion(text) <= 0f)
			{
				_stepPotionsToRemove.Add(text);
			}
		}
		int count2 = _stepPotionsToRemove.Count;
		for (int j = 0; j < count2; j++)
		{
			string potion = _stepPotionsToRemove[j];
			DeActivePotion(potion, p);
		}
		_stepPotionsToRemove.Clear();
		_stepActivePotionKeys.Clear();
	}

	public void DeActivePotion(string _potion, Player_move_c p, bool isDeleteObject = true)
	{
		if (PotionsController.PotionDisactivated != null)
		{
			PotionsController.PotionDisactivated(_potion);
		}
		if (activePotions.ContainsKey(_potion))
		{
			activePotions.Remove(_potion);
			activePotionsList.Remove(_potion);
			if (isDeleteObject)
			{
				potionMethods[_potion].Value(p, new Dictionary<string, object>());
			}
		}
	}

	private void OnLevelWasLoaded(int lev)
	{
		activePotions.Clear();
		activePotionsList.Clear();
	}

	private void Start()
	{
		sharedController = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}
}
