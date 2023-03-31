using FrameWork.ModSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace NpcScan
{
    public class View
    {
        public bool isShow = false;
        public Dictionary<Model.Input, InputField> InputDic;
        public Dictionary<Model.Toggle, CToggle> ToggleDic;
        public Dictionary<Model.CharacterInfo, CButton> ButtonDic;

        public Transform scrollContent;
        public List<List<Transform>> resultLabels;
        public CButton buttonNext;
        public CButton buttonPrevious;
        public CButton buttonSearch;
        public CButton buttonUpdate;
        public TextMeshProUGUI pageCount;
        public VerticalLayoutGroup inputContainer;
        public GameObject ScrollView;

        private int index = 0;

        private Camera mainCamera;
        private GameObject root;
        //private Transform tooltip;
        private CanvasGroup blockClickGroup;
        public void Init()
        {
            InputDic = new Dictionary<Model.Input, InputField>();
            ToggleDic = new Dictionary<Model.Toggle, CToggle>();
            ButtonDic = new Dictionary<Model.CharacterInfo, CButton>();
            mainCamera = Camera.main;
            Canvas canvas = mainCamera.transform.Find("Canvas").GetComponent<Canvas>();
            blockClickGroup = canvas.gameObject.AddComponent<CanvasGroup>();

            root = new GameObject("NPCScan");
            root.transform.SetParent(mainCamera.transform);
            root.layer = LayerMask.NameToLayer("UI");
            Canvas rootCanvas = root.AddComponent<Canvas>();
            rootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            rootCanvas.worldCamera = mainCamera;
            rootCanvas.overrideSorting = true;
            rootCanvas.sortingLayerName = "UI";
            rootCanvas.sortingOrder = 1001;

            root.AddComponent<GraphicRaycaster>();

            var mask = GameObjectCreationUtils.InstantiateUIElement(rootCanvas.transform, "Image");
            mask.GetComponent<CImage>().color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
            mask.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            mask.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            mask.name = "Mask";

            inputContainer = new GameObject("InputContainer").AddComponent<VerticalLayoutGroup>();
            inputContainer.spacing = 30;
            SetTransform(root.transform, inputContainer.transform);
            inputContainer.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            inputContainer.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.95f);
            inputContainer.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.95f);
            inputContainer.transform.localScale = new Vector3(0.6f, 0.6f, 1);

            var text = GameObjectCreationUtils.InstantiateUIElement(inputContainer.transform, "Text");
            text.GetComponent<TextMeshProUGUI>().text = "Npc查找器";
            text.transform.name = "title";
            SetTransform(text.transform);

            //InitTooltip();
            InitFilterContainer(inputContainer);          
            InitScrollView();            

            LayoutRebuilder.ForceRebuildLayoutImmediate(inputContainer.GetComponent<RectTransform>());
        }

        public void Destroy()
        {
            Object.Destroy(root);
        }

        //private void InitTooltip()
        //{
        //    var canva = new GameObject("test");
        //    canva.transform.SetParent(mainCamera.transform);
        //    canva.layer = LayerMask.NameToLayer("UI");
        //    Canvas canvas = canva.AddComponent<Canvas>();
        //    canvas.renderMode = RenderMode.ScreenSpaceCamera;
        //    canvas.worldCamera = mainCamera;
        //    canvas.overrideSorting = true;
        //    canvas.sortingLayerName = "UI";
        //    canvas.sortingOrder = 1002;

        //    tooltip = new GameObject("Tooltip").AddComponent<ContentSizeFitter>().transform;
        //    ResetTransform(canva.transform, tooltip);

        //    tooltip.gameObject.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        //    tooltip.gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        //    tooltip.gameObject.AddComponent<VerticalLayoutGroup>();

        //    var image = tooltip.gameObject.AddComponent<Image>();

        //    CreateLabel(tooltip.transform, "");

        //    tooltip.gameObject.SetActive(false);
        //}

        #region InitFilter
        private void InitFilterContainer(VerticalLayoutGroup container)
        {
            var filterContainer = new GameObject("FilterContainer").AddComponent<VerticalLayoutGroup>();
            filterContainer.childForceExpandHeight = false;
            filterContainer.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            filterContainer.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            SetTransform(container.transform, filterContainer.transform);           

            CreateBasicInfo(filterContainer);
            CreateCombatSkillInfo(filterContainer);
            CreateLifeSkillInfo(filterContainer);
            CreateToggleGroup(filterContainer);
            CreateMultiSearch(filterContainer);
        }
        
        private void CreateBasicInfo(VerticalLayoutGroup parent)
        {
            var basicInfo = new GameObject("BasicInfo").AddComponent<HorizontalLayoutGroup>();
            SetTransform(parent.transform, basicInfo.transform);
            basicInfo.transform.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            basicInfo.childForceExpandWidth = false;

            CreateLabel(basicInfo.transform, "年龄:");
            CreateInputField(basicInfo.transform, Model.Input.最低年龄);
            CreateLabel(basicInfo.transform, "-");
            CreateInputField(basicInfo.transform, Model.Input.最大年龄);

            CreateLabel(basicInfo.transform, "入魔值:");
            CreateInputField(basicInfo.transform, Model.Input.入魔值下限);
            CreateLabel(basicInfo.transform, "-");
            CreateInputField(basicInfo.transform, Model.Input.入魔值上限);

            for (index=(int)Model.Input.膂力; index<= (int)Model.Input.轮回次数; ++index)
            {
                CreateLabel(basicInfo.transform, ((Model.Input) index).ToString() + ":");
                CreateInputField(basicInfo.transform, (Model.Input) index);
            }
        }

        private void CreateCombatSkillInfo(VerticalLayoutGroup parent)
        {
            var combatSkillInfo = new GameObject("CombatSkillInfo").AddComponent<HorizontalLayoutGroup>();
            SetTransform(parent.transform, combatSkillInfo.transform);
            combatSkillInfo.transform.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            combatSkillInfo.childForceExpandWidth = false;

            for (index = (int)Model.Input.内功; index <= (int)Model.Input.乐器; ++index)
            {
                CreateLabel(combatSkillInfo.transform, ((Model.Input)index).ToString() + ":");
                CreateInputField(combatSkillInfo.transform, (Model.Input)index);
            }
        }

        private void CreateLifeSkillInfo(VerticalLayoutGroup parent)
        {
            var lifrSkillInfo = new GameObject("LifrSkillInfo").AddComponent<HorizontalLayoutGroup>();
            SetTransform(parent.transform, lifrSkillInfo.transform);
            lifrSkillInfo.transform.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            lifrSkillInfo.childForceExpandWidth = false;

            for (index = (int)Model.Input.音律; index <= (int)Model.Input.杂学; ++index)
            {
                CreateLabel(lifrSkillInfo.transform, ((Model.Input)index).ToString() + ":");
                CreateInputField(lifrSkillInfo.transform, (Model.Input)index);
            }
        }

        private void CreateToggleGroup(VerticalLayoutGroup parent)
        {
            var toggleGroup = new GameObject("ToggleGroup").AddComponent<HorizontalLayoutGroup>();
            SetTransform(parent.transform, toggleGroup.transform);
            toggleGroup.transform.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            toggleGroup.childForceExpandWidth = false;

            CreateLabel(toggleGroup.transform, "性别:");
            for (index = (int)Model.Toggle.全部性别; index <= (int)Model.Toggle.女性; ++index)
            {               
                CreateToggle(toggleGroup.transform, (Model.Toggle)index, ((Model.Toggle)index).ToString());
            }

            CreateLabel(toggleGroup.transform, "立场:");
            for (index = (int)Model.Toggle.全部立场; index <= (int)Model.Toggle.唯我; ++index)
            {
                CreateToggle(toggleGroup.transform, (Model.Toggle)index, ((Model.Toggle)index).ToString());
            }

            CreateLabel(toggleGroup.transform, "婚姻:");
            for (index = (int)Model.Toggle.全部婚姻; index <= (int)Model.Toggle.丧偶; ++index)
            {
                CreateToggle(toggleGroup.transform, (Model.Toggle)index, ((Model.Toggle)index).ToString());
            }

            CreateLabel(toggleGroup.transform, "其他:");

            ToggleDic[Model.Toggle.全部性别].GetComponent<CToggle>().isOn = true;
            ToggleDic[Model.Toggle.全部立场].GetComponent<CToggle>().isOn = true;
            ToggleDic[Model.Toggle.全部婚姻].GetComponent<CToggle>().isOn = true;
        }

        private void CreateMultiSearch(VerticalLayoutGroup parent)
        {
            var multiSearch = new GameObject("MultiSearch").AddComponent<HorizontalLayoutGroup>();
            SetTransform(parent.transform, multiSearch.transform);
            multiSearch.transform.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            multiSearch.childForceExpandWidth = false;

            for (index = (int)Model.Input.角色ID; index <= (int)Model.Input.身份; ++index)
            {
                CreateLabel(multiSearch.transform, ((Model.Input)index).ToString() + ":");
                CreateInputField(multiSearch.transform, (Model.Input)index, minWidth:150);
            }
            for (index = (int)Model.Input.特性; index <= (int)Model.Input.物品; ++index)
            {
                CreateLabel(multiSearch.transform, ((Model.Input)index).ToString() + ":");
                CreateInputField(multiSearch.transform, (Model.Input)index, minWidth:400);
            }

            buttonSearch = CreateButton(multiSearch.transform, Model.CharacterInfo.查找, "查找").GetComponent<CButton>();
            buttonUpdate = CreateButton(multiSearch.transform, Model.CharacterInfo.更新数据, "更新数据").GetComponent<CButton>();
            buttonPrevious = CreateButton(multiSearch.transform, Model.CharacterInfo.上一页, "上一页").GetComponent<CButton>();
            buttonNext = CreateButton(multiSearch.transform, Model.CharacterInfo.下一页, "下一页").GetComponent<CButton>();
            pageCount = CreateLabel(multiSearch.transform, "").GetComponent<TextMeshProUGUI>();
        }
        #endregion

        #region InitScrollView
        private void InitScrollView()
        {           
            ScrollView = ABResourceManager.Instantiate("ScrollView");
            SetTransform(root.transform, ScrollView.transform);
            var rect = ScrollView.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 0.05f);
            rect.anchorMax = new Vector2(1, 0.8f);
            rect.localScale = new Vector3(0.8f, 0.8f, 1);
            ScrollView.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1);            
            ScrollView.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
            ScrollView.GetComponent<ScrollRect>().horizontal = false;
            ScrollView.GetComponent<ScrollRect>().scrollSensitivity = 25;

            scrollContent = ScrollView.transform.Find("Viewport").Find("Content").transform;
            scrollContent.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scrollContent.gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.MinSize;
            scrollContent.gameObject.AddComponent<VerticalLayoutGroup>().spacing = 20;

            CreateTitle(scrollContent);
            CreateScanResult(scrollContent);          
        }

        private void CreateTitle(Transform parent)
        {
            var buttonTitle = new GameObject("ButtonTitle").AddComponent<HorizontalLayoutGroup>();
            buttonTitle.spacing = 20;
            SetTransform(parent, buttonTitle.transform);
            buttonTitle.transform.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            buttonTitle.childForceExpandWidth = false;

            for (index = (int)Model.CharacterInfo.姓名; index <= (int)Model.CharacterInfo.杂学; ++index)
            {
                int minWidth = 100;
                if (index == (int)Model.CharacterInfo.姓名 || index == (int)Model.CharacterInfo.位置)
                    minWidth = 200;
                else if (index == (int)Model.CharacterInfo.性别 || index == (int)Model.CharacterInfo.年龄)
                    minWidth = 50;
                else if (index >= (int)Model.CharacterInfo.膂力 && index <= (int)Model.CharacterInfo.杂学)
                    minWidth = 50;
                CreateButton(buttonTitle.transform, (Model.CharacterInfo)index, ((Model.CharacterInfo)index).ToString(), minWidth: minWidth);
            }
            for (index = (int)Model.CharacterInfo.前世; index <= (int)Model.CharacterInfo.前世; ++index)
            {
                CreateButton(buttonTitle.transform, (Model.CharacterInfo)index, ((Model.CharacterInfo)index).ToString(), minWidth:300);
            }
            for (index = (int)Model.CharacterInfo.物品; index <= (int)Model.CharacterInfo.人物特性; ++index)
            {
                CreateButton(buttonTitle.transform, (Model.CharacterInfo)index, ((Model.CharacterInfo)index).ToString(), minWidth:1000);
            }
        }

        private void CreateScanResult(Transform parent)
        {
            resultLabels = new List<List<Transform>>();  
            for (int rowIndex=0; rowIndex<50; ++rowIndex)
            {
                var line = new GameObject("line" + rowIndex.ToString()).AddComponent<HorizontalLayoutGroup>();
                line.spacing = 20;
                SetTransform(parent, line.transform);
                line.transform.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                line.childForceExpandWidth = false;
                line.gameObject.AddComponent<CImage>().color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                line.gameObject.AddComponent<CButton>();

                List<Transform> rowLabels = new List<Transform>();

                for (index = (int)Model.CharacterInfo.姓名; index <= (int)Model.CharacterInfo.杂学; ++index)
                {
                    int minWidth = 100;
                    if (index == 0 || index == 3)
                        minWidth = 200;
                    else if (index == 1 || index == 2)
                        minWidth = 50;
                    else if (index > 11 && index < 48)
                        minWidth = 50;

                    var lable = CreateLabel(line.transform, "", minWidth: minWidth).transform;
                    rowLabels.Add(lable);                  
                }
                for (index = (int)Model.CharacterInfo.前世; index <= (int)Model.CharacterInfo.前世; ++index)
                {
                    var lable = CreateLabel(line.transform, "", minWidth: 300).transform;
                    lable.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
                    rowLabels.Add(lable);
                }
                for (index = (int)Model.CharacterInfo.物品; index <= (int)Model.CharacterInfo.人物特性; ++index)
                {
                    var lable = CreateLabel(line.transform, "", minWidth: 1000).transform;
                    lable.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.TopJustified;                    

                    rowLabels.Add(lable);
                }

                resultLabels.Add(rowLabels);
            }
        }
        #endregion


        #region CreateGameobject

        private GameObject CreateInputField(Transform parent, Model.Input type, string text = "", float minWidth = 100, float minHeight = 50)
        {
            var gameObject = ABResourceManager.Instantiate("InputField");
            SetTransform(parent, gameObject.transform);
            var element = gameObject.AddComponent<LayoutElement>();
            element.minWidth = minWidth;
            element.minHeight = minHeight;
            gameObject.transform.GetComponent<InputField>().text = text;
            InputDic.Add(type, gameObject.GetComponent<InputField>());

            return gameObject;
        }


        private GameObject CreateLabel(Transform parent, string text, float minWidth = 100, float minHeight = 50)
        {
            var gameObject = GameObjectCreationUtils.InstantiateUIElement(parent, "Text");
            SetTransform(gameObject.transform);
            var element = gameObject.AddComponent<LayoutElement>();
            element.minWidth = minWidth;
            element.minHeight = minHeight;
            gameObject.GetComponent<TextMeshProUGUI>().text = text;

            return gameObject;
        }

        private GameObject CreateToggle(Transform parent, Model.Toggle type, string text, float minWidth = 100, float minHeight = 50)
        {
            var gameObject = GameObjectCreationUtils.InstantiateUIElement(parent, "CommonToggle1_Switch");
            SetTransform(gameObject.transform);
            var element = gameObject.AddComponent<LayoutElement>();
            element.minWidth = minWidth;
            element.minHeight = minHeight;
            gameObject.transform.Find("Labels").Find("LabelOff").GetComponent<TextMeshProUGUI>().text = text;
            gameObject.transform.Find("Labels").Find("LabelOn").GetComponent<TextMeshProUGUI>().text = text;
            ToggleDic.Add(type, gameObject.GetComponent<CToggle>());

            return gameObject;
        }

        private GameObject CreateButton(Transform parent, Model.CharacterInfo type, string text, float minWidth = 100, float minHeight = 50)
        {
            var gameObject = GameObjectCreationUtils.InstantiateUIElement(parent, "BuildingQuickButton");
            SetTransform(gameObject.transform);
            gameObject.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
            var element = gameObject.AddComponent<LayoutElement>();
            element.minWidth = minWidth;
            element.minHeight = minHeight;
            ButtonDic.Add(type, gameObject.GetComponent<CButton>());

            return gameObject;
        }

        #endregion
        private void SetTransform(Transform parent, Transform transform)
        {
            transform.SetParent(parent, false);
            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(1, 1, 1);
        }

        private void SetTransform(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(1, 1, 1);
        }       

        public void Show()
        {
            root.SetActive(true);

            float xScale = Screen.width / inputContainer.preferredWidth;
            inputContainer.transform.localScale = new Vector3(xScale, xScale, 1);

            float height = inputContainer.preferredHeight * inputContainer.transform.localScale.x;
            float yAnchor = 1 - (0.05f * Screen.height + height) / Screen.height;
            var rect = ScrollView.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(1, yAnchor);
            float yScale = yAnchor * Screen.height / rect.rect.height;
            ScrollView.transform.localScale = new Vector3(yScale, yScale, 1);

            if (blockClickGroup != null)
                blockClickGroup.interactable = false;
        }

        public void Hide()
        {
            root.SetActive(false);
            if (blockClickGroup != null)
                blockClickGroup.interactable = true;
        }
    }
}
