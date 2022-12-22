using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rilisoft
{
	public class PetsManager : Singleton<PetsManager>
	{
		private const string KEY_PLAYER_PETS = "pets";

		public const string PET_EMPTY_SLOT_SUFFIX = "_empty_slot";

		public static readonly List<string> PetsSortOrder = new List<string>
		{
			"pet_cat", "pet_dog", "pet_parrot", "pet_rabbit", "pet_bat", "pet_fox", "pet_penguin", "pet_wolf", "pet_owl", "pet_snake",
			"pet_lion", "pet_panda", "pet_eagle", "pet_deer", "pet_bear", "pet_pterodactyl", "pet_dinosaur", "pet_arnold_3000", "pet_unicorn", "pet_phoenix",
			"pet_magical_dragon"
		};

		public static Dictionary<string, PetInfo> Infos
		{
			get
			{
				return PetsInfo.info;
			}
		}

		public List<PlayerPet> PlayerPets { get; private set; }

		public static event Action OnPetsUpdated;

		public event Action<string> OnPlayerPetAdded;

		public event Action<PetUpdateInfo> OnPlayerPetChanged;

		private void OnInstanceCreated()
		{
			LoadPlayerPets();
			ActualizeEquippedPet();
		}

		public static bool IsEmptySlotId(string petId)
		{
			if (petId.IsNullOrEmpty())
			{
				Debug.LogErrorFormat("PetsManager.IsEmptySlotId: petId.IsNullOrEmpty()");
				return false;
			}
			return petId.Contains("_empty_slot");
		}

		public IEnumerable<string> PlayerPetIdsAndEmptySlots()
		{
			IEnumerable<string> enumerable = PlayerPets.Select((PlayerPet pet) => pet.InfoId);
			IEnumerable<string> first = PetsInfo.info.Keys.Select((string petId) => GetIdWithoutUp(petId)).Distinct();
			IEnumerable<string> source = first.Except(enumerable.Select((string petId) => GetIdWithoutUp(petId)));
			IEnumerable<string> second = source.Select((string petId) => string.Format("{0}{1}", petId, "_empty_slot"));
			return from petId in enumerable.Concat(second)
				orderby PetsSortOrder.IndexOf(PetIdWithoutSuffixes(petId))
				select petId;
		}

		public static string PetIdWithoutSuffixes(string petId)
		{
			if (petId.IsNullOrEmpty())
			{
				Debug.LogErrorFormat("PetIdWithoutSuffixes: petId.IsNullOrEmpty()");
				return string.Empty;
			}
			if (Singleton<PetsManager>.Instance == null)
			{
				Debug.LogErrorFormat("PetIdWithoutSuffixes: Instance == null, petId = {0}", petId);
				return petId;
			}
			string idWithoutUp = Singleton<PetsManager>.Instance.GetIdWithoutUp(petId);
			int num = idWithoutUp.IndexOf("_empty_slot");
			if (num == -1)
			{
				return idWithoutUp;
			}
			return idWithoutUp.Substring(0, num);
		}

		public static void LoadPetsToMemory()
		{
			try
			{
				if (SceneManager.GetActiveScene().name == Defs.MainMenuScene && !ShopNGUIController.GuiActive)
				{
					Singleton<PetsManager>.Instance.LoadPlayerPets();
					Singleton<PetsManager>.Instance.ActualizeEquippedPet();
					Action onPetsUpdated = PetsManager.OnPetsUpdated;
					if (onPetsUpdated != null)
					{
						onPetsUpdated();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in LoadPetsToMemory: {0}", ex);
			}
		}

		public string GetFirstSmallestUpPet(List<string> fromPets)
		{
			if (fromPets == null || !fromPets.Any())
			{
				return string.Empty;
			}
			List<string> uniquePets = (from id in fromPets.Distinct()
				select GetIdWithoutUp(id)).ToList();
			List<PlayerPet> existsPets = (from pp in PlayerPets
				where uniquePets.Contains(pp.Info.IdWithoutUp)
				orderby uniquePets.IndexOf(pp.Info.IdWithoutUp)
				select pp).ToList();
			if (!existsPets.Any())
			{
				return uniquePets.First();
			}
			if (existsPets.All((PlayerPet pp) => pp.Info.Up == 5))
			{
				return null;
			}
			if (existsPets.Count < uniquePets.Count)
			{
				return uniquePets.First((string p) => existsPets.All((PlayerPet pp) => pp.Info.IdWithoutUp != p));
			}
			PlayerPet playerPet = existsPets.First();
			foreach (PlayerPet item in existsPets)
			{
				if (playerPet.Info.Up > item.Info.Up)
				{
					playerPet = item;
				}
			}
			return playerPet.Info.IdWithoutUp;
		}

		public string GetIdWithoutUp(string petId)
		{
			if (petId.IsNullOrEmpty())
			{
				return petId;
			}
			if (!Infos.Keys.Contains(petId))
			{
				return petId;
			}
			return Infos[petId].IdWithoutUp;
		}

		public int GetUp(string petId)
		{
			if (petId.IsNullOrEmpty())
			{
				return -1;
			}
			if (!Infos.Keys.Contains(petId))
			{
				return -1;
			}
			return Infos[petId].Up;
		}

		public string GetRelativePrefabPath(string petId)
		{
			return (!Infos.ContainsKey(petId)) ? string.Empty : Infos[petId].GetRelativePrefabPath();
		}

		public PetInfo GetInfo(string petId)
		{
			return (!Infos.ContainsKey(petId)) ? GetFirstUpgrade(petId) : Infos[petId];
		}

		public PetInfo GetRandomInfo(ItemDb.ItemRarity? rarity)
		{
			string[] array = PetsIdsByRarity(rarity, true);
			if (array.Length > 0)
			{
				int num = UnityEngine.Random.Range(0, array.Length);
				return GetFirstUpgrade(array[num]);
			}
			return null;
		}

		public string[] PetsIdsByRarity(ItemDb.ItemRarity? rarity, bool ignoreUpgrades = false)
		{
			IEnumerable<PetInfo> enumerable;
			if (!rarity.HasValue)
			{
				IEnumerable<PetInfo> values = Infos.Values;
				enumerable = values;
			}
			else
			{
				enumerable = Infos.Values.Where((PetInfo i) => i.Rarity == rarity.GetValueOrDefault() && rarity.HasValue);
			}
			IEnumerable<PetInfo> source = enumerable;
			return ignoreUpgrades ? source.Select((PetInfo i) => i.IdWithoutUp).Distinct().ToArray() : source.Select((PetInfo i) => i.Id).ToArray();
		}

		public string GetEqipedPetId()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer && !Storager.hasKey("EquppedPetSN"))
			{
				Storager.setString("EquppedPetSN", string.Empty, false);
			}
			return Storager.getString("EquppedPetSN", false);
		}

		public string GetBaseEquipedPetId()
		{
			string eqipedPetId = GetEqipedPetId();
			return (!eqipedPetId.IsNullOrEmpty()) ? GetIdWithoutUp(eqipedPetId) : string.Empty;
		}

		public void SetEquipedPet(string petId)
		{
			string eqipedPetId = GetEqipedPetId();
			if (!petId.IsNullOrEmpty() && !IsExistsPet(petId))
			{
				Debug.LogErrorFormat("[PETS] pet '{0}' not found", petId);
			}
			else
			{
				Storager.setString("EquppedPetSN", petId, false);
			}
		}

		public bool IsExistsPet(string petId)
		{
			if (petId.IsNullOrEmpty())
			{
				return false;
			}
			string baseId = GetIdWithoutUp(petId);
			if (baseId.IsNullOrEmpty())
			{
				return false;
			}
			return PlayerPets.Any((PlayerPet p) => p.Info.IdWithoutUp == baseId);
		}

		public PlayerPet GetPlayerPet(string petId)
		{
			if (petId.IsNullOrEmpty())
			{
				return null;
			}
			string baseId = GetIdWithoutUp(petId);
			if (baseId.IsNullOrEmpty())
			{
				return null;
			}
			return PlayerPets.FirstOrDefault((PlayerPet p) => p.Info.IdWithoutUp == baseId);
		}

		public bool SetPetName(string petId, string newName)
		{
			if (newName.IsNullOrEmpty())
			{
				return false;
			}
			PlayerPet playerPet = GetPlayerPet(petId);
			if (playerPet == null)
			{
				return false;
			}
			playerPet.PetName = newName;
			playerPet.NameTimestamp = DateTime.UtcNow.Ticks;
			Save();
			return true;
		}

		public PetUpdateInfo AddOrUpdatePet(string petId)
		{
			PetInfo info = GetInfo(petId);
			if (info == null)
			{
				Debug.LogErrorFormat("[PETS] pet data '{0}' not found", petId);
				return null;
			}
			PetUpdateInfo petUpdateInfo = new PetUpdateInfo();
			if (IsExistsPet(petId))
			{
				PlayerPet playerPet = GetPlayerPet(petId);
				petUpdateInfo.PetOld = new PlayerPet
				{
					InfoId = playerPet.InfoId,
					Points = playerPet.Points,
					NameTimestamp = playerPet.NameTimestamp
				};
				playerPet.Points++;
				petUpdateInfo.PetNew = playerPet;
				Save();
			}
			else
			{
				string id = GetFirstUpgrade(petId).Id;
				PetInfo info2 = GetInfo(id);
				PlayerPet playerPet2 = new PlayerPet();
				playerPet2.InfoId = id;
				playerPet2.Points = 1;
				playerPet2.NameTimestamp = DateTime.UtcNow.Ticks;
				playerPet2.PetName = ((info2 == null) ? string.Empty : LocalizationStore.Get(info2.Lkey));
				PlayerPet playerPet3 = playerPet2;
				PlayerPets.Add(playerPet3);
				Save();
				if (this.OnPlayerPetAdded != null)
				{
					this.OnPlayerPetAdded(petId);
				}
				petUpdateInfo.PetNew = playerPet3;
			}
			if (this.OnPlayerPetChanged != null)
			{
				this.OnPlayerPetChanged(petUpdateInfo);
			}
			return petUpdateInfo;
		}

		public PetUpdateInfo Upgrade(string petId)
		{
			PetInfo info = GetInfo(petId);
			if (info == null)
			{
				Debug.LogErrorFormat("[PETS] pet data '{0}' not found", petId);
				return null;
			}
			PlayerPet playerPet = GetPlayerPet(petId);
			if (playerPet == null)
			{
				Debug.LogErrorFormat("[PETS] pet '{0}' not found", petId);
				return null;
			}
			PetInfo nextUp = GetNextUp(playerPet.InfoId);
			if (nextUp == null)
			{
				Debug.LogErrorFormat("[PETS] pet '{0}' buy error: next UP not found", petId);
				return null;
			}
			int num = playerPet.Info.ToUpPoints - playerPet.Points;
			if (num > 0)
			{
				Debug.LogErrorFormat("[PETS] pet '{0}' buy error: lacks points", petId);
				return null;
			}
			PetUpdateInfo petUpdateInfo = new PetUpdateInfo();
			petUpdateInfo.PetOld = new PlayerPet
			{
				InfoId = playerPet.InfoId,
				Points = playerPet.Points,
				NameTimestamp = playerPet.NameTimestamp
			};
			PetUpdateInfo petUpdateInfo2 = petUpdateInfo;
			playerPet.InfoId = nextUp.Id;
			playerPet.Points = Mathf.Abs(num);
			Save();
			petUpdateInfo2.PetNew = playerPet;
			if (this.OnPlayerPetChanged != null)
			{
				this.OnPlayerPetChanged(petUpdateInfo2);
			}
			if (petUpdateInfo2.PetOld.InfoId == GetEqipedPetId())
			{
				SetEquipedPet(petUpdateInfo2.PetNew.InfoId);
			}
			return petUpdateInfo2;
		}

		public bool NextUpAvailable(string petId)
		{
			PetInfo info = GetInfo(petId);
			if (info == null)
			{
				return false;
			}
			PlayerPet playerPet = GetPlayerPet(petId);
			if (playerPet == null)
			{
				return false;
			}
			PetInfo nextUp = GetNextUp(playerPet.InfoId);
			if (nextUp == null)
			{
				return false;
			}
			return playerPet.Info.ToUpPoints - playerPet.Points < 1;
		}

		public string[] GetAllUpgrades(string petId)
		{
			if (petId.IsNullOrEmpty())
			{
				return null;
			}
			PetInfo info = GetInfo(petId);
			if (info == null)
			{
				info = GetFirstUpgrade(petId);
			}
			if (info == null)
			{
				return null;
			}
			return (from i in Infos.Values
				where i.IdWithoutUp == info.IdWithoutUp
				orderby i.Up
				select i.Id).ToArray();
		}

		public PetInfo GetFirstUpgrade(string petId)
		{
			string id = GetIdWithoutUp(petId);
			if (id.IsNullOrEmpty())
			{
				return null;
			}
			return (from i in Infos.Values
				where i.IdWithoutUp == id
				orderby i.Up
				select i).FirstOrDefault();
		}

		public PetInfo GetFirstUnboughtPet(string petId)
		{
			if (petId.IsNullOrEmpty())
			{
				Debug.LogErrorFormat("GetFirstUnboughtPet: petId.IsNullOrEmpty()");
				return null;
			}
			if (IsEmptySlotId(petId))
			{
				Debug.LogErrorFormat("GetFirstUnboughtPet: IsEmptySlotId(petId), petId = {0}", petId);
				return null;
			}
			string text = null;
			try
			{
				List<string> list = GetAllUpgrades(petId).ToList();
				string idWithoutUp = GetIdWithoutUp(petId);
				PlayerPet playerPet = PlayerPets.FirstOrDefault((PlayerPet p) => GetIdWithoutUp(p.InfoId) == idWithoutUp);
				if (playerPet != null)
				{
					int num = list.IndexOf(playerPet.InfoId);
					if (num == -1)
					{
						Debug.LogErrorFormat("GetFirstUnboughtPet: indexOfPlayerPetInUpgrades == -1, petId = {0}", petId);
					}
					text = list[Math.Min(num + 1, list.Count - 1)];
				}
				else
				{
					text = list.First();
				}
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in GetFirstUnboughtPet: {0}", ex);
			}
			PetInfo result = null;
			try
			{
				if (text != null)
				{
					result = GetInfo(text);
					return result;
				}
				Debug.LogErrorFormat("GetFirstUnboughtPet: firstUnbought == null, petId = {0}", petId);
				return result;
			}
			catch (Exception ex2)
			{
				Debug.LogErrorFormat("Exception in GetFirstUnboughtPet, getting info of first unbought: {0}", ex2);
				return result;
			}
		}

		public PetInfo GetNextUp(string petId)
		{
			if (petId.IsNullOrEmpty())
			{
				return null;
			}
			PetInfo info = GetInfo(petId);
			if (info == null)
			{
				return null;
			}
			return Infos.Values.FirstOrDefault((PetInfo i) => i.IdWithoutUp == info.IdWithoutUp && i.Up == info.Up + 1);
		}

		public void ActualizeEquippedPet()
		{
			try
			{
				string eqipedPetId = GetEqipedPetId();
				if (eqipedPetId.IsNullOrEmpty())
				{
					return;
				}
				string baseEquippedPetId = GetIdWithoutUp(eqipedPetId);
				PlayerPet playerPet2 = PlayerPets.FirstOrDefault((PlayerPet playerPet) => GetIdWithoutUp(playerPet.InfoId) == baseEquippedPetId);
				if (playerPet2 == null)
				{
					SetEquipedPet(string.Empty);
					Debug.LogErrorFormat("ActualizeEquippedPet: equippedPetOrItsUpgrade == null, equippedPetId = {0}", eqipedPetId);
				}
				else if (!(playerPet2.InfoId == eqipedPetId))
				{
					if (playerPet2.InfoId.IsNullOrEmpty())
					{
						Debug.LogErrorFormat("ActualizeEquippedPet: equippedPetOrItsUpgrade.InfoId.IsNullOrEmpty(), equippedPetId = {0}", eqipedPetId);
					}
					else
					{
						SetEquipedPet(playerPet2.InfoId);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in ActualizeEquippedPet: {0}", ex);
			}
		}

		public void LoadPlayerPets()
		{
			PlayerPets playerPets = LoadPlayerPetsMemento();
			PlayerPets = playerPets.Pets;
		}

		private void Save()
		{
			PlayerPets playerPets = new PlayerPets(false);
			playerPets.Pets = PlayerPets;
			OverwritePlayerPetsMemento(playerPets);
		}

		internal static PlayerPets LoadPlayerPetsMemento()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer && !Storager.hasKey("pets"))
			{
				Storager.setString("pets", string.Empty, false);
			}
			string @string = Storager.getString("pets", false);
			if (@string == string.Empty)
			{
				return new PlayerPets(false);
			}
			return JsonUtility.FromJson<PlayerPets>(@string);
		}

		internal static void OverwritePlayerPetsMemento(PlayerPets memento)
		{
			string val = JsonUtility.ToJson(memento ?? new PlayerPets(false));
			Storager.setString("pets", val, false);
		}
	}
}
