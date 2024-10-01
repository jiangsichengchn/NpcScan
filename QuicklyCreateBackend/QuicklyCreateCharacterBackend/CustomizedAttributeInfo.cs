using System;
using GameData.Domains.Character;
using HarmonyLib;

namespace QuicklyCreateCharacterBackend;

public class CustomizedAttributeInfo
{
	public bool bool_LifeGrowthType;

	public sbyte value_LifeGrowthType;

	public bool bool_CombatGrowthType;

	public sbyte value_CombatGrowthType;

	public bool bool_LifeQulification;

	public int type_LifeQulification;

	public int value_LifeQulification;

	public bool bool_CombatQulification;

	public int type_CombatQulification;

	public int value_CombatQulification;

	public bool bool_MainAttribute;

	public int type_MainAttribute;

	public int value_MainAttribute;

	public int rollCountLimit;

	public CustomizedAttributeInfo(int Dropdown_LifeSkillGrowthType, int Dropdown_CombatSkillGrowthType, int Dropdown_LifeSkillType, int Slider_LifeSkillQualification, int Dropdown_CombatSkillType, int Slider_CombatSkillQualification, int Dropdown_MainAttributeType, int Slider_MainAttribute, int Slider_RollCountLimit)
	{
		bool_LifeGrowthType = Dropdown_LifeSkillGrowthType > 0;
		value_LifeGrowthType = (sbyte)(Dropdown_LifeSkillGrowthType - 1);
		bool_CombatGrowthType = Dropdown_CombatSkillGrowthType > 0;
		value_CombatGrowthType = (sbyte)(Dropdown_CombatSkillGrowthType - 1);
		bool_LifeQulification = Dropdown_LifeSkillType > 0;
		type_LifeQulification = Dropdown_LifeSkillType - 1;
		value_LifeQulification = Slider_LifeSkillQualification;
		bool_CombatQulification = Dropdown_CombatSkillType > 0;
		type_CombatQulification = Dropdown_CombatSkillType - 1;
		value_CombatQulification = Slider_CombatSkillQualification;
		bool_MainAttribute = Dropdown_MainAttributeType > 0;
		type_MainAttribute = Dropdown_MainAttributeType - 1;
		value_MainAttribute = Slider_MainAttribute;
		rollCountLimit = Slider_RollCountLimit;
	}

	public CustomizedAttributeInfo(bool bool_LifeGrowthType_Value, sbyte value_LifeGrowthType_Value, bool bool_CombatGrowthType_Value, sbyte value_CombatGrowthType_Value, bool bool_LifeQulification_Value, int type_LifeQulification_Value, int value_LifeQulification_Value, bool bool_CombatQulification_Value, int type_CombatQulification_Value, int value_CombatQulification_Value, bool bool_MainAttribute_Value, int type_MainAttribute_Value, int value_MainAttribute_Value, int Slider_RollCountLimit)
	{
		bool_LifeGrowthType = bool_LifeGrowthType_Value;
		value_LifeGrowthType = value_LifeGrowthType_Value;
		bool_CombatGrowthType = bool_CombatGrowthType_Value;
		value_CombatGrowthType = value_CombatGrowthType_Value;
		bool_LifeQulification = bool_LifeQulification_Value;
		type_LifeQulification = type_LifeQulification_Value;
		value_LifeQulification = value_LifeQulification_Value;
		bool_CombatQulification = bool_CombatQulification_Value;
		type_CombatQulification = type_CombatQulification_Value;
		value_CombatQulification = value_CombatQulification_Value;
		bool_MainAttribute = bool_MainAttribute_Value;
		type_MainAttribute = type_MainAttribute_Value;
		value_MainAttribute = value_MainAttribute_Value;
		rollCountLimit = Slider_RollCountLimit;
	}

	public unsafe bool CheckIsPassed(Character charater)
	{
		Traverse val = Traverse.Create((object)charater);
		sbyte lifeSkillQualificationGrowthType = charater.GetLifeSkillQualificationGrowthType();
		sbyte combatSkillQualificationGrowthType = charater.GetCombatSkillQualificationGrowthType();
		val.Field("_lifeSkillQualificationGrowthType").SetValue((object)(sbyte)0);
		val.Field("_combatSkillQualificationGrowthType").SetValue((object)(sbyte)0);
		if (bool_LifeQulification)
		{
			LifeSkillShorts lifeSkillShorts = (LifeSkillShorts)val.Method("CalcLifeSkillQualifications", Array.Empty<object>()).GetValue();
			short num = lifeSkillShorts.Items[type_LifeQulification];
			if (num < value_LifeQulification)
			{
				return false;
			}
		}
		if (bool_CombatQulification)
		{
			CombatSkillShorts combatSkillShorts = (CombatSkillShorts)val.Method("CalcCombatSkillQualifications", Array.Empty<object>()).GetValue();
			short num2 = combatSkillShorts.Items[type_CombatQulification];
			if (num2 < value_CombatQulification)
			{
				return false;
			}
		}
		if (bool_MainAttribute)
		{
			MainAttributes maxMainAttributes = charater.GetMaxMainAttributes();
			short num3 = maxMainAttributes.Items[type_MainAttribute];
			if (num3 < value_MainAttribute)
			{
				return false;
			}
		}
		sbyte b = (bool_LifeGrowthType ? value_LifeGrowthType : lifeSkillQualificationGrowthType);
		sbyte b2 = (bool_CombatGrowthType ? value_CombatGrowthType : lifeSkillQualificationGrowthType);
		val.Field("_lifeSkillQualificationGrowthType").SetValue((object)b);
		val.Field("_combatSkillQualificationGrowthType").SetValue((object)b2);
		return true;
	}
}
