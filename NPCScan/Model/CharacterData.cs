using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcScan
{
    public class CharacterData
    {
        public int id { get; set; }
        public int templateId { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string givenname { get; set; }
        public int[] fullname { get; set; }
        public int monkType { get; set; }
        public int[] monasticTitle { get; set; }
        public int gender { get; set; }
        public bool transgender { get; set; }
        public bool bisexual { get; set; }
        public int age { get; set; }
        public int xiangshuInfection { get; set; }
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
        public int[] lifeSkillQualifications { get; set; }
        public int[] combatSkillQualifications { get; set; }
        public List<int[]> items { get; set; }
        public string[] preexistenceCharacterNames { get; set; }
        public int[] preexistenceCharacterIds { get; set; }
        public List<int> featureIds { get; set; }
        public List<int> potentialFeatureIds { get; set; }
        public int jilue {  get; set; }
        public int isAlive { get; set; }

        public string organization;
        public string identify;
        public List<string> itemList;
        public List<string> featureList;
    }
}
