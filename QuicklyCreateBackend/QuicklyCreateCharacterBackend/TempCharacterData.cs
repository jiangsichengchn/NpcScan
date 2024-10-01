using System.Collections.Generic;
using GameData.Domains.Character;

namespace QuicklyCreateCharacterBackend;

public class TempCharacterData
{
	public Character character;

	public Character.ProtagonistFeatureRelatedStatus status;

	public sbyte lifeSkillQualificationGrowthType;

	public sbyte combatSkillQualificationGrowthType;

	public List<short> featureIds;

	public LifeSkillShorts lifeSkillQualifications_ForOverwrite;

	public CombatSkillShorts combatSkillQualifications_ForOverwrite;

	public MainAttributes mainAttributes_ForOverwrite;

	public LifeSkillShorts lifeSkillQualifications_ForDisplay;

	public CombatSkillShorts combatSkillQualifications_ForDisplay;

	public MainAttributes mainAttributes__ForDisplay;

	public TempIteamData itemData;

	public List<string> displayList;

	public TempCharacterData(Character characterValue, Character.ProtagonistFeatureRelatedStatus statusValue, sbyte lifeSkillQualificationGrowthTypeValue, sbyte combatSkillQualificationGrowthTypeValue, List<short> featureIdsValue, LifeSkillShorts lifeSkill__ForOverwrite, CombatSkillShorts combatSkill_ForOverwrite, MainAttributes attributes_ForOverwrite, LifeSkillShorts lifeSkill__ForDisplay, CombatSkillShorts combatSkill_ForDisplay, MainAttributes attributes_ForDisplay, TempIteamData itemDataValue)
	{
		character = characterValue;
		status = statusValue;
		lifeSkillQualificationGrowthType = lifeSkillQualificationGrowthTypeValue;
		combatSkillQualificationGrowthType = combatSkillQualificationGrowthTypeValue;
		featureIds = featureIdsValue;
		lifeSkillQualifications_ForOverwrite = lifeSkill__ForOverwrite;
		combatSkillQualifications_ForOverwrite = combatSkill_ForOverwrite;
		mainAttributes_ForOverwrite = attributes_ForOverwrite;
		lifeSkillQualifications_ForDisplay = lifeSkill__ForDisplay;
		combatSkillQualifications_ForDisplay = combatSkill_ForDisplay;
		mainAttributes__ForDisplay = attributes_ForDisplay;
		itemData = itemDataValue;
		UpdateTransferDisplayList();
	}

	private unsafe void UpdateTransferDisplayList()
	{
		if (displayList != null)
		{
			displayList = null;
		}
		displayList = new List<string>();
		displayList.Add("lifeSkillQualificationGrowthType");
		displayList.Add(ListToString(new List<sbyte> { lifeSkillQualificationGrowthType }) ?? "");
		displayList.Add("baseLifeSkillQualifications");
		fixed (short* ptr = lifeSkillQualifications_ForDisplay.Items)
		{
			List<short> list = new List<short>();
			for (sbyte b = 0; b < 16; b = (sbyte)(b + 1))
			{
				short item = ptr[b];
				list.Add(item);
			}
			displayList.Add(ListToString(list, ',') ?? "");
		}
		displayList.Add("combatSkillQualificationGrowthType");
		displayList.Add(ListToString(new List<sbyte> { combatSkillQualificationGrowthType }) ?? "");
		displayList.Add("baseCombatSkillQualifications");
		fixed (short* ptr2 = combatSkillQualifications_ForDisplay.Items)
		{
			List<short> list2 = new List<short>();
			for (sbyte b2 = 0; b2 < 14; b2 = (sbyte)(b2 + 1))
			{
				short item2 = ptr2[b2];
				list2.Add(item2);
			}
			displayList.Add(ListToString(list2, ',') ?? "");
		}
		displayList.Add("featureIds");
		displayList.Add(ListToString(featureIds, ',') ?? "");
		displayList.Add("mainAttributes");
		fixed (short* ptr3 = mainAttributes__ForDisplay.Items)
		{
			List<short> list3 = new List<short>();
			for (sbyte b3 = 0; b3 < 6; b3 = (sbyte)(b3 + 1))
			{
				short item3 = ptr3[b3];
				list3.Add(item3);
			}
			displayList.Add(ListToString(list3, ',') ?? "");
		}
		if (itemData != null && itemData.lifeSkillBook != null)
		{
			displayList.Add("lifeSkillBookName");
			displayList.Add(itemData.lifeSkillBook.GetName() ?? "");
			displayList.Add("lifeSkillBookType");
			displayList.Add(itemData.lifeSkillBook.GetLifeSkillType().ToString() ?? "");
		}
		if (itemData != null && itemData.combatSkillBook != null)
		{
			displayList.Add("combatSkillBookName");
			displayList.Add(itemData.combatSkillBook.GetName() ?? "");
			displayList.Add("combatSkillBookPageTypes");
			displayList.Add(ListToString(itemData.combatSkillBookPageTypes, ',') ?? "");
		}
		HitOrAvoidInts hitValues = character.GetHitValues();
		OuterAndInnerInts penetrations = character.GetPenetrations();
		HitOrAvoidInts avoidValues = character.GetAvoidValues();
		OuterAndInnerInts penetrationResists = character.GetPenetrationResists();
		OuterAndInnerShorts recoveryOfStanceAndBreath = character.GetRecoveryOfStanceAndBreath();
		short moveSpeed = character.GetMoveSpeed();
		short recoveryOfFlaw = character.GetRecoveryOfFlaw();
		short castSpeed = character.GetCastSpeed();
		short recoveryOfBlockedAcupoint = character.GetRecoveryOfBlockedAcupoint();
		short weaponSwitchSpeed = character.GetWeaponSwitchSpeed();
		short attackSpeed = character.GetAttackSpeed();
		short innerRatio = character.GetInnerRatio();
		short recoveryOfQiDisorder = character.GetRecoveryOfQiDisorder();
		displayList.Add("atkHitAttribute");
		List<int> list4 = new List<int>();
		for (sbyte b4 = 0; b4 < 4; b4 = (sbyte)(b4 + 1))
		{
			int item4 = hitValues.Items[b4];
			list4.Add(item4);
		}
		displayList.Add(ListToString(list4, ',') ?? "");
		displayList.Add("defHitAttribute");
		List<int> list5 = new List<int>();
		for (sbyte b5 = 0; b5 < 4; b5 = (sbyte)(b5 + 1))
		{
			int item5 = avoidValues.Items[b5];
			list5.Add(item5);
		}
		displayList.Add(ListToString(list5, ',') ?? "");
		displayList.Add("atkPenetrability");
		displayList.Add(ListToString(new List<int> { penetrations.Outer, penetrations.Inner }, ',') ?? "");
		displayList.Add("defPenetrability");
		displayList.Add(ListToString(new List<int> { penetrationResists.Outer, penetrationResists.Inner }, ',') ?? "");
		List<int> list6 = new List<int>();
		list6.Add(recoveryOfStanceAndBreath.Outer);
		list6.Add(recoveryOfStanceAndBreath.Inner);
		list6.Add(moveSpeed);
		list6.Add(recoveryOfFlaw);
		list6.Add(castSpeed);
		list6.Add(recoveryOfBlockedAcupoint);
		list6.Add(weaponSwitchSpeed);
		list6.Add(attackSpeed);
		list6.Add(innerRatio);
		list6.Add(recoveryOfQiDisorder);
		displayList.Add("secondaryAttribute");
		displayList.Add(ListToString(list6, ',') ?? "");
	}

	public string ListToString<T>(List<T> list, char charPstr)
	{
		string text = "";
		if (list.Count == 1)
		{
			return text + list[0].ToString();
		}
		for (int i = 0; i < list.Count; i++)
		{
			text += list[i].ToString();
			text += charPstr;
		}
		return text.TrimEnd(charPstr);
	}

	public string ListToString<T>(List<T> list)
	{
		string text = "";
		if (list.Count == 1)
		{
			return text + list[0].ToString();
		}
		for (int i = 0; i < list.Count; i++)
		{
			text += list[i].ToString();
		}
		return text;
	}
}
