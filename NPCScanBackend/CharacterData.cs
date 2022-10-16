using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Character.Relation;
using GameData.Domains.Item;
using GameData.Domains.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using Character = GameData.Domains.Character.Character;

namespace NpcScan
{
    internal unsafe class CharacterData
    {
        public int id { get; set; }
        public int templateId { get; set; }
        public string surname { get; set; }
        public string givenname { get; set; }
        public int[] fullname { get; set; }
        public int monkType { get; set; }
        public int[] monasticTitle { get; set; }
        public int gender { get; set; }
        public bool transgender { get; set; }
        public bool bisexual { get; set; }
        public int age { get; set; }
        public int[] location { get; set; }
        public int attraction { get; set; }
        public int creatingType { get; set; }
        public int[] organizationInfo { get; set; }
        public int behaviorType { get; set; }
        public int marriage { get; set; }
        public int lifeSkillGrowthType { get; set; }
        public int combatSkillGrowthType { get; set; }
        public int health { get; set; }
        public int maxLeftHealth { get; set; }
        public int[] maxMainAttributes { get; set; }       
        //public LifeSkillShorts baseLifeSkillQualifications { get; set; }
        //public CombatSkillShorts baseCombatSkillQualifications { get; set; }
        public int[] lifeSkillQualifications { get; set; }
        public int[] combatSkillQualifications { get; set; }
        public List<int[]> items { get; set; }       
        public string[] preexistenceCharacterNames { get; set; } 
        public int[] preexistenceCharacterIds { get; set; } 
        public List<int> featureIds { get; set; }
        public List<int> potentialFeatureIds { get; set; }
        public int isAlive { get; set; }
        //public List<LifeSkillItem> learnedLifeSkills { get; set; }
        //public List<int> learnedCombatSkills { get; set; }

        public void SetData(Character character)
        {
            isAlive = 1;
            id = character.GetId();
            templateId = character.GetTemplateId();

            FullName full = character.GetFullName();
            fullname = new int[9] { full.Type, full.CustomSurnameId, full.CustomGivenNameId, full.SurnameId, full.GivenNameGroupId, full.GivenNameSuffixId, full.GivenNameType, full.ZangPrefixId, full.ZangSuffixId };
            (surname, givenname) = DomainManager.Character.GetRealName(id);

            age = character.GetActualAge();
            gender = character.GetGender();
            monkType = character.GetMonkType();

            MonasticTitle monastic = character.GetMonasticTitle();
            monasticTitle = new int[2] { monastic.SeniorityId, monastic.SuffixId };

            Location tempLocation = character.GetLocation();

            location = new int[2] { tempLocation.AreaId, tempLocation.BlockId };

            attraction = character.GetAttraction();
            creatingType = character.GetCreatingType();

            OrganizationInfo tempOrganizationInfo = character.GetOrganizationInfo();
            organizationInfo = new int[4] { tempOrganizationInfo.OrgTemplateId, tempOrganizationInfo.Grade, Convert.ToInt32(tempOrganizationInfo.Principal), tempOrganizationInfo.SettlementId };

            behaviorType = character.GetBehaviorType();

            var mates = DomainManager.Character.GetRelatedCharIds(id, RelationType.HusbandOrWife);
            if (mates.Count > 0)
            {
                foreach(int mateId in mates)
                {
                    if (DomainManager.Character.IsCharacterAlive(mateId))
                    {
                        marriage = 1;
                        break;
                    }
                    marriage = 2;
                }
            }
            else
                marriage = 0;

            lifeSkillGrowthType = character.GetLifeSkillQualificationGrowthType();
            combatSkillGrowthType = character.GetCombatSkillQualificationGrowthType();
            health = character.GetHealth();
            maxLeftHealth = character.GetLeftMaxHealth(character.IsCompletelyInfected());

            MainAttributes tempMaxMainAttributes = character.GetMaxMainAttributes();
            maxMainAttributes = new int[6];
            for (int index = 0; index < 6; index++)
            {
                maxMainAttributes[index] = tempMaxMainAttributes.Items[index];
            }              

            LifeSkillShorts tempLifeSkillQualifications = character.GetLifeSkillQualifications();
            lifeSkillQualifications = new int[16];
            for (int index = 0; index < 16; index++)
                lifeSkillQualifications[index] = tempLifeSkillQualifications.Items[index];

            CombatSkillShorts tempCombatSkillQualifications = character.GetCombatSkillQualifications();
            combatSkillQualifications = new int[14];
            for (int index = 0; index < 14; index++)
                combatSkillQualifications[index] = tempCombatSkillQualifications.Items[index];

            var inventorys = character.GetInventory();
            var equipments = character.GetEquipment();
            int size = inventorys.Items.Count + equipments.Length;
            items = new List<int[]>(size);
            List<ItemBase> itemList = new List<ItemBase>();
            foreach (var inventory in inventorys.Items.Keys)
            {
                if (inventory.ItemType == -1)
                    continue;
                itemList.Add(DomainManager.Item.GetBaseItem(inventory));
            }
            foreach (var equipment in equipments)
            {
                if (equipment.ItemType == -1)
                    continue;
                itemList.Add(DomainManager.Item.GetBaseItem(equipment));
            }
            itemList = itemList.OrderByDescending(item => item.GetGrade()).ToList();
            itemList.ForEach(item => items.Add(new int[3] { item.GetItemType(), item.GetTemplateId(), item.GetGrade() }));

            PreexistenceCharIds tempPreexistenceCharIds = character.GetPreexistenceCharIds();
            preexistenceCharacterNames = new string[tempPreexistenceCharIds.Count];
            preexistenceCharacterIds = new int[tempPreexistenceCharIds.Count];
            for (int index = 0; index < tempPreexistenceCharIds.Count; index++)
            {
                preexistenceCharacterIds[index] = tempPreexistenceCharIds.CharIds[index];
                var (tempSurname, tempGivenname) = DomainManager.Character.GetRealName(tempPreexistenceCharIds.CharIds[index]);
                preexistenceCharacterNames[index] = tempSurname + tempGivenname;
            }

            featureIds = character.GetFeatureIds().ConvertAll(id => (int) id);
            potentialFeatureIds = character.GetPotentialFeatureIds().ConvertAll(id => (int) id);
        }

        public void SetDeadData(int id, DeadCharacter character, Location location)
        {
            isAlive = 0;
            this.id = id;
            templateId = character.TemplateId;

            FullName full = character.FullName;
            fullname = new int[9] { full.Type, full.CustomSurnameId, full.CustomGivenNameId, full.SurnameId, full.GivenNameGroupId, full.GivenNameSuffixId, full.GivenNameType, full.ZangPrefixId, full.ZangSuffixId };
            (surname, givenname) = DomainManager.Character.GetRealName(id);

            age = character.GetActualAge();
            gender = character.Gender;
            monkType = character.MonkType;

            MonasticTitle monastic = character.MonasticTitle;
            monasticTitle = new int[2] { monastic.SeniorityId, monastic.SuffixId };
            this.location = new int[2] { location.AreaId, location.BlockId };
            attraction = character.Attraction;
            creatingType = 1;

            OrganizationInfo tempOrganizationInfo = character.OrganizationInfo;
            organizationInfo = new int[4] { tempOrganizationInfo.OrgTemplateId, tempOrganizationInfo.Grade, Convert.ToInt32(tempOrganizationInfo.Principal), tempOrganizationInfo.SettlementId };

            behaviorType = GameData.Domains.Character.BehaviorType.GetBehaviorType(character.Morality);

            var mates = DomainManager.Character.GetRelatedCharIds(id, RelationType.HusbandOrWife);
            if (mates.Count > 0)
            {
                foreach (int mateId in mates)
                {
                    if (DomainManager.Character.IsCharacterAlive(mateId))
                    {
                        marriage = 1;
                        break;
                    }
                    marriage = 2;
                }
            }
            else
                marriage = 0;

            lifeSkillGrowthType = 0;
            combatSkillGrowthType = 0;
            health = 0;
            maxLeftHealth = 0;

            MainAttributes tempMaxMainAttributes = character.BaseMainAttributes;
            maxMainAttributes = new int[6];
            for (int index = 0; index < 6; index++)
            {
                maxMainAttributes[index] = tempMaxMainAttributes.Items[index];
            }

            LifeSkillShorts tempLifeSkillQualifications = character.BaseLifeSkillQualifications;
            lifeSkillQualifications = new int[16];
            for (int index = 0; index < 16; index++)
                lifeSkillQualifications[index] = tempLifeSkillQualifications.Items[index];

            CombatSkillShorts tempCombatSkillQualifications = character.BaseCombatSkillQualifications;
            combatSkillQualifications = new int[14];
            for (int index = 0; index < 14; index++)
                combatSkillQualifications[index] = tempCombatSkillQualifications.Items[index];

            items = new List<int[]>();           

            PreexistenceCharIds tempPreexistenceCharIds = character.PreexistenceCharIds;
            preexistenceCharacterNames = new string[tempPreexistenceCharIds.Count];
            preexistenceCharacterIds = new int[tempPreexistenceCharIds.Count];
            for (int index = 0; index < tempPreexistenceCharIds.Count; index++)
            {
                preexistenceCharacterIds[index] = tempPreexistenceCharIds.CharIds[index];
                var (tempSurname, tempGivenname) = DomainManager.Character.GetRealName(tempPreexistenceCharIds.CharIds[index]);
                preexistenceCharacterNames[index] = tempSurname + tempGivenname;
            }

            featureIds = character.FeatureIds.ConvertAll(id => (int)id);
            potentialFeatureIds = new List<int>();
        }
    }  
}
