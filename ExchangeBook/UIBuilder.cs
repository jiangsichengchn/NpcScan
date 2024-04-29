using Config;
using DG.Tweening;
using DG.Tweening.Core;
using FrameWork.ModSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DG.Tweening.DOTweenAnimation;

namespace ExchangeBook;

public class UIBuilder
{
	private static GameObject UI_ExchangeBook;

	public static GameObject UI_Reading;

	public static void PrepareMaterial()
	{
		ResLoader.Load<GameObject>("RemakeResources/Prefab/Views/UI_ExchangeBook", OnPrefabLoaded);
		ResLoader.Load<GameObject>("RemakeResources/Prefab/Views/Reading/UI_Reading", OnPrefabLoaded);
		static void OnPrefabLoaded(GameObject obj)
		{
			if (obj.name == "UI_ExchangeBook")
			{
				UI_ExchangeBook = obj;
			}
			else if (obj.name == "UI_Reading")
			{
				UI_Reading = obj;
			}

		}
	}

	public static GameObject BuildMainUI(string name)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.SetActive(value: false);
		gameObject.layer = 5;
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.SetAnchor(Vector2.zero, Vector2.one);
		rectTransform.sizeDelta = Vector2.zero;
		gameObject.transform.localScale = Vector3.one;
		Canvas canvas = gameObject.AddComponent<Canvas>();
		gameObject.AddComponent<ConchShipGraphicRaycaster>();
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.sortingLayerName = "UI";
		GameObject gameObject2 = GameObjectCreationUtils.InstantiateUIElement(gameObject.transform, "UIMask");
		gameObject2.GetComponent<RectTransform>().SetAnchor(Vector2.zero, Vector2.one);
		GameObject gameObject3 = new GameObject("MoveIn");
		GameObject gameObject4 = new GameObject("MoveOut");
		gameObject3.transform.SetParent(gameObject.transform);
		gameObject4.transform.SetParent(gameObject.transform);
		GameObject gameObject5 = Object.Instantiate(UI_ExchangeBook.transform.Find("MainWindow").gameObject, gameObject.transform);
		gameObject5.name = "MainWindow";
		DOTweenAnimation dOTweenAnimation = gameObject3.AddComponent<DOTweenAnimation>();
		dOTweenAnimation.name = "MoveIn";
		DOTweenAnimation dOTweenAnimation2 = gameObject4.AddComponent<DOTweenAnimation>();
		dOTweenAnimation2.name = "MoveOut";
		dOTweenAnimation.animationType = (AnimationType)1;
		dOTweenAnimation.endValueV3 = new Vector3(0f, 1500f, 0f);
		dOTweenAnimation.duration = 0.2f;
		dOTweenAnimation.easeType = Ease.Linear;
		dOTweenAnimation2.animationType = (AnimationType)1;
		dOTweenAnimation2.endValueV3 = new Vector3(0f, 1500f, 0f);
		dOTweenAnimation2.duration = 0.2f;
		dOTweenAnimation2.easeType = Ease.Linear;
		dOTweenAnimation.targetIsSelf = (dOTweenAnimation2.targetIsSelf = false);
		dOTweenAnimation.isRelative = (dOTweenAnimation2.isRelative = true);
		dOTweenAnimation.isFrom = true;
		dOTweenAnimation2.isFrom = false;
		dOTweenAnimation.isValid = (dOTweenAnimation2.isValid = true);
		dOTweenAnimation.target = (dOTweenAnimation2.target = gameObject5.GetComponent<RectTransform>());
		dOTweenAnimation.targetGO = (dOTweenAnimation2.targetGO = gameObject5);
		TargetType targetType = (TargetType)5;
		dOTweenAnimation2.targetType = (TargetType)5;
		dOTweenAnimation.targetType = targetType;
		dOTweenAnimation.autoKill = (dOTweenAnimation2.autoKill = false);
		dOTweenAnimation.autoPlay = (dOTweenAnimation2.autoPlay = false);
		Object.Destroy(gameObject5.transform.Find("TaiwuBooks/").gameObject);
		Object.Destroy(gameObject5.transform.Find("ExchangeArea/Self/").gameObject);
		Object.Destroy(gameObject5.transform.Find("ExchangeArea/Splitter/").gameObject);
		Object.Destroy(gameObject5.transform.Find("ExchangeArea/NpcPrestige/").gameObject);
		gameObject5.transform.Find("ExchangeArea/Npc/").gameObject.GetComponent<RectTransform>().SetWidth(2160f);
		GameObject gameObject6 = gameObject5.transform.Find("NpcBooks/").gameObject;
		Object.Destroy(gameObject6.transform.Find("Load/").gameObject);
		RectTransform component = gameObject6.GetComponent<RectTransform>();
		component.SetWidth(component.rect.width * 2f);
		component.anchoredPosition = new Vector2(1047f, 684f);
		gameObject6.transform.Find("NpcItemScroll/").gameObject.GetComponent<RectTransform>().SetWidth(1696f);
		GameObject gameObject7 = Object.Instantiate(UI_Reading.transform.Find("MainWindow/BookBg/LifeSkillTypeTogGroup/"), gameObject6.transform).gameObject;
		gameObject7.name = "LifeSkillTypeTogGroup";
		RectTransform component2 = gameObject7.GetComponent<RectTransform>();
		component2.SetAnchor(Vector2.up, Vector2.up);
		component2.anchoredPosition = new Vector2(0, -100f);
		CToggleGroup component3 = gameObject7.GetComponent<CToggleGroup>();
		gameObject7.transform.GetChild(0).gameObject.SetActive(value: false);
		for (int i = 1; i < gameObject7.transform.childCount; i++)
		{
			gameObject7.transform.GetChild(i).gameObject.SetActive(i <= 14);
		}
		for (int j = 0; j < 14; j++)
		{
			Refers component4 = component3.Get(j).GetComponent<Refers>();
			component4.CGet<TextMeshProUGUI>("Label").text = CombatSkillType.Instance[j].Name;
			string filterCombatSkillTypeIcon = CommonUtils.GetFilterCombatSkillTypeIcon(j);
			component4.CGet<CImage>("Icon").SetSprite(filterCombatSkillTypeIcon);
		}
		component3.SetAllowOnNum(1);
		component3.AllowSwitchOff = false;
		component3.Set(0);
		GameObject gameObject8 = Object.Instantiate(gameObject7, gameObject6.transform).gameObject;
		gameObject8.name = "GradeToggleGroup";
		RectTransform component5 = gameObject8.GetComponent<RectTransform>();
		component5.anchoredPosition = new Vector2(0, -300f);
		CToggleGroup component6 = gameObject8.GetComponent<CToggleGroup>();
		for (int k = 10; k < 15; k++)
		{
			component6.transform.GetChild(k).gameObject.SetActive(value: false);
		}
		for (int l = 0; l < 9; l++)
		{
			Refers component7 = component6.Get(l).GetComponent<Refers>();
			component7.CGet<TextMeshProUGUI>("Label").text = (LocalStringManager.Get($"LK_Num_{9 - l}") + LocalStringManager.Get("LK_Item_Grade")).SetColor(Colors.Instance.GradeColors[l]);
			component7.CGet<CImage>("Icon").gameObject.SetActive(value: false);
		}
		gameObject5.transform.Find("NpcBooks/NpcItemScroll/ItemSortAndFilter/Back/").gameObject.GetComponent<RectTransform>().SetWidth(1755f);
		gameObject5.transform.Find("NpcBooks/NpcItemScroll/Viewport/").gameObject.GetComponent<RectTransform>().SetWidth(1696f);
		gameObject5.transform.Find("NpcBooks/NpcItemScroll/Viewport/Content").gameObject.GetComponent<RectTransform>().SetWidth(1678f);
		GameObject gameObject9 = gameObject5.transform.Find("NpcBooks/NpcItemScroll/ItemSortAndFilter/Filter/").gameObject;
		for (int m = 0; m < gameObject9.transform.childCount; m++)
		{
			gameObject9.transform.GetChild(m).gameObject.SetActive(m < 6);
			gameObject9.transform.GetChild(m).gameObject.name = m.ToString();
		}
		GameObject gameObject10 = Object.Instantiate(gameObject9, gameObject9.transform.parent);
		gameObject10.name = "FirstPageFilter";
		for (int n = 1; n < 6; n++)
		{
			GameObject gameObject11 = gameObject10.transform.GetChild(n).GetChild(1).gameObject;
			Object.DestroyImmediate(gameObject11.GetComponent<TextLanguage>());
			gameObject11.GetComponent<TextMeshProUGUI>().text = LocalStringManager.Get($"LK_CombatSkill_First_Page_Type_{n - 1}");
		}
		CToggleGroup component8 = gameObject10.GetComponent<CToggleGroup>();
		component8.SetAllowOnNum(1);
		component8.AllowSwitchOff = false;
		GameObject gameObject12 = Object.Instantiate(gameObject10, gameObject9.transform.parent);
		gameObject12.name = "DirectPageFilter";
		gameObject12.GetComponent<RectTransform>().anchoredPosition = new Vector2(400f, 0f);
		gameObject12.transform.GetChild(0).gameObject.SetActive(value: false);
		for (int num = 0; num < 5; num++)
		{
			GameObject gameObject13 = gameObject12.transform.GetChild(num + 1).GetChild(1).gameObject;
			gameObject13.GetComponent<TextMeshProUGUI>().text = LocalStringManager.Get($"LK_CombatSkill_Direct_Page_{num}");
		}
		CToggleGroup component9 = gameObject12.GetComponent<CToggleGroup>();
		component9.SetAllowOnNum(5);
		component9.AllowSwitchOff = true;
		component9.AllowUncheck = true;
		GameObject gameObject14 = Object.Instantiate(gameObject12, gameObject9.transform.parent);
		gameObject14.name = "ReversePageFilter";
		gameObject14.GetComponent<RectTransform>().anchoredPosition = new Vector2(720f, 0f);
		for (int num2 = 0; num2 < 5; num2++)
		{
			GameObject gameObject15 = gameObject14.transform.GetChild(num2 + 1).GetChild(1).gameObject;
			gameObject15.GetComponent<TextMeshProUGUI>().text = LocalStringManager.Get($"LK_CombatSkill_Reverse_Page_{num2}");
		}
		GameObject gameObject16 = GameObjectCreationUtils.InstantiateUIElement(gameObject9.transform.parent, "AutoBackLabel");
		RectTransform component10 = gameObject16.GetComponent<RectTransform>();
		component10.SetAnchor(new Vector2(0f, 0.5f), new Vector2(0f, 0.5f));
		component10.anchoredPosition = new Vector2(1050f, 0f);
		Transform child = component10.GetChild(1);
		child.GetComponent<LayoutElement>().minWidth = 50f;
		child.GetComponent<TextMeshProUGUI>().text = "完整页";
		GameObject gameObject17 = Object.Instantiate(gameObject12, gameObject9.transform.parent);
		gameObject17.name = "CompletePageFilter";
		gameObject17.GetComponent<RectTransform>().anchoredPosition = new Vector2(1100f, 0f);
		for (int num3 = 1; num3 < 6; num3++)
		{
			GameObject gameObject18 = gameObject17.transform.GetChild(num3).GetChild(1).gameObject;
			gameObject18.GetComponent<TextMeshProUGUI>().text = LocalStringManager.Get($"LK_Num_{num3}");
		}
		ItemScrollView component11 = gameObject5.transform.Find("NpcBooks/NpcItemScroll/").gameObject.GetComponent<ItemScrollView>();
		component11.DetailViewLineCount = 6;
		component11.SimpleViewLineCount = 10;
		InfinityScroll componentInChildren = component11.GetComponentInChildren<InfinityScroll>();
		componentInChildren.LineCount = 6;
		componentInChildren.InitPageCount();
		gameObject9.SetActive(value: false);
		return gameObject;
	}
}
