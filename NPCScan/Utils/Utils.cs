using Config;
using System.Text;
using UnityEngine;

namespace NpcScan.Utils
{
    public class CommonUtils
    {
        public static string GetMariageStatu(int marriage)
        {
            if (marriage == 0)
                return "未婚";
            else if (marriage == 1)
                return "已婚";
            else
                return "丧偶";
        }

        public static string GetItemName(int itemType, int itemId)
        {
            switch (itemType)
            {
                case 0:
                    return Weapon.Instance[itemId].Name;
                case 1:
                    return Armor.Instance[itemId].Name;
                case 2:
                    return Accessory.Instance[itemId].Name;
                case 3:
                    return Clothing.Instance[itemId].Name;
                case 4:
                    return Carrier.Instance[itemId].Name;
                case 5:
                    return Config.Material.Instance[itemId].Name;
                case 6:
                    return CraftTool.Instance[itemId].Name;
                case 7:
                    return Food.Instance[itemId].Name;
                case 8:
                    return Medicine.Instance[itemId].Name;
                case 9:
                    return TeaWine.Instance[itemId].Name;
                case 10:
                    return SkillBook.Instance[itemId].Name;
                case 11:
                    return Cricket.Instance[itemId].Name;
                case 12:
                    return Misc.Instance[itemId].Name;
                default:
                    return "None";
            }
        }

        public static string GetQualificationGrowth(int age, int growthType)
        {
            AgeEffectItem ageEffectItem = AgeEffect.Instance[Mathf.Min(age, AgeEffect.Instance.Count - 1)];
            if (ageEffectItem == null)
                return "";
            int num = 0;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Clear();
            switch (growthType)
            {
                case 0:
                    stringBuilder.Append(LocalStringManager.Get("LK_Qualification_Growth_Average"));
                    num = ageEffectItem.SkillQualificationAverage;
                    break;
                case 1:
                    stringBuilder.Append(LocalStringManager.Get("LK_Qualification_Growth_Precocious"));
                    num = ageEffectItem.SkillQualificationPrecocious;
                    break;
                case 2:
                    stringBuilder.Append(LocalStringManager.Get("LK_Qualification_Growth_LateBlooming"));
                    num = ageEffectItem.SkillQualificationLateBlooming;
                    break;
                default:
                    break;
            }
            if (num > 0)
            {
                stringBuilder.Append($"+{num}".SetColor("lightblue"));
            }
            else if (num == 0)
            {
                stringBuilder.Append("+0".SetColor("lightgrey"));
            }
            else
            {
                stringBuilder.Append($"{num}".SetColor("red"));
            }
            return stringBuilder.ToString();
        }

        public static string SetColor(string input, Color color)
        {
            return "<color=" + color.ColorToHexString() + ">" + input + "</color>";
        }
    }
}
