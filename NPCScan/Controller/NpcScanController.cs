﻿using Config;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character;
using GameData.Domains.Map;
using GameData.Serializer;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaiwuModdingLib.Core.Plugin;
using TMPro;
using UnityEngine;
using NpcScan.Utils;
using FrameWork;
using XLua.Cast;

namespace NpcScan.Controller
{
    public class NpcScanController : MonoBehaviour
    {
        public View view;
        public KeyCode mainFormKey = KeyCode.F3;
        private bool isShow = false;
        private List<CharacterData> characterDataList;

        public void Awake()
        {
            if (view == null)
            {
                view = new View();
                view.Init();
                AddListener();
                HideUI();
                page = 1;
            }           
        }

        public void AddListener()
        {
            view.buttonNext.onClick.AddListener(() => NextPage());
            view.buttonPrevious.onClick.AddListener(() => PreviousPage());
            view.buttonSearch.onClick.AddListener(() => { SetOptions(); SearchResult(0); });
            view.buttonUpdate.onClick.AddListener(() => GetAllCharacters());
            for (int index=0; index<51; ++index)
            {
                int i = index;
                view.ButtonDic[(Model.CharacterInfo)i].onClick.AddListener(()=>SortClick(i));
                if (i < 50)
                    view.resultLabels[i][0].parent.GetComponent<CButton>().onClick.AddListener(()=>OpenCharacterMenu(i));
            }
        }

        public void SortClick(int index)
        {
            if (index >= 12 && index <= 17)
                Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderByDescending(character => character.maxMainAttributes[index - 12]).ToList();
            else if (index >= 18 && index <= 31)
            {
                Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderByDescending(character => character.combatSkillQualifications[index - 18]).ToList();
            }
            else if (index >= 32 && index <= 47)
            {
                Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderByDescending(character => character.lifeSkillQualifications[index - 32]).ToList();
            }
            else
            {
                switch (index)
                {
                    case 0:
                        Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderByDescending(character => character.name).ToList();
                        break;
                    case 1:
                        Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderByDescending(character => character.age).ToList();
                        break;
                    case 2:
                        Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderBy(character => character.gender).ToList();
                        break;
                    case 3:
                        Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderBy(character => character.location[0]).ThenBy(character => character.location[1]).ToList();
                        break;
                    case 4:
                        Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderByDescending(character => character.attraction).ToList();
                        break;
                    case 5:
                        Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderByDescending(character => character.organizationInfo[0]).ToList();
                        break;
                    case 6:
                        Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderByDescending(character => character.organizationInfo[1]).ToList();
                        break;
                    case 7:
                        Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderBy(character => character.behaviorType).ToList();
                        break;
                    case 8:
                        Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderBy(character => character.marriage).ToList();
                        break;
                    case 9:
                        Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderBy(character => character.lifeSkillGrowthType).ToList();
                        break;
                    case 10:
                        Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderBy(character => character.combatSkillGrowthType).ToList();
                        break;
                    case 11:
                        Model.Instance.CurrentDataList = Model.Instance.CurrentDataList.OrderByDescending(character => character.maxLeftHealth).ToList();
                        break;
                    case 49:
                        Model.Instance.CurrentDataList.Sort(ItemCompare);
                        break;
                }
            }
            SearchResult(0);
        }

        public void OpenCharacterMenu(int index)
        {            
            UIManager.Instance.HideUI(UIElement.CharacterMenu);
            int dataIndex = (page - 1) * 50 + index;           
            if (Model.Instance.CurrentDataList.Count > dataIndex)
            {
                CharacterData data = Model.Instance.CurrentDataList[dataIndex];
                if (data.isAlive == 1)
                {
                    HideUI();
                    isShow = false;
                    CommandKitBase.SetDisable(false);
                    GMFunc.EnterCharacterMenu(data.id);
                }    
            }               
        }

        public int ItemCompare(CharacterData character1, CharacterData character2)
        {
            List<int[]> Item1 = character1.items;
            List<int[]> Item2 = character2.items;
            int length = Math.Min(Item1.Count, Item2.Count);
            for (int index=0; index<length; ++index)
            {
                if (Item1[index][2] < Item2[index][2])
                    return 1;
                else if (Item1[index][2] > Item2[index][2])
                    return -1;
            }
            for (int index = 0; index < length; ++index)
            {
                if (Item1[index][1] < Item2[index][1])
                    return 1;
                else if (Item1[index][1] > Item2[index][1])
                    return -1;
            }
            return 0;
        }

        public void Update()
        {
            if (Input.GetKeyUp(mainFormKey))
            {
                if (isShow)
                {
                    HideUI();
                    isShow = false;
                    CommandKitBase.SetDisable(false);
                }
                else
                {
                    ShowUI();
                    isShow = true;
                    CommandKitBase.SetDisable(true);
                }
            }
            else if (Input.GetKey(KeyCode.LeftAlt))
            {
                if (Input.GetKeyUp(KeyCode.PageUp))
                    view.ScrollView.transform.localScale += new Vector3(0.05f, 0.05f, 0);
                else if (Input.GetKeyUp(KeyCode.PageDown))
                    view.ScrollView.transform.localScale -= new Vector3(0.05f, 0.05f, 0);
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyUp(KeyCode.PageUp))
                    view.inputContainer.transform.localScale += new Vector3(0.05f, 0.05f, 0);
                else if (Input.GetKeyUp(KeyCode.PageDown))
                    view.inputContainer.transform.localScale -= new Vector3(0.05f, 0.05f, 0);
            }
            else if (Input.GetKeyUp(KeyCode.PageUp))
            {
                PreviousPage();
            }               
            else if (Input.GetKeyUp(KeyCode.PageDown))
            {
                NextPage();
            }                
            else if (Input.GetKeyUp(KeyCode.Return))
            {
                if (Model.Instance.AllDataList.Count == 0)
                {
                    GetAllCharacters();
                }
                else
                {
                    SetOptions();
                    SearchResult(0);
                }               
            }
        }

        private void GetAllCharacters()
        {
            SingletonObject.getInstance<AsynchMethodDispatcher>().AsynchMethodCall(97, 0, (offset, dataPool) =>
            {              
                string AllCharacterString = File.ReadAllText("NpcScanData.json");

                characterDataList = JsonConvert.DeserializeObject<List<CharacterData>>(AllCharacterString);

                Model.Instance.SetData(characterDataList);
                SetOptions();
                SearchResult(0);
            });
        }

        public void ShowUI()
        {
            view.Show();
        }

        public void HideUI()
        {
            view.Hide();
        }

        private int _page;
        public int page
        {
            get 
            {
                return _page;
            }
            set
            {
                if (value == 0)
                {
                    _page = (Model.Instance.CurrentDataList.Count - 1) / 50 + 1;
                }
                else if (value > (Model.Instance.CurrentDataList.Count - 1) / 50 + 1)
                {
                    _page = 1;
                }
                else
                    _page = value;
            }
        }

        public void SearchResult(int direction)
        {
            if (direction > 0)
                page += 1;
            if (direction < 0)
                page -= 1;
            if (direction == 0)
            {
                page = 1;               
            }
                
            view.pageCount.text = page + "/" + ((Model.Instance.CurrentDataList.Count-1) / 50 + 1);
            int startIndex = (page - 1) * 50;
            int length = Math.Min(50, Model.Instance.CurrentDataList.Count - startIndex);
            WorldMapModel instance = SingletonObject.getInstance<WorldMapModel>();

            for (int row = 0; row < 50; ++row)
            {
                List<Transform> rowLabels = view.resultLabels[row];
                for (int col = 0; col <51; ++col)
                {
                    rowLabels[col].GetComponent<TextMeshProUGUI>().text = "";
                }
            }
            for (int index = 0; index < length; ++index)
            {
                CharacterData characterData = Model.Instance.CurrentDataList[startIndex + index];

                List<Transform> rowLabels = view.resultLabels[index];             

                rowLabels[0].GetComponent<TextMeshProUGUI>().text = characterData.name;
                rowLabels[1].GetComponent<TextMeshProUGUI>().text = characterData.age.ToString();
                rowLabels[2].GetComponent<TextMeshProUGUI>().text = CommonUtils.GetGenderString((sbyte)characterData.gender);

                Location location = new Location((short)characterData.location[0], (short)characterData.location[1]);
                if (location.AreaId == -1 || location.BlockId == -1)
                {
                    rowLabels[3].GetComponent<TextMeshProUGUI>().text = $"无法到达地区";
                }
                else
                {
                    string state = MapState.Instance[instance.Areas[location.AreaId].GetConfig().StateID].Name;
                    string area = instance.Areas[location.AreaId].GetConfig().Name;
                    try
                    {
                        MapBlockData blockData = instance.GetBlockData(location);
                        if (blockData != null)
                        {
                            string block = instance.GetBlockName(location.AreaId, blockData.BlockId, blockData.TemplateId);
                            ByteCoordinate blockPos = blockData.GetBlockPos();
                            rowLabels[3].GetComponent<TextMeshProUGUI>().text = $"{state} {area} {block} ({blockPos.X},{blockPos.Y})";
                        }
                        else
                        {
                            rowLabels[3].GetComponent<TextMeshProUGUI>().text = $"{state} {area}";
                        }
                    }
                    catch
                    {
                        rowLabels[3].GetComponent<TextMeshProUGUI>().text = $"非法位置";
                    }
                }

                string charm = CommonUtils.GetCharmLevelText((short)characterData.attraction, (sbyte)characterData.gender, 16, 1, CreatingType.IsFixedPresetType((byte)characterData.creatingType));
                rowLabels[4].GetComponent<TextMeshProUGUI>().text = $"{charm}({characterData.attraction})";

                OrganizationInfo organizationInfo = new OrganizationInfo((sbyte)characterData.organizationInfo[0], (sbyte)characterData.organizationInfo[1], Convert.ToBoolean(characterData.organizationInfo[2]), (short)characterData.organizationInfo[3]);
                rowLabels[5].GetComponent<TextMeshProUGUI>().text = characterData.organization;

                rowLabels[6].GetComponent<TextMeshProUGUI>().text = characterData.identify;

                rowLabels[7].GetComponent<TextMeshProUGUI>().text = CommonUtils.GetBehaviorString((sbyte)characterData.behaviorType);
                rowLabels[8].GetComponent<TextMeshProUGUI>().text = Utils.CommonUtils.GetMariageStatu(characterData.marriage);
                rowLabels[9].GetComponent<TextMeshProUGUI>().text = Utils.CommonUtils.GetQualificationGrowth(characterData.age, characterData.lifeSkillGrowthType);
                rowLabels[10].GetComponent<TextMeshProUGUI>().text = Utils.CommonUtils.GetQualificationGrowth(characterData.age, characterData.combatSkillGrowthType);
                rowLabels[11].GetComponent<TextMeshProUGUI>().text = $"{characterData.health}/{characterData.maxLeftHealth}";

                for (int attributeIndex = 0; attributeIndex < 6; ++attributeIndex)
                    rowLabels[12 + attributeIndex].GetComponent<TextMeshProUGUI>().text = characterData.maxMainAttributes[attributeIndex].ToString();

                for (int combatSkillIndex = 0; combatSkillIndex < 14; ++combatSkillIndex)
                {
                    var text = rowLabels[18 + combatSkillIndex].GetComponent<TextMeshProUGUI>();
                    text.text = characterData.combatSkillQualifications[combatSkillIndex].ToString();
                    text.color = CommonUtils.GetCharacterSkillColorByValue((short)characterData.combatSkillQualifications[combatSkillIndex]);
                }

                for (int lifeSkillIndex = 0; lifeSkillIndex < 16; ++lifeSkillIndex)
                {
                    var text = rowLabels[32 + lifeSkillIndex].GetComponent<TextMeshProUGUI>();
                    text.text = characterData.lifeSkillQualifications[lifeSkillIndex].ToString();
                    text.color = CommonUtils.GetCharacterSkillColorByValue((short)characterData.lifeSkillQualifications[lifeSkillIndex]);
                }

                rowLabels[48].GetComponent<TextMeshProUGUI>().text = string.Join(",", characterData.preexistenceCharacterNames);

                rowLabels[49].GetComponent<TextMeshProUGUI>().text = string.Join(",", characterData.itemList);

                rowLabels[50].GetComponent<TextMeshProUGUI>().text = string.Join(",", characterData.featureList);
            }
        }

        public void SetOptions()
        {
            GetIntInput();
            GetStringInput();
            GetBoolInput();
            Model.Instance.CurrentDataList = Model.Instance.AllDataList.FindAll(character => CheckSearchOption(character));
        }

        public void NextPage()
        {
            SearchResult(1);
        }

        public void PreviousPage()
        {
            SearchResult(-1);
        }

        private List<int> intInputValue = new List<int>();
        private List<string> stringInputValue = new List<string>();
        private List<bool> boolInputValue = new List<bool>();

        public bool CheckSearchOption(CharacterData character)
        {
            if (CheckInt(character) && CheckString(character) && CheckBool(character))
                return true;
            else
                return false;
        }

        public bool CheckInt(CharacterData character)
        {
            int minAge = intInputValue[0];
            if (minAge != 0)
            {
                if (character.age < minAge)
                    return false;
            }
            int maxAge = intInputValue[1];
            if (maxAge != 0)
            {
                if (character.age > maxAge)
                    return false;
            }
            for (int index = 2, attributeIndex = 0; index < 8; ++index, ++attributeIndex)
            {
                int value = intInputValue[index];
                if (value != 0)
                {
                    if (character.maxMainAttributes[attributeIndex] < value)
                        return false;
                }
            }
            int charm = intInputValue[8];
            if (charm != 0)
            {
                if (character.attraction < charm)
                    return false;
            }
            int health = intInputValue[9];
            if (health != 0)
            {
                if (character.maxLeftHealth < health)
                    return false;
            }
            int samsara = intInputValue[10];
            if (samsara != 0)
            {
                if (character.preexistenceCharacterNames.Length < samsara)
                    return false;
            }
            for (int index = 11, combatSkillIndex = 0; index < 25; ++index, ++combatSkillIndex)
            {
                int value = intInputValue[index];
                if (value != 0)
                {
                    if (character.combatSkillQualifications[combatSkillIndex] < value)
                        return false;
                }
            }
            for (int index = 25, lifeSkillIndex = 0; index < 41; ++index, ++lifeSkillIndex)
            {
                int value = intInputValue[index];
                if (value != 0)
                {
                    if (character.lifeSkillQualifications[lifeSkillIndex] < value)
                        return false;
                }
            }
            return true;
        }

        public bool CheckString(CharacterData character)
        {
            string input = stringInputValue[0];
            if (!input.IsNullOrEmpty())
            {
                if (!character.id.ToString().Equals(input))
                    return false;
            }

            input = stringInputValue[1];
            if (!input.IsNullOrEmpty())
            {
                if (!character.name.Contains(input) && !character.preexistenceCharacterNames.Any(name => name.Contains(input)))
                    return false;
            }

            input = stringInputValue[2];
            if (!input.IsNullOrEmpty())
            {
                if (!character.organization.Equals(input))
                    return false;
            }
            input = stringInputValue[3];
            if (!input.IsNullOrEmpty())
            {
                if (character.identify.Equals(input))
                    return false;
            }
            input = stringInputValue[4];
            if (!input.IsNullOrEmpty())
            {
                if (input.Contains("&") && input.Contains("|"))
                    ;
                else if (input.Contains("&"))
                {
                    var list = input.Split('&');

                    if (list.Any(feature => character.featureList.All(value => !value.Contains(feature))))
                        return false;
                }
                else if (input.Contains("|"))
                {
                    var list = input.Split('|');
                    if (list.All(feature => character.featureList.All(value => !value.Contains(feature))))
                        return false;
                }
                else
                {
                    if (character.featureList.All(value => !value.Contains(input)))
                        return false;
                }
            }
            input = stringInputValue[5];
            if (!input.IsNullOrEmpty())
            {
                if (input.Contains("&") && input.Contains("|"))
                    ;
                else if (input.Contains("&"))
                {
                    var list = input.Split('&');
                    if (list.Any(item => character.itemList.All(value => !value.Contains(item))))
                        return false;
                }
                else if (input.Contains("|"))
                {
                    var list = input.Split('|');
                    if (list.All(item => character.itemList.All(value => !value.Contains(item))))
                        return false;
                }
                else
                {
                    if (character.itemList.All(value => !value.Contains(input)))
                        return false;
                }
            }
            return true;
        }

        public bool CheckBool(CharacterData character)
        {
            List<int> conditions = new List<int>();
            if (boolInputValue[0])
            {
                conditions.Add(0);
                conditions.Add(1);
            }
            else
            {
                if (boolInputValue[1])
                    conditions.Add(1);
                if (boolInputValue[2])
                    conditions.Add(0);
            }
            if (!conditions.Contains(character.gender))
                return false;

            conditions.Clear();
            if (boolInputValue[3])
            {
                conditions.Add(0);
                conditions.Add(1);
                conditions.Add(2);
                conditions.Add(3);
                conditions.Add(4);
            }
            else
            {
                if (boolInputValue[4])
                    conditions.Add(0);
                if (boolInputValue[5])
                    conditions.Add(1);
                if (boolInputValue[6])
                    conditions.Add(2);
                if (boolInputValue[7])
                    conditions.Add(3);
                if (boolInputValue[8])
                    conditions.Add(4);
            }
            if (!conditions.Contains(character.behaviorType))
                return false;

            conditions.Clear();
            if (boolInputValue[9])
            {
                conditions.Add(0);
                conditions.Add(1);
                conditions.Add(2);
            }
            else
            {
                if (boolInputValue[10])
                    conditions.Add(0);
                if (boolInputValue[11])
                    conditions.Add(1);
                if (boolInputValue[12])
                    conditions.Add(2);
            }
            if (!conditions.Contains(character.marriage))
                return false;
            if (character.isAlive == 0)
                return false;

            return true;
        }

        public void GetIntInput()
        {
            intInputValue.Clear();
            for (int index=0; index<41; ++index)
            {
                int value = 0;
                int.TryParse(view.InputDic[(Model.Input)index].text, out value);
                intInputValue.Add(value);
            }
                
        }

        public void GetStringInput()
        {
            stringInputValue.Clear();
            for (int index = 41; index < 47; ++index)
            {
                stringInputValue.Add(view.InputDic[(Model.Input)index].text);
            }
        }

        public void GetBoolInput()
        {
            boolInputValue.Clear();
            for (int index = (int)Model.Toggle.全部性别; index <= (int)Model.Toggle.精确特性; ++index)
            {
                if (index == (int)Model.Toggle.男生女相 || index == (int)Model.Toggle.女生男相 || index == (int)Model.Toggle.仅搜死者 || index == (int)Model.Toggle.精确特性 || index == (int)Model.Toggle.仅搜门派)
                    continue;
                boolInputValue.Add(view.ToggleDic[(Model.Toggle)index].isOn);
            }
        }
    }
}