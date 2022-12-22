using UnityEngine;

public class KillerInfo
{
	public bool isSuicide;

	public bool isGrenade;

	public bool isMech;

	public bool isTurret;

	public string nickname;

	public Texture2D rankTex;

	public int rank;

	public Texture clanLogoTex;

	public string clanName;

	public string weapon;

	public Texture skinTex;

	public string hat;

	public string mask;

	public string armor;

	public string cape;

	public Texture capeTex;

	public string boots;

	public int mechUpgrade;

	public int turretUpgrade;

	public Transform killerTransform;

	public int healthValue;

	public int armorValue;

	public string pet;

	public string gadgetSupport;

	public string gadgetTrowing;

	public string gadgetTools;

	public void CopyTo(KillerInfo killerInfo)
	{
		killerInfo.isSuicide = isSuicide;
		killerInfo.nickname = nickname;
		killerInfo.rankTex = rankTex;
		killerInfo.rank = rank;
		killerInfo.clanLogoTex = clanLogoTex;
		killerInfo.clanName = clanName;
		killerInfo.weapon = weapon;
		killerInfo.skinTex = skinTex;
		killerInfo.hat = hat;
		killerInfo.mask = mask;
		killerInfo.armor = armor;
		killerInfo.cape = cape;
		killerInfo.capeTex = capeTex;
		killerInfo.boots = boots;
		killerInfo.mechUpgrade = mechUpgrade;
		killerInfo.turretUpgrade = turretUpgrade;
		killerInfo.killerTransform = killerTransform;
		killerInfo.healthValue = healthValue;
		killerInfo.armorValue = armorValue;
		killerInfo.pet = pet;
		killerInfo.gadgetSupport = gadgetSupport;
		killerInfo.gadgetTrowing = gadgetTrowing;
		killerInfo.gadgetTools = gadgetTools;
	}

	public void Reset()
	{
		isSuicide = false;
		isGrenade = false;
		isMech = false;
		isTurret = false;
		nickname = string.Empty;
		rankTex = null;
		rank = 0;
		clanLogoTex = null;
		clanName = string.Empty;
		weapon = string.Empty;
		skinTex = null;
		hat = string.Empty;
		mask = string.Empty;
		armor = string.Empty;
		cape = string.Empty;
		capeTex = null;
		boots = string.Empty;
		pet = string.Empty;
		gadgetSupport = string.Empty;
		gadgetTrowing = string.Empty;
		gadgetTools = string.Empty;
		mechUpgrade = 0;
		turretUpgrade = 0;
		killerTransform = null;
		healthValue = 0;
		armorValue = 0;
	}
}
