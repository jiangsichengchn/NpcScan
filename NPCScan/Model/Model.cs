using Config;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NpcScan
{
	public class Model
	{
		private static Model instance;

		public static Model Instance
		{
			get
			{
				if (instance == null)
					instance = new Model();
				return instance;
			}
		}

		public List<CharacterData> AllDataList = new List<CharacterData>();
		public List<CharacterData> CurrentDataList = new List<CharacterData>();

		public void SetData(List<CharacterData> dataList)
		{
			if (dataList == null || dataList.Count == 0)
				return;
			AllDataList = dataList;
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			foreach (CharacterData data in dataList)
			{
				OrganizationInfo organizationInfo = new OrganizationInfo((sbyte)data.organizationInfo[0], (sbyte)data.organizationInfo[1], Convert.ToBoolean(data.organizationInfo[2]), (short)data.organizationInfo[3]);
				short randomNameId = (short)(worldMapModel.SettlementRandNameDict.ContainsKey(organizationInfo.SettlementId) ? worldMapModel.SettlementRandNameDict[organizationInfo.SettlementId] : -1);
				data.organization = CommonUtils.GetOrganizationString(organizationInfo.OrgTemplateId, randomNameId);

				data.identify = CommonUtils.GetCharacterGradeString(organizationInfo, (sbyte)data.gender);

				data.name = data.surname + data.givenname;
				string displayName = GetDisplayName(data);
				if (displayName != null)
				{
					data.name += $"({data.id})\n({displayName})";
				}
				else
				{
					data.name += $"({data.id})";
				}

				data.itemList = new List<string>();
				foreach (int[] item in data.items)
				{
					int itemType = item[0];
					int itemId = item[1];
					int itemGrade = item[2];
					string name = Utils.CommonUtils.GetItemName(itemType, itemId);
					data.itemList.Add("<color=" + Colors.Instance.GradeColors[itemGrade].ColorToHexString() + ">" + name + "</color>");
				}

				data.featureList = new List<string>();
				List<CharacterFeatureItem> featureItems = data.featureIds.Select(f => CharacterFeature.Instance[f]).ToList();
				List<sbyte> medals = featureItems.SelectMany(x => x.FeatureMedals[2].Values).ToList();
				int countBai = medals.FindAll(x => x == 2).Count;
				int countLan = medals.FindAll(x => x == 0).Count;
				int countHong = medals.FindAll(x => x == 1).Count;
				data.jilue = countBai + Math.Abs(countLan - countHong);
				featureItems.ForEach(feature => data.featureList.Add(feature.Name));
				data.potentialFeatureIds.RemoveAll(feature => data.featureIds.Contains(feature));
				data.potentialFeatureIds.ForEach(feature => data.featureList.Add(CharacterFeature.Instance[feature].Name));

				for (int index = 0; index < data.preexistenceCharacterNames.Length; ++index)
				{
					string name = data.preexistenceCharacterNames[index];
					int deadId = data.preexistenceCharacterIds[index];
					CharacterData deadData = AllDataList.Find(c => c.id == deadId);
					if (deadData != null)
					{
						displayName = GetDisplayName(deadData);
						if (displayName != null)
						{
							name += $"({displayName})";
							data.preexistenceCharacterNames[index] = name;
						}
					}
				}
			}
		}

		private string GetDisplayName(CharacterData data)
		{
			NameRelatedData nameRelatedData = new NameRelatedData();
			nameRelatedData.CharTemplateId = (short)data.templateId;
			nameRelatedData.Gender = (sbyte)data.gender;
			nameRelatedData.MonkType = (byte)data.monkType;

			int[] full = data.fullname;
			FullName fullName = new FullName();
			fullName.Type = (sbyte)full[0];
			fullName.CustomSurnameId = full[1];
			fullName.CustomGivenNameId = full[2];
			fullName.SurnameId = (short)full[3];
			fullName.GivenNameGroupId = (short)full[4];
			fullName.GivenNameSuffixId = (short)full[5];
			fullName.GivenNameType = (sbyte)full[6];
			fullName.ZangPrefixId = (sbyte)full[7];
			fullName.ZangSuffixId = (sbyte)full[8];

			nameRelatedData.FullName = fullName;
			nameRelatedData.OrgTemplateId = (sbyte)data.organizationInfo[0];
			nameRelatedData.OrgGrade = (sbyte)data.organizationInfo[1];
			nameRelatedData.MonasticTitle = new MonasticTitle((short)data.monasticTitle[0], (short)data.monasticTitle[1]);

			string monasticTitle;
			try
			{
				monasticTitle = NameCenter.GetMonasticTitle(ref nameRelatedData, false);
			}
			catch
			{
				monasticTitle = null;
			}

			short id = Organization.Instance[nameRelatedData.OrgTemplateId].Members[nameRelatedData.OrgGrade];
			short surnameId = OrganizationMember.Instance[id].SurnameId;

			if (monasticTitle != null)
			{
				return monasticTitle;
			}
			else
			{
				if (surnameId >= 0)
				{
					return LocalSurnames.Instance.SurnameCore[surnameId].Surname + data.givenname;
				}
			}

			return null;
		}

		public CharacterData GetCharacterData(int id)
		{
			return AllDataList.Find(data => data.id == id);
		}

		public enum Input
		{
			最低年龄,
			最大年龄,
			入魔值下限,
			入魔值上限,
			膂力,
			灵敏,
			定力,
			体质,
			根骨,
			悟性,
			魅力,
			健康,
			轮回次数,
			内功,
			身法,
			绝技,
			拳掌,
			指法,
			腿法,
			暗器,
			剑法,
			刀法,
			长兵,
			奇门,
			软兵,
			御射,
			乐器,
			音律,
			弈棋,
			诗书,
			绘画,
			术数,
			品鉴,
			锻造,
			制木,
			医术,
			毒术,
			织锦,
			巧匠,
			道法,
			佛学,
			厨艺,
			杂学,
			角色ID,
			姓名,
			从属,
			身份,
			特性,
			物品
		}
		public enum Toggle
		{
			全部性别,
			男性,
			女性,
			男生女相,
			女生男相,
			全部立场,
			刚正,
			仁善,
			中庸,
			叛逆,
			唯我,
			全部婚姻,
			未婚,
			已婚,
			丧偶,
			仅搜死者,
			仅搜门派,
			精确特性
		}

		public enum CharacterInfo
		{
			姓名,
			年龄,
			性别,
			位置,
			魅力,
			从属,
			身份,
			立场,
			婚姻,
			技艺成长,
			功法成长,
			健康,
			膂力,
			灵敏,
			定力,
			体质,
			根骨,
			悟性,
			机略,
			内功,
			身法,
			绝技,
			拳掌,
			指法,
			腿法,
			暗器,
			剑法,
			刀法,
			长兵,
			奇门,
			软兵,
			御射,
			乐器,
			音律,
			弈棋,
			诗书,
			绘画,
			术数,
			品鉴,
			锻造,
			制木,
			医术,
			毒术,
			织锦,
			巧匠,
			道法,
			佛学,
			厨艺,
			杂学,
			前世,
			物品,
			人物特性,
			查找,
			更新数据,
			上一页,
			下一页
		}
	}
}
