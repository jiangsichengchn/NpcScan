using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using GameData.Common;
using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Global.Inscription;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;

namespace QuicklyCreateCharacterBackend;

[PluginConfig("QuicklyCreateCharacterBackend", "Senior kangbazi", "1.0.0")]
public class QuicklyCreateCharacterBackend : TaiwuRemakePlugin
{
	private Harmony harmony;

	public static MemoryMappedFile mappedFile;

	public static bool bool_Toggle_Total;

	public static bool bool_Toggle_BackendCustomized;

	public static bool bool_IsEnterNewSave = false;

	public static bool bool_IsCreatWorld = false;

	public static ProtagonistCreationInfo protagonistCreationInfo = new ProtagonistCreationInfo();

	public static TempCharacterData characterData;

	public static CustomizedAttributeInfo customizedInfo;

	public override void OnModSettingUpdate()
	{
		DomainManager.Mod.GetSetting(((TaiwuRemakePlugin)this).ModIdStr, "Toggle_Total", ref bool_Toggle_Total);
		DomainManager.Mod.GetSetting(((TaiwuRemakePlugin)this).ModIdStr, "Toggle_BackendCustomized", ref bool_Toggle_BackendCustomized);
		int val = 0;
		int val2 = 0;
		int val3 = 0;
		int val4 = 60;
		int val5 = 0;
		int val6 = 60;
		int val7 = 0;
		int val8 = 60;
		int val9 = 1500;
		DomainManager.Mod.GetSetting(((TaiwuRemakePlugin)this).ModIdStr, "Dropdown_LifeSkillGrowthType", ref val);
		DomainManager.Mod.GetSetting(((TaiwuRemakePlugin)this).ModIdStr, "Dropdown_CombatSkillGrowthType", ref val2);
		DomainManager.Mod.GetSetting(((TaiwuRemakePlugin)this).ModIdStr, "Dropdown_LifeSkillType", ref val3);
		DomainManager.Mod.GetSetting(((TaiwuRemakePlugin)this).ModIdStr, "Slider_LifeSkillQualification", ref val4);
		DomainManager.Mod.GetSetting(((TaiwuRemakePlugin)this).ModIdStr, "Dropdown_CombatSkillType", ref val5);
		DomainManager.Mod.GetSetting(((TaiwuRemakePlugin)this).ModIdStr, "Slider_CombatSkillQualification", ref val6);
		DomainManager.Mod.GetSetting(((TaiwuRemakePlugin)this).ModIdStr, "Dropdown_MainAttributeType", ref val7);
		DomainManager.Mod.GetSetting(((TaiwuRemakePlugin)this).ModIdStr, "Slider_MainAttribute", ref val8);
		DomainManager.Mod.GetSetting(((TaiwuRemakePlugin)this).ModIdStr, "Slider_RollCountLimit", ref val9);
		if (bool_Toggle_BackendCustomized)
		{
			customizedInfo = new CustomizedAttributeInfo(val, val2, val3, val4, val5, val6, val7, val8, val9);
		}
		else
		{
			customizedInfo = null;
		}
	}

	public override void OnEnterNewWorld()
	{
		bool_IsEnterNewSave = true;
		bool_IsCreatWorld = false;
	}

	public override void OnLoadedArchiveData()
	{
		if (mappedFile != null)
		{
			mappedFile.Dispose();
		}
		bool_IsCreatWorld = false;
		bool_IsEnterNewSave = false;
	}

	public override void Initialize()
	{
		harmony = Harmony.CreateAndPatchAll(typeof(QuicklyCreateCharacterBackend), (string)null);
	}

	public override void Dispose()
	{
		if (harmony != null)
		{
			harmony.UnpatchSelf();
		}
		if (mappedFile != null)
		{
			mappedFile.Dispose();
		}
		if (customizedInfo != null)
		{
			customizedInfo = null;
		}
		bool_IsCreatWorld = false;
		bool_IsEnterNewSave = false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(CharacterDomain), "CallMethod")]
	public static bool CharacterDomain_CallMethod_PrePatch(CharacterDomain __instance, int __result, Operation operation, RawDataPool argDataPool, RawDataPool returnDataPool, DataContext context)
	{
		if (!bool_Toggle_Total)
		{
			return true;
		}
		if (operation.DomainId == 4 && operation.MethodId == 0 && operation.ArgsCount == 1 && bool_IsEnterNewSave && !bool_IsCreatWorld)
		{
			int argsOffset = operation.ArgsOffset;
			protagonistCreationInfo = null;
			argsOffset += Serializer.DeserializeDefault(argDataPool, argsOffset, ref protagonistCreationInfo);
			GetCharacterDataByInfo(protagonistCreationInfo);
			__result = -1;
			return false;
		}
		return true;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(WorldDomain), "CallMethod")]
	public static void WorldDomain_CallMethod_PostPatch(WorldDomain __instance, int __result, Operation operation, RawDataPool argDataPool, RawDataPool returnDataPool, DataContext context)
	{
		if (bool_Toggle_Total)
		{
			bool_IsCreatWorld = true;
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Character), "OfflineCreateProtagonist")]
	public static void Character_OfflineCreateProtagonist_PostPatch(Character __instance, Character.ProtagonistFeatureRelatedStatus __result, DataContext context, ProtagonistCreationInfo info)
	{
		if (bool_Toggle_Total && bool_IsEnterNewSave && bool_IsCreatWorld)
		{
			OverwriteCharacterAttribute(__instance, __result);
			bool_IsCreatWorld = false;
			bool_IsEnterNewSave = false;
			protagonistCreationInfo = null;
			characterData = null;
			if (mappedFile != null)
			{
				mappedFile.Dispose();
			}
		}
	}

	public static void GetCharacterDataByInfo(ProtagonistCreationInfo info)
	{
		if (mappedFile != null)
		{
			mappedFile.Dispose();
		}
		sbyte sectOrgTemplateIdByStateTemplateId = MapDomain.GetSectOrgTemplateIdByStateTemplateId(info.TaiwuVillageStateTemplateId);
		short memberId = OrganizationDomain.GetMemberId(sectOrgTemplateIdByStateTemplateId, 8);
		InscribedCharacter inscribedChar = info.InscribedChar;
		sbyte gender = info.Gender;
		short characterTemplateId = MapDomain.GetCharacterTemplateId(info.TaiwuVillageStateTemplateId, gender);
		Character.ProtagonistFeatureRelatedStatus status;
		Character character = CreateTempCharacter(info, characterTemplateId, memberId, out status);
		if (bool_Toggle_BackendCustomized && customizedInfo != null)
		{
			bool flag = customizedInfo.CheckIsPassed(character);
			int num = 1;
			while (!flag)
			{
				character = CreateTempCharacter(info, characterTemplateId, memberId, out status);
				flag = customizedInfo.CheckIsPassed(character);
				num++;
				if (num >= customizedInfo.rollCountLimit)
				{
					break;
				}
			}
		}
		Traverse val = Traverse.Create((object)character);
		List<short> featureIds = character.GetFeatureIds();
		sbyte lifeSkillQualificationGrowthType = character.GetLifeSkillQualificationGrowthType();
		LifeSkillShorts baseLifeSkillQualifications = character.GetBaseLifeSkillQualifications();
		sbyte combatSkillQualificationGrowthType = character.GetCombatSkillQualificationGrowthType();
		CombatSkillShorts baseCombatSkillQualifications = character.GetBaseCombatSkillQualifications();
		MainAttributes baseMainAttributes = character.GetBaseMainAttributes();
		LifeSkillShorts lifeSkill__ForDisplay = (LifeSkillShorts)val.Method("CalcLifeSkillQualifications", Array.Empty<object>()).GetValue();
		CombatSkillShorts combatSkill_ForDisplay = (CombatSkillShorts)val.Method("CalcCombatSkillQualifications", Array.Empty<object>()).GetValue();
		MainAttributes maxMainAttributes = character.GetMaxMainAttributes();
		Inventory inventory = character.GetInventory();
		TempIteamData itemDataValue = new TempIteamData(inventory);
		characterData = new TempCharacterData(character, status, lifeSkillQualificationGrowthType, combatSkillQualificationGrowthType, featureIds, baseLifeSkillQualifications, baseCombatSkillQualifications, baseMainAttributes, lifeSkill__ForDisplay, combatSkill_ForDisplay, maxMainAttributes, itemDataValue);
		TransferCharacterAttributeList();
	}

	public static Character CreateTempCharacter(ProtagonistCreationInfo info, short charTemplateId, short orgMemberId, out Character.ProtagonistFeatureRelatedStatus status)
	{
		Character character = new Character(charTemplateId);
		DataContext currentThreadDataContext = DataContextManager.GetCurrentThreadDataContext();
		status = character.OfflineCreateProtagonist(charTemplateId, orgMemberId, info, currentThreadDataContext);
		character.OfflineSetId(-1);
		ObjectCollectionDataStates objectCollectionDataStates = (ObjectCollectionDataStates)Traverse.Create((object)DomainManager.Character).Field("_dataStatesObjects").GetValue();
		character.CollectionHelperData = DomainManager.Character.HelperDataObjects;
		character.DataStatesOffset = objectCollectionDataStates.Create();
		character.SetCurrMainAttributes(character.GetMaxMainAttributes(), currentThreadDataContext);
		return character;
	}

	public static void OverwriteCharacterAttribute(Character oldCharacter, Character.ProtagonistFeatureRelatedStatus status)
	{
		if (characterData != null)
		{
			Traverse val = Traverse.Create((object)oldCharacter);
			val.Field("_lifeSkillQualificationGrowthType").SetValue((object)characterData.lifeSkillQualificationGrowthType);
			val.Field("_baseLifeSkillQualifications").SetValue((object)characterData.lifeSkillQualifications_ForOverwrite);
			val.Field("_combatSkillQualificationGrowthType").SetValue((object)characterData.combatSkillQualificationGrowthType);
			val.Field("_baseCombatSkillQualifications").SetValue((object)characterData.combatSkillQualifications_ForOverwrite);
			val.Field("_featureIds").SetValue((object)characterData.featureIds);
			val.Field("_baseMainAttributes").SetValue((object)characterData.mainAttributes_ForOverwrite);
			val.Field("_skillQualificationBonuses").SetValue((object)characterData.character.GetSkillQualificationBonuses());
			val.Field("_inventory").SetValue((object)characterData.character.GetInventory());
			val.Field("_learnedLifeSkills").SetValue((object)characterData.character.GetLearnedLifeSkills());
			val.Field("_lifeSkillQualifications").SetValue((object)characterData.character.GetLifeSkillQualifications());
			val.Field("_learnedCombatSkills").SetValue((object)characterData.character.GetLearnedCombatSkills());
			val.Field("_combatSkillQualifications").SetValue((object)characterData.character.GetCombatSkillQualifications());
			status.ReadLifeSkillTemplateId = characterData.status.ReadLifeSkillTemplateId;
			status.ReadCombatSkillTemplateId = characterData.status.ReadCombatSkillTemplateId;
			status.CombatSkillBookPageTypes = characterData.status.CombatSkillBookPageTypes;
			status.CombatSkills = characterData.status.CombatSkills;
		}
	}

	public static void TransferCharacterAttributeList()
	{
		string s = JsonSerializer.Serialize(characterData.displayList, new JsonSerializerOptions
		{
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		});
		byte[] bytes = Encoding.Unicode.GetBytes(s);
		if (mappedFile != null)
		{
			mappedFile.Dispose();
		}
		mappedFile = MemoryMappedFile.CreateOrOpen("QuicklyCreateCharacterData", bytes.Length);
		using MemoryMappedViewAccessor memoryMappedViewAccessor = mappedFile.CreateViewAccessor();
		memoryMappedViewAccessor.WriteArray(0L, bytes, 0, bytes.Length);
	}
}
