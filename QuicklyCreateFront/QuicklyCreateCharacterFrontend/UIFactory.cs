using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace QuicklyCreateCharacterFrontend;

internal class UIFactory
{
	private static GameObject _backgroundContainerGameObjectAPrefab;

	private static GameObject _backgroundContainerGameObjectBPrefab;

	private static GameObject _skillBookPrefab;

	private static GameObject _buttonPrefab;

	private static GameObject _blankLabelPrefab;

	private static GameObject _subTitlePrefab;

	private static GameObject _skillQulificationPrefab;

	private static GameObject _rollIconPrefab;

	public static void PrintChild(Transform elementsRootTF, int level)
	{
		GameObject gameObject = elementsRootTF.gameObject;
		Component[] components = gameObject.GetComponents<Component>();
		Debug.Log($"----------------------------IsNULL{gameObject == null}------------------------------------------");
		Debug.Log("-----------------------------" + elementsRootTF.name + "------------------------------------------------");
		for (int i = 0; i < components.Length; i++)
		{
			Debug.Log($"-----------------------------{components[i].GetType()}------------------------------------------------");
		}
		for (int j = 0; j < gameObject.transform.childCount && j < 100; j++)
		{
			Transform child = gameObject.transform.GetChild(j);
			Component[] components2 = child.GetComponents<Component>();
			Debug.Log($"--------------------No{j}childName: {child.name}------------------------------------------");
			for (int k = 0; k < components2.Length; k++)
			{
				bool flag = components2[k].GetType().IsEquivalentTo(typeof(MouseTipDisplayer));
				Debug.Log($"--------------------No{j}child  {k}name: {components2[k].name}------------------------------------------");
				Debug.Log($"--------------------No{j}child  {k}type: {components2[k].GetType()} {flag}------------------------------------------");
				if (level < 3)
				{
					continue;
				}
				for (int l = 0; l < child.childCount; l++)
				{
					Transform child2 = child.GetChild(l);
					Debug.Log($"--------------------------------No{l} grandChildName: {child2.name}------------------------------------------");
					for (int m = 0; m < child2.childCount; m++)
					{
						Transform child3 = child2.GetChild(m);
						Debug.Log($"-------------------------------------------No{m} grandgrandChildName: {child3.name}------------------------------------------");
						Component[] components3 = child3.GetComponents<Component>();
						Component[] array = components3;
						foreach (Component component in array)
						{
							Debug.Log($"-------------------------------------------No{m} grandgrandChild: ComType {component.GetType()}------------------------------------------");
						}
					}
				}
			}
		}
	}

	public static GameObject GetRollButtonGo(string buttonText, UnityAction onClick)
	{
		GameObject gameObject = Object.Instantiate(GetButtonPrefab());
		GameObject gameObject2 = Object.Instantiate(GetRollIcoPrefab());
		gameObject.transform.Find("LabelBack/Label").GetComponent<TextMeshProUGUI>().text = buttonText;
		ButtonAddOnClick(gameObject, onClick);
		gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		gameObject2.transform.localPosition = gameObject.transform.Find("Icon").localPosition;
		gameObject2.transform.localScale = Vector3.one * 0.5f;
		Object.Destroy(gameObject.transform.Find("Icon").gameObject);
		return gameObject;
	}

	public static GameObject GetRollIcoPrefab()
	{
		List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.NewGame });
		GameObject gameObject = uIElementPrefabs[0];
		Transform child = gameObject.transform.Find("WindowRoot/NewGameBack/ScrollTabs/NameView/NameBack/Container/RandomName");
		_rollIconPrefab = child.gameObject;
		return _rollIconPrefab;
	}

	public static GameObject GetBlankLabelGo()
	{
		if (_blankLabelPrefab == null)
		{
			List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.CharacterMenuInfo });
			GameObject gameObject = uIElementPrefabs[0];
			Transform child = gameObject.transform.Find("ElementsRoot/AreaFeature/FeatureTtitle/Feature");
			_blankLabelPrefab = child.gameObject;
		}
		GameObject gameObject2 = Object.Instantiate(_blankLabelPrefab);
		gameObject2.SetActive(value: true);
		return gameObject2;
	}

	public static GameObject GetSubTitleGo(string titleName)
	{
		if (_subTitlePrefab == null)
		{
			List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.CharacterMenuInfo });
			GameObject gameObject = uIElementPrefabs[0];
			Transform child = gameObject.transform.Find("ElementsRoot/AreaFeature/FeatureTtitle");
			_subTitlePrefab = child.gameObject;
		}
		GameObject gameObject2 = Object.Instantiate(_subTitlePrefab);
		gameObject2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = titleName;
		gameObject2.name = titleName;
		gameObject2.SetActive(value: true);
		return gameObject2;
	}

	public static GameObject GetLifeQulificationGo(int type)
	{
		if (_skillQulificationPrefab == null)
		{
			List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.CharacterMenuLifeSkill });
			GameObject gameObject = uIElementPrefabs[0];
			Transform child = gameObject.transform.Find("ElementsRoot/Attainment/SelectSkillTypeMask/SkillType/TextBack");
			_skillQulificationPrefab = child.gameObject;
		}
		GameObject gameObject2 = Object.Instantiate(_skillQulificationPrefab.gameObject);
		gameObject2.transform.GetChild(0).localPosition = new Vector2(-15f, 0f);
		//gameObject2.transform.GetChild(2).localPosition = new Vector2(35f, 0f);
		gameObject2.transform.GetComponentInChildren<TextMeshProUGUI>().text = "<b>" + CharacterDataTool.LifeSkillNameArray[type].ToString() + "</b>";
		gameObject2.SetActive(value: true);
		gameObject2.name = "lifeSkill" + type;
		return gameObject2;
	}

	public static GameObject GetCombatQulificationGo(int type)
	{
		if (_skillQulificationPrefab == null)
		{
			List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.CharacterMenuLifeSkill });
			GameObject gameObject = uIElementPrefabs[0];
			Transform child = gameObject.transform.Find("ElementsRoot/Attainment/SelectSkillTypeMask/SkillType/TextBack");
			_skillQulificationPrefab = child.gameObject;
		}
		GameObject gameObject2 = Object.Instantiate(_skillQulificationPrefab);
		gameObject2.transform.GetChild(0).localPosition = new Vector2(-15f, 0f);
		//gameObject2.transform.GetChild(7).localPosition = new Vector2(35f, 0f);
		gameObject2.transform.GetComponentInChildren<TextMeshProUGUI>().text = "<b>" + CharacterDataTool.CombatSkillNameArray[type].ToString() + "</b>";
		gameObject2.SetActive(value: true);
		gameObject2.name = "combatSkill" + type;
		return gameObject2;
	}

	public static GameObject GetSkillBookGo()
	{
		if (_skillBookPrefab == null)
		{
			List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.CharacterMenuLifeSkill });
			GameObject gameObject = uIElementPrefabs[0];
			Transform child = gameObject.transform.Find("ElementsRoot/Attainment/SelectSkillTypeMask/SkillType/TextBack");
			_skillBookPrefab = child.gameObject;
		}
		GameObject gameObject2 = Object.Instantiate(_skillBookPrefab.gameObject);
		gameObject2.transform.GetChild(0).localPosition = new Vector2(0f, 10f);
		//gameObject2.transform.GetChild(2).localPosition = new Vector2(0f, -10f);
		gameObject2.transform.GetChild(0).localScale = Vector3.one * 0.6f;
		//gameObject2.transform.GetChild(2).localScale = Vector3.one * 0.6f;
		gameObject2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Midline;
		//gameObject2.transform.GetChild(2).GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Midline;
		gameObject2.transform.localScale = Vector3.one * 2f;
		gameObject2.SetActive(value: true);
		gameObject2.name = "SkillBook";
		return gameObject2;
	}

	public static GameObject GetTotalTitleGo(string titleText = "标题")
	{
		List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.Dialog });
		GameObject gameObject = uIElementPrefabs[0];
		GameObject gameObject2 = gameObject.transform.Find("AnimRoot/ImgTitle36").gameObject;
		GameObject gameObject3 = Object.Instantiate(gameObject2);
		gameObject3.SetActive(value: true);
		gameObject3.name = "TotalTitle";
		gameObject3.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = titleText;
		return gameObject3;
	}

	public static GameObject GetMainAttributeGo()
	{
		List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.CharacterMenu });
		GameObject gameObject = uIElementPrefabs[0];
		Transform child = gameObject.transform.Find("AnimationRoot/ChildPages/CharacterAttributeView/");
		GameObject gameObject2 = Object.Instantiate(child.gameObject);
		gameObject2.SetActive(value: true);
		gameObject2.name = "MainAttribute";
		gameObject2.transform.Find("TabToggleGroup").gameObject.SetActive(value: false);
		gameObject2.transform.Find("TabAttribute").Find("AttackAttributeTitleBack").Find("DifficultyLayout_Penetrations")
			.gameObject.SetActive(value: false);
		gameObject2.transform.Find("TabAttribute").Find("DefendAttributeTitleBack").Find("DifficultyLayout_PenetrationResists")
			.gameObject.SetActive(value: false);
		gameObject2.transform.Find("TabAttribute").Find("HitAttributeTitleBack").Find("DifficultyLayout_HitValues")
			.gameObject.SetActive(value: false);
		gameObject2.transform.Find("TabAttribute").Find("AvoidAttributeTitleBack").Find("DifficultyLayout_AvoidValues")
			.gameObject.SetActive(value: false);
		gameObject2.transform.Find("TabAttribute").Find("MinorAttributeTitleBack").Find("DifficultyLayout_SecondAttribute")
			.gameObject.SetActive(value: false);
		return gameObject2;
	}

	public static GameObject GetFeatureScrollGo()
	{
		List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.CharacterMenuInfo });
		GameObject gameObject = uIElementPrefabs[0];
		Transform child = gameObject.transform.Find("ElementsRoot/AreaFeature/FeatureScroll");
		return Object.Instantiate(child.gameObject);
	}

	public static GameObject GetTotalMedalGo()
	{
		List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.CharacterMenuInfo });
		GameObject gameObject = uIElementPrefabs[0];
		Transform child = gameObject.transform.Find("ElementsRoot/AreaFeature/TotalMedal/");
		return Object.Instantiate(child.gameObject);
	}

	public static GameObject GetCommonButtonGo(string buttonText, UnityAction onClick, bool hasIcon)
	{
		GameObject buttonPrefab = GetButtonPrefab();
		GameObject gameObject = Object.Instantiate(buttonPrefab);
		gameObject.transform.Find("LabelBack/Label").GetComponent<TextMeshProUGUI>().text = buttonText;
		if (!hasIcon)
		{
			Object.Destroy(gameObject.transform.Find("Icon").gameObject);
			gameObject.transform.Find("LabelBack").localPosition = Vector2.zero;
		}
		ButtonAddOnClick(gameObject, onClick);
		return gameObject;
	}

	public static GameObject GetNewXCloseButtonGo(UnityAction onClick = null)
	{
		List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.Dialog });
		GameObject gameObject = uIElementPrefabs[0];
		GameObject gameObject2 = gameObject.transform.Find("AnimRoot/BtnNo").gameObject;
		GameObject gameObject3 = Object.Instantiate(gameObject2);
		gameObject3.SetActive(value: true);
		gameObject3.name = "xCloseButton";
		ButtonAddOnClick(gameObject3, onClick);
		return gameObject3;
	}

	public static GameObject GetNewUIMaskGo(Color color)
	{
		List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.SystemOption });
		GameObject gameObject = uIElementPrefabs[0];
		Transform transform = gameObject.transform.Find("UIMask");
		GameObject gameObject2 = Object.Instantiate(transform.gameObject);
		gameObject2.SetActive(value: true);
		gameObject2.name = "UIMask";
		gameObject2.GetComponent<CRawImage>().color = color;
		return gameObject2;
	}

	public static GameObject GetNewBackgroundContainerGoA()
	{
		if (_backgroundContainerGameObjectAPrefab == null)
		{
			List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.NewGame });
			GameObject gameObject = uIElementPrefabs[0];
			GameObject gameObject2 = gameObject.transform.Find("WindowRoot/NewGameBack/ScrollTabs/SettingView").gameObject;
			_backgroundContainerGameObjectAPrefab = gameObject2;
		}
		GameObject gameObject3 = Object.Instantiate(_backgroundContainerGameObjectAPrefab);
		gameObject3.transform.Find("SettingBack").gameObject.SetActive(value: false);
		gameObject3.SetActive(value: true);
		return gameObject3;
	}

	public static GameObject GetNewBackgroundContainerGoB()
	{
		if (_backgroundContainerGameObjectBPrefab == null)
		{
			List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.CharacterMenu });
			GameObject gameObject = uIElementPrefabs[0];
			GameObject gameObject2 = gameObject.transform.Find("SubPageBack").gameObject;
			_backgroundContainerGameObjectBPrefab = gameObject2;
		}
		return Object.Instantiate(_backgroundContainerGameObjectBPrefab);
	}

	private static GameObject GetButtonPrefab()
	{
		if (_buttonPrefab == null)
		{
			List<GameObject> uIElementPrefabs = UITool.GetUIElementPrefabs(new List<UIElement> { UIElement.SystemOption });
			GameObject gameObject = uIElementPrefabs[0];
			Transform transform = gameObject.transform.Find("MainWindow/ButtonHolder/ReturnToGame");
			if (transform == null)
			{
				transform = gameObject.transform.Find("MainWindow/ReturnToGame");
			}
			GameObject gameObject2 = transform.gameObject;
			_buttonPrefab = gameObject2;
		}
		return _buttonPrefab;
	}

	public static void ButtonAddOnClick(GameObject button, UnityAction onClick)
	{
		button.GetComponent<CButton>().onClick.RemoveAllListeners();
		if (onClick != null)
		{
			button.GetComponent<CButton>().onClick.AddListener(onClick);
		}
	}
}
