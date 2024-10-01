using System.Collections.Generic;
using Config;
using Config.ConfigCells.Character;
using UnityEngine;

namespace QuicklyCreateCharacterFrontend;

internal static class CharacterDataTool
{
	public delegate string GetColorFunc(string argString);

	public static string[] QualificationGrowthTypeNameArray = new string[3] { "均衡", "早熟", "晚成" };

	public static string[] LifeSkillNameArray = new string[16]
	{
		"音律", "弈棋", "诗书", "绘画", "术数", "品鉴", "锻造", "制木", "医术", "毒术",
		"织锦", "巧匠", "道法", "佛学", "厨艺", "杂学"
	};

	public static string[] CombatSkillNameArray = new string[14]
	{
		"内功", "身法", "绝技", "拳掌", "指法", "腿法", "暗器", "剑法", "刀法", "长兵",
		"奇门", "软兵", "御射", "乐器"
	};

	public static string[] MainAttributeNameArray = new string[6] { "膂力", "灵敏", "定力", "体质", "根骨", "悟性" };

	public static string[] FeatureMedalNameArray = new string[3] { "进攻", "守御", "机略" };

	public static string[] AtkHitAttributeNameArray = new string[4] { "力道", "迅疾", "卸力", "闪避" };

	public static string[] DefHitAttributeNameArray = new string[4] { "精妙", "动心", "拆招", "守心" };

	public static Dictionary<CharacterDataType, List<string>> CharacterDataListToDataDict(List<string> argList)
	{
		Dictionary<CharacterDataType, List<string>> dictionary = new Dictionary<CharacterDataType, List<string>>();
		for (int i = 0; i < argList.Count; i += 2)
		{
			switch (argList[i])
			{
			case "lifeSkillQualificationGrowthType":
				dictionary.Add(CharacterDataType.LifeSkillGrowthType, new List<string> { argList[i + 1] });
				break;
			case "combatSkillQualificationGrowthType":
				dictionary.Add(CharacterDataType.CombatSkillGrowthType, new List<string> { argList[i + 1] });
				break;
			case "baseLifeSkillQualifications":
				dictionary.Add(CharacterDataType.LifeSkillQualification, StringArgToStringList(argList[i + 1]));
				break;
			case "baseCombatSkillQualifications":
				dictionary.Add(CharacterDataType.CombatSkillQualification, StringArgToStringList(argList[i + 1]));
				break;
			case "featureIds":
			{
				List<string> list = DoHideFeatureIdList(StringArgToStringList(argList[i + 1]));
				dictionary.Add(CharacterDataType.FeatureIds, list);
				dictionary.Add(CharacterDataType.FeatureMedalValue, GetMedalsListByFeatureIdList(list));
				break;
			}
			case "mainAttributes":
				dictionary.Add(CharacterDataType.MainAttribute, StringArgToStringList(argList[i + 1]));
				break;
			case "lifeSkillBookName":
				dictionary.Add(CharacterDataType.LifeSkillBookName, new List<string> { argList[i + 1] });
				break;
			case "lifeSkillBookType":
				dictionary.Add(CharacterDataType.LifeSkillBookType, new List<string> { argList[i + 1] });
				break;
			case "combatSkillBookName":
				dictionary.Add(CharacterDataType.CombatSkillBookName, new List<string> { argList[i + 1] });
				break;
			case "combatSkillBookPageTypes":
				dictionary.Add(CharacterDataType.CombatSkillBookPageType, NormalizeCombatSkillBookPageTypeList(StringArgToStringList(argList[i + 1])));
				break;
			case "atkHitAttribute":
				dictionary.Add(CharacterDataType.AtkHitAttribute, StringArgToStringList(argList[i + 1]));
				break;
			case "defHitAttribute":
				dictionary.Add(CharacterDataType.DefHitAttribute, StringArgToStringList(argList[i + 1]));
				break;
			case "atkPenetrability":
				dictionary.Add(CharacterDataType.AtkPenetrability, StringArgToStringList(argList[i + 1]));
				break;
			case "defPenetrability":
				dictionary.Add(CharacterDataType.DefPenetrability, StringArgToStringList(argList[i + 1]));
				break;
			case "secondaryAttribute":
				dictionary.Add(CharacterDataType.SecondaryAttribute, StringArgToStringList(argList[i + 1]));
				break;
			}
		}
		return dictionary;
	}

	public static Dictionary<CharacterDataType, List<short>> CharacterDataDictToShortDataDict(Dictionary<CharacterDataType, List<string>> dataDict)
	{
		Dictionary<CharacterDataType, List<short>> dictionary = new Dictionary<CharacterDataType, List<short>>();
		foreach (CharacterDataType key in dataDict.Keys)
		{
			switch (key)
			{
			case CharacterDataType.LifeSkillGrowthType:
			case CharacterDataType.LifeSkillQualification:
			case CharacterDataType.CombatSkillGrowthType:
			case CharacterDataType.CombatSkillQualification:
			case CharacterDataType.FeatureIds:
			case CharacterDataType.FeatureMedalValue:
			case CharacterDataType.MainAttribute:
			case CharacterDataType.LifeSkillBookType:
			case CharacterDataType.CombatSkillBookPageType:
			case CharacterDataType.AtkHitAttribute:
			case CharacterDataType.DefHitAttribute:
			case CharacterDataType.AtkPenetrability:
			case CharacterDataType.DefPenetrability:
			case CharacterDataType.SecondaryAttribute:
			{
				List<short> list = new List<short>();
				for (int i = 0; i < dataDict[key].Count; i++)
				{
					list.Add((short)int.Parse(dataDict[key][i]));
				}
				dictionary.Add(key, list);
				break;
			}
			}
		}
		return dictionary;
	}

	public static Dictionary<CharacterDataType, List<string>> CharacterDataDictToNameDict(Dictionary<CharacterDataType, List<string>> dataDict)
	{
		Dictionary<CharacterDataType, List<string>> dictionary = new Dictionary<CharacterDataType, List<string>>();
		foreach (CharacterDataType key in dataDict.Keys)
		{
			switch (key)
			{
			case CharacterDataType.LifeSkillGrowthType:
			case CharacterDataType.CombatSkillGrowthType:
			{
				List<string> list2 = new List<string>();
				list2.Add(QualificationGrowthTypeNameArray[int.Parse(dataDict[key][0])]);
				dictionary.Add(key, list2);
				break;
			}
			case CharacterDataType.LifeSkillQualification:
				dictionary.Add(key, new List<string>(LifeSkillNameArray));
				break;
			case CharacterDataType.CombatSkillQualification:
				dictionary.Add(key, new List<string>(CombatSkillNameArray));
				break;
			case CharacterDataType.FeatureIds:
				dictionary.Add(key, GetNameListByFeatureIdList(dataDict[key]));
				break;
			case CharacterDataType.FeatureMedalValue:
				dictionary.Add(key, new List<string>(FeatureMedalNameArray));
				break;
			case CharacterDataType.MainAttribute:
				dictionary.Add(key, new List<string>(MainAttributeNameArray));
				break;
			case CharacterDataType.LifeSkillBookName:
				dictionary.Add(key, dataDict[key]);
				break;
			case CharacterDataType.LifeSkillBookType:
			{
				List<string> list = new List<string>();
				list.Add(LifeSkillNameArray[int.Parse(dataDict[key][0])]);
				dictionary.Add(key, list);
				break;
			}
			case CharacterDataType.CombatSkillBookName:
				dictionary.Add(key, dataDict[key]);
				break;
			case CharacterDataType.CombatSkillBookPageType:
				dictionary.Add(key, GetNameListByCombatSkillBookPageType(dataDict[key]));
				break;
			case CharacterDataType.AtkHitAttribute:
				dictionary.Add(key, new List<string>(AtkHitAttributeNameArray));
				break;
			case CharacterDataType.DefHitAttribute:
				dictionary.Add(key, new List<string>(DefHitAttributeNameArray));
				break;
			}
		}
		return dictionary;
	}

	public static Dictionary<CharacterDataType, List<string>> CharacterDataDictToColorDict(Dictionary<CharacterDataType, List<string>> dataDict)
	{
		Dictionary<CharacterDataType, List<string>> dictionary = new Dictionary<CharacterDataType, List<string>>();
		foreach (CharacterDataType key in dataDict.Keys)
		{
			switch (key)
			{
			case CharacterDataType.LifeSkillGrowthType:
			case CharacterDataType.CombatSkillGrowthType:
				dictionary.Add(key, GetColorList(dataDict[key], GetColorWhite));
				break;
			case CharacterDataType.LifeSkillQualification:
			case CharacterDataType.CombatSkillQualification:
				dictionary.Add(key, GetColorList(dataDict[key], GetColorByQualification));
				break;
			case CharacterDataType.FeatureIds:
				dictionary.Add(key, GetColorList(dataDict[CharacterDataType.FeatureIds], GetColorByFeatureId));
				break;
			case CharacterDataType.FeatureMedalValue:
				dictionary.Add(key, GetColorList(dataDict[key], GetColorByFeatureMedal));
				break;
			case CharacterDataType.MainAttribute:
				dictionary.Add(key, GetColorList(dataDict[key], GetColorWhite));
				break;
			case CharacterDataType.LifeSkillBookName:
				dictionary.Add(key, GetColorList(dataDict[key], GetColorYellow));
				break;
			case CharacterDataType.LifeSkillBookType:
				dictionary.Add(key, GetColorList(dataDict[key], GetColorWhite));
				break;
			case CharacterDataType.CombatSkillBookName:
				dictionary.Add(key, GetColorList(dataDict[key], GetColorYellow));
				break;
			case CharacterDataType.CombatSkillBookPageType:
				dictionary.Add(key, GetColorList(dataDict[key], GetColorByPageType));
				break;
			}
		}
		return dictionary;
	}

	public static List<string> StringArgToStringList(string argString)
	{
		List<string> list = new List<string>();
		string[] array = argString.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			list.Add(array[i]);
		}
		return list;
	}

	public static List<string> DoHideFeatureIdList(List<string> argList)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < argList.Count; i++)
		{
			CharacterFeature instance = CharacterFeature.Instance;
			int id = int.Parse(argList[i]);
			if (!instance[id].Hidden)
			{
				list.Add(argList[i]);
			}
		}
		return list;
	}

	public static List<string> NormalizeCombatSkillBookPageTypeList(List<string> argList)
	{
		List<string> list = new List<string>();
		list.Add((int.Parse(argList[0]) + int.Parse(argList[1]) * 2 + int.Parse(argList[2]) * 4).ToString());
		for (int i = 3; i < argList.Count; i++)
		{
			list.Add(argList[i]);
		}
		return list;
	}

	public static List<string> GetMedalsListByFeatureIdList(List<string> argList)
	{
		List<string> list = new List<string>();
		CharacterFeature instance = CharacterFeature.Instance;
		for (sbyte b = 0; b < 3; b = (sbyte)(b + 1))
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < argList.Count; i++)
			{
				int id = int.Parse(argList[i]);
				FeatureMedals featureMedals = instance[id].FeatureMedals[b];
				List<sbyte> values = featureMedals.Values;
				for (int j = 0; j < values.Count; j++)
				{
					switch (values[j])
					{
					case 0:
						num++;
						break;
					case 1:
						num--;
						break;
					case 2:
						num2++;
						break;
					case 3:
						num2 -= 3;
						break;
					}
				}
			}
			if (num > 0)
			{
				int a = num + num2;
				list.Add(Mathf.Max(0, Mathf.Min(a, 8)).ToString());
			}
			else if (num < 0)
			{
				int a2 = num - num2;
				list.Add(Mathf.Max(-8, Mathf.Min(a2, 0)).ToString());
			}
			else
			{
				list.Add(0.ToString());
			}
		}
		return list;
	}

	public static List<string> GetNameListByFeatureIdList(List<string> argList)
	{
		List<string> list = new List<string>();
		CharacterFeature instance = CharacterFeature.Instance;
		for (int i = 0; i < argList.Count; i++)
		{
			int id = int.Parse(argList[i]);
			CharacterFeatureItem characterFeatureItem = instance[id];
			list.Add(instance[id].Name);
		}
		return list;
	}

	public static List<string> GetNameListByCombatSkillBookPageType(List<string> argList)
	{
		List<string> list = new List<string>();
		if (argList.Count != 6)
		{
			return null;
		}
		switch (int.Parse(argList[0]))
		{
		case 0:
			list.Add("承");
			break;
		case 1:
			list.Add("合");
			break;
		case 2:
			list.Add("解");
			break;
		case 3:
			list.Add("异");
			break;
		case 4:
			list.Add("独");
			break;
		default:
			list.Add("Null");
			break;
		}
		for (int i = 1; i < 6; i++)
		{
			if (int.Parse(argList[i]) == 0)
			{
				list.Add("正");
			}
			else
			{
				list.Add("逆");
			}
		}
		return list;
	}

	public static List<string> GetColorList(List<string> argStringList, GetColorFunc getColorFunc)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < argStringList.Count; i++)
		{
			list.Add(getColorFunc(argStringList[i]));
		}
		return list;
	}

	public static string GetColorWhite(string argString)
	{
		return "#ffffffff";
	}

	public static string GetColorYellow(string argString)
	{
		return "#ffff00ff";
	}

	public static string GetColorByQualification(string argString)
	{
		int num = int.Parse(argString);
		if (num >= 90)
		{
			return "#ff0000ff";
		}
		if (num >= 80)
		{
			return "#ffa500ff";
		}
		if (num >= 70)
		{
			return "#ff00ffff";
		}
		if (num >= 60)
		{
			return "#00ffffff";
		}
		if (num >= 40)
		{
			return "#ffffffff";
		}
		return "#c0c0c0ff";
	}

	public static string GetColorByFeatureId(string argString)
	{
		CharacterFeature instance = CharacterFeature.Instance;
		int id = int.Parse(argString);
		CharacterFeatureItem characterFeatureItem = instance[id];
		return instance[id].CandidateGroupId switch
		{
			-1 => "#ffffffff", 
			0 => "#00ffffff", 
			1 => "#ff0000ff", 
			_ => "#ffffffff", 
		};
	}

	public static string GetColorByFeatureMedal(string argString)
	{
		int num = int.Parse(argString);
		if (num > 0)
		{
			return "#00ffffff";
		}
		if (num < 0)
		{
			return "#ff0000ff";
		}
		return "#c0c0c0ff";
	}

	public static string GetColorByPageType(string argString)
	{
		if (!(argString == "0"))
		{
			if (argString == "1")
			{
				return "#ff0000ff";
			}
			return "#ffffffff";
		}
		return "#00ffffff";
	}
}
