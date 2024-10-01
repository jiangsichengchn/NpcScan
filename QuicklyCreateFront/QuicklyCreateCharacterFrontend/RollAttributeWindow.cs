using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UICommon.Character;
using UICommon.Character.Elements;
using UnityEngine;

namespace QuicklyCreateCharacterFrontend;

public class RollAttributeWindow : MonoBehaviour
{
	private class QulificationContainer
	{
		public GameObject Go;

		public TextMeshProUGUI NameTextMesh;

		private string name;

		//public TextMeshProUGUI ValueTextMesh;

		public QulificationContainer(GameObject gameObject)
		{
			Go = gameObject;
			NameTextMesh = Go.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			name = NameTextMesh.text;
			//ValueTextMesh = Go.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
		}

		public void SetColor(Color color)
		{
			NameTextMesh.color = color;
			//ValueTextMesh.color = color;
		}

		public void SetValue(int value)
		{
			NameTextMesh.text = name + " <b>" + value + "</b>";
			if (value >= 90)
			{
				SetColor(Color.red);
			}
			else if (value >= 80)
			{
				SetColor(Color.yellow);
			}
			else if (value >= 70)
			{
				SetColor(new Color(0.8f, 0.2f, 0.8f));
			}
			else if (value >= 60)
			{
				SetColor(new Color(0f, 0.8f, 0.8f));
			}
			else if (value >= 40)
			{
				SetColor(Color.white);
			}
			else
			{
				SetColor(new Color(0.6f, 0.6f, 0.6f));
			}
		}
	}

	public CharacterDataController characterDataController = new CharacterDataController();

	private CharacterFeatureScroll characterFeatureScroll;

	private RectTransform totalMedal;

	private CharacterMajorAttribute characterMajorAttribute;

	private CharacterSecondaryAttribute characterSecondaryAttribute;

	private Dictionary<int, QulificationContainer> lifeQulificationContainerDict = new Dictionary<int, QulificationContainer>();

	private Dictionary<int, QulificationContainer> combatQulificationContainerDict = new Dictionary<int, QulificationContainer>();

	private Vector3 _horizontalOffset = new Vector3(115f, 0f, 0f);

	private float _titleUnderSpace = 40f;

	private Vector2 _backgroundOffset = new Vector2(0f, 100f);

	private Vector2 _windowPadding = new Vector2(40f, 40f) + new Vector2(60f, 0f);

	private Vector2 _gameObjectMargin = new Vector2(20f, 20f);

	private Vector2 _windowSize = new Vector2(1600f, 1100f);

	private Vector2 _initialPostion;

	private Vector2 _currentPostion;

	private string _title = "随机人物属性";

	private Canvas _rootCanvas;

	private GameObject _layer;

	private GameObject _maskGo;

	public GameObject _backgroundGo;

	private GameObject _closeButton;

	private GameObject _rollButton;

	private GameObject _combatGrowthLabelGo;

	private GameObject _lifeGrowthLabelGo;

	private GameObject _lifeSkillBookGo;

	private GameObject _combatSkillBookGo;

	private void Awake()
	{
		_initialPostion = _windowSize / 2f + new Vector2(0f - _windowSize.x, 0f) + new Vector2(_windowPadding.x, 0f - _windowPadding.y);
		_currentPostion = _initialPostion;
	}

	private void Start()
	{
		CreateUI();
	}

	private void OnDestroy()
	{
		if (_maskGo != null)
		{
			Object.Destroy(_maskGo);
		}
	}

	private void CreateUI()
	{
		GameObject newUIMaskGo = UIFactory.GetNewUIMaskGo(new Color(0f, 0f, 0f, 0.8f));
		newUIMaskGo.transform.SetParent(_layer.transform, worldPositionStays: false);
		newUIMaskGo.name = "customUIMask";
		_maskGo = newUIMaskGo;
		GameObject gameObject = (_backgroundGo = UIFactory.GetNewBackgroundContainerGoA());
		gameObject.transform.SetParent(_maskGo.transform, worldPositionStays: false);
		Vector2 sizeDelta = gameObject.transform.Find("CoverFrame2").gameObject.GetComponent<RectTransform>().sizeDelta;
		Vector2 sizeDelta2 = gameObject.transform.Find("CoverFrame").gameObject.GetComponent<RectTransform>().sizeDelta;
		Vector2 sizeDelta3 = _windowSize * sizeDelta2 / sizeDelta;
		gameObject.transform.Find("CoverFrame2").gameObject.GetComponent<RectTransform>().sizeDelta = _windowSize;
		gameObject.transform.Find("CoverFrame").gameObject.GetComponent<RectTransform>().sizeDelta = sizeDelta3;
		gameObject.transform.localPosition = _backgroundOffset;
		gameObject.GetComponent<RectTransform>().sizeDelta = sizeDelta3;
		gameObject.name = "customBackground";
		GameObject totalTitleGo = UIFactory.GetTotalTitleGo(_title);
		totalTitleGo.transform.SetParent(_backgroundGo.transform, worldPositionStays: false);
		totalTitleGo.GetComponent<RectTransform>().sizeDelta = new Vector2(_windowSize.x - _windowPadding.x * 2f, 45f);
		totalTitleGo.transform.localPosition = new Vector2(0f, _windowSize.y * 0.45f);
		ReSetPosition(totalTitleGo.GetComponent<RectTransform>().sizeDelta.y + _titleUnderSpace - _gameObjectMargin.y);
		_closeButton = UIFactory.GetNewXCloseButtonGo(Close);
		_closeButton.transform.SetParent(gameObject.transform, worldPositionStays: false);
		_closeButton.transform.localPosition = _windowSize * 0.5f;
		_closeButton.name = "customCloseBtn";
		_rollButton = UIFactory.GetRollButtonGo("重置属性", characterDataController.DoRollCharacterData);
		_rollButton.transform.SetParent(gameObject.transform, worldPositionStays: false);
		_rollButton.transform.localPosition = new Vector2(0f, (0f - _windowSize.y) * 0.6f);
		_rollButton.name = "customRollBtn";
		GameObject newBackgroundContainerGoB = UIFactory.GetNewBackgroundContainerGoB();
		newBackgroundContainerGoB.transform.SetParent(_maskGo.transform, worldPositionStays: false);
		newBackgroundContainerGoB.GetComponent<RectTransform>().sizeDelta = new Vector2(1000f, 400f);
		newBackgroundContainerGoB.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1000f, 400f);
		newBackgroundContainerGoB.transform.localPosition = new Vector3(-250f, 110f, 0f);
		GameObject gameObject2 = CreateEmptyContainerGo(newBackgroundContainerGoB, new Vector3(-450f, 300f, 0f));
		GameObject subTitleGo = UIFactory.GetSubTitleGo("技艺资质");
		subTitleGo.transform.SetParent(gameObject2.transform, worldPositionStays: false);
		subTitleGo.transform.localPosition = new Vector3(0f, 50f, 0f);
		GameObject blankLabelGo = UIFactory.GetBlankLabelGo();
		blankLabelGo.transform.SetParent(gameObject2.transform, worldPositionStays: false);
		blankLabelGo.transform.localPosition = new Vector3(100f, 50f, 0f);
		_lifeGrowthLabelGo = blankLabelGo;
		for (int i = 0; i < 8; i++)
		{
			GameObject lifeQulificationGo = UIFactory.GetLifeQulificationGo(i);
			lifeQulificationGo.transform.SetParent(gameObject2.transform, worldPositionStays: false);
			lifeQulificationContainerDict.Add(i, new QulificationContainer(lifeQulificationGo));
			if (i > 0)
			{
				lifeQulificationGo.transform.localPosition = lifeQulificationContainerDict[i - 1].Go.transform.localPosition + _horizontalOffset;
			}
			else
			{
				lifeQulificationGo.transform.localPosition = new Vector2(40f, 0f);
			}
		}
		for (int j = 8; j < 16; j++)
		{
			GameObject lifeQulificationGo2 = UIFactory.GetLifeQulificationGo(j);
			lifeQulificationGo2.transform.SetParent(gameObject2.transform, worldPositionStays: false);
			lifeQulificationContainerDict.Add(j, new QulificationContainer(lifeQulificationGo2));
			if (j > 8)
			{
				lifeQulificationGo2.transform.localPosition = lifeQulificationContainerDict[j - 1].Go.transform.localPosition + _horizontalOffset;
			}
			else
			{
				lifeQulificationGo2.transform.localPosition = new Vector2(40f, -60f);
			}
		}
		GameObject gameObject3 = CreateEmptyContainerGo(newBackgroundContainerGoB, new Vector3(-450f, 130f, 0f));
		GameObject subTitleGo2 = UIFactory.GetSubTitleGo("功法资质");
		subTitleGo2.transform.SetParent(gameObject3.transform, worldPositionStays: false);
		subTitleGo2.transform.localPosition = new Vector3(0f, 50f, 0f);
		GameObject blankLabelGo2 = UIFactory.GetBlankLabelGo();
		blankLabelGo2.transform.SetParent(gameObject3.transform, worldPositionStays: false);
		blankLabelGo2.transform.localPosition = new Vector3(100f, 50f, 0f);
		_combatGrowthLabelGo = blankLabelGo2;
		for (int k = 0; k < 7; k++)
		{
			GameObject combatQulificationGo = UIFactory.GetCombatQulificationGo(k);
			combatQulificationGo.transform.SetParent(gameObject3.transform, worldPositionStays: false);
			combatQulificationContainerDict.Add(k, new QulificationContainer(combatQulificationGo));
			if (k > 0)
			{
				combatQulificationGo.transform.localPosition = combatQulificationContainerDict[k - 1].Go.transform.localPosition + _horizontalOffset;
			}
			else
			{
				combatQulificationGo.transform.localPosition = new Vector2(40f, 0f);
			}
		}
		for (int l = 7; l < 14; l++)
		{
			GameObject combatQulificationGo2 = UIFactory.GetCombatQulificationGo(l);
			combatQulificationGo2.transform.SetParent(gameObject3.transform, worldPositionStays: false);
			combatQulificationContainerDict.Add(l, new QulificationContainer(combatQulificationGo2));
			if (l > 7)
			{
				combatQulificationGo2.transform.localPosition = combatQulificationContainerDict[l - 1].Go.transform.localPosition + _horizontalOffset;
			}
			else
			{
				combatQulificationGo2.transform.localPosition = new Vector3(40f, -60f, 0f);
			}
		}
		GameObject newBackgroundContainerGoB2 = UIFactory.GetNewBackgroundContainerGoB();
		newBackgroundContainerGoB2.transform.SetParent(_maskGo.transform, worldPositionStays: false);
		newBackgroundContainerGoB2.GetComponent<RectTransform>().sizeDelta = new Vector2(1000f, 300f);
		newBackgroundContainerGoB2.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1000f, 300f);
		newBackgroundContainerGoB2.transform.localPosition = new Vector3(-250f, -250f, 0f);
		GameObject gameObject4 = CreateEmptyContainerGo(newBackgroundContainerGoB2, new Vector3(-400f, 270f, 0f));
		GameObject subTitleGo3 = UIFactory.GetSubTitleGo("人物特性");
		subTitleGo3.transform.SetParent(gameObject4.transform, worldPositionStays: false);
		subTitleGo3.transform.localPosition = Vector3.zero;
		GameObject featureScrollGo = UIFactory.GetFeatureScrollGo();
		featureScrollGo.transform.SetParent(gameObject4.transform, worldPositionStays: false);
		featureScrollGo.transform.localPosition = new Vector3(400f, -170f, 0f);
		GameObject totalMedalGo = UIFactory.GetTotalMedalGo();
		totalMedalGo.transform.SetParent(gameObject4.transform, worldPositionStays: false);
		totalMedalGo.transform.localPosition = new Vector3(200f, 0f, 0f);
		RectTransform rectTransform = (totalMedal = totalMedalGo.GetComponent<RectTransform>());
		InfinityScroll component = featureScrollGo.GetComponent<InfinityScroll>();
		CharacterFeatureScroll characterFeatureScroll = new CharacterFeatureScroll(component, rectTransform);
		this.characterFeatureScroll = characterFeatureScroll;
		GameObject newBackgroundContainerGoB3 = UIFactory.GetNewBackgroundContainerGoB();
		newBackgroundContainerGoB3.transform.SetParent(_maskGo.transform, worldPositionStays: false);
		newBackgroundContainerGoB3.GetComponent<RectTransform>().sizeDelta = new Vector2(1000f, 150f);
		newBackgroundContainerGoB3.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1000f, 150f);
		newBackgroundContainerGoB3.transform.localPosition = new Vector3(-250f, -425f, 0f);
		GameObject gameObject5 = CreateEmptyContainerGo(newBackgroundContainerGoB3, new Vector3(-400f, 120f, 0f));
		GameObject subTitleGo4 = UIFactory.GetSubTitleGo("古冢遗刻");
		subTitleGo4.transform.SetParent(gameObject5.transform, worldPositionStays: false);
		subTitleGo4.transform.localPosition = Vector3.zero;
		GameObject skillBookGo = UIFactory.GetSkillBookGo();
		skillBookGo.transform.SetParent(gameObject5.transform, worldPositionStays: false);
		skillBookGo.transform.localPosition = new Vector3(300f, -60f, 0f);
		_lifeSkillBookGo = skillBookGo;
		GameObject skillBookGo2 = UIFactory.GetSkillBookGo();
		skillBookGo2.transform.SetParent(gameObject5.transform, worldPositionStays: false);
		skillBookGo2.transform.localPosition = new Vector3(600f, -60f, 0f);
		_combatSkillBookGo = skillBookGo2;
		GameObject mainAttributeGo = UIFactory.GetMainAttributeGo();
		mainAttributeGo.transform.SetParent(_maskGo.transform, worldPositionStays: false);
		mainAttributeGo.transform.localPosition = new Vector3(750f, 90f, 0f);
		CharacterAttributeDataView component2 = mainAttributeGo.GetComponent<CharacterAttributeDataView>();
		component2.IsTaiwuTeam = true;
		Traverse traverse = Traverse.Create(component2);
		characterMajorAttribute = (CharacterMajorAttribute)traverse.Field("_majorAttributeController").GetValue();
		characterSecondaryAttribute = (CharacterSecondaryAttribute)traverse.Field("_secondaryAttributeController").GetValue();
		GameObject subTitleGo5 = UIFactory.GetSubTitleGo("主要属性");
		subTitleGo5.transform.SetParent(mainAttributeGo.transform, worldPositionStays: false);
		subTitleGo5.transform.localPosition = new Vector3(-210f, 430f, 0f);
		characterDataController.updateDataEvent += UpdateData;
	}

	private GameObject CreateEmptyContainerGo(GameObject backGo, Vector3 localPositonValue)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.SetParent(backGo.transform, worldPositionStays: false);
		gameObject.transform.localPosition = localPositonValue;
		return gameObject;
	}

	private void UpdateData()
	{
		SetSkillGrowthValue();
		SetLifeQualificationValue();
		SetCombatQualificationValue();
		SetFeatureIdValue();
		SetTotalMedalValue();
		SetAttributeValue();
		SetLifeSkillBookValue();
		SetCombatSkillBookValue();
	}

	private void SetLifeQualificationValue()
	{
		if (lifeQulificationContainerDict.Count == 0)
		{
			return;
		}
		foreach (int key in lifeQulificationContainerDict.Keys)
		{
			lifeQulificationContainerDict[key].SetValue(characterDataController.characterDataShortDict[CharacterDataType.LifeSkillQualification][key]);
		}
	}

	private void SetCombatQualificationValue()
	{
		if (combatQulificationContainerDict.Count == 0)
		{
			return;
		}
		foreach (int key in combatQulificationContainerDict.Keys)
		{
			combatQulificationContainerDict[key].SetValue(characterDataController.characterDataShortDict[CharacterDataType.CombatSkillQualification][key]);
		}
	}

	private void SetSkillGrowthValue()
	{
		if (!(_combatGrowthLabelGo == null))
		{
			if (characterDataController.characterDataNameDict.ContainsKey(CharacterDataType.CombatSkillGrowthType))
			{
				string text = characterDataController.characterDataNameDict[CharacterDataType.LifeSkillGrowthType][0];
				_lifeGrowthLabelGo.transform.GetComponent<TextMeshProUGUI>().text = text;
				string text2 = characterDataController.characterDataNameDict[CharacterDataType.CombatSkillGrowthType][0];
				_combatGrowthLabelGo.transform.GetComponent<TextMeshProUGUI>().text = text2;
			}
			else
			{
				_lifeGrowthLabelGo.transform.GetComponent<TextMeshProUGUI>().text = "无";
				_combatGrowthLabelGo.transform.GetComponent<TextMeshProUGUI>().text = "无";
			}
		}
	}

	private void SetFeatureIdValue()
	{
		characterFeatureScroll.ResetToEmpty();
		characterFeatureScroll.CharacterId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		if (characterDataController.characterDataShortDict.ContainsKey(CharacterDataType.FeatureIds))
		{
			characterFeatureScroll.SetShowFeatureListFromOutside(characterDataController.characterDataShortDict[CharacterDataType.FeatureIds]);
			return;
		}
		characterFeatureScroll.SetShowFeatureListFromOutside(new List<short> { 1 });
	}

	private void SetTotalMedalValue()
	{
		List<short> list = ((!characterDataController.characterDataShortDict.ContainsKey(CharacterDataType.FeatureMedalValue)) ? new List<short> { 0, 0, 0 } : characterDataController.characterDataShortDict[CharacterDataType.FeatureMedalValue]);
		short num = list[0];
		short num2 = list[1];
		short num3 = list[2];
		Refers component = totalMedal.transform.GetChild(0).GetComponent<Refers>();
		Refers component2 = totalMedal.transform.GetChild(1).GetComponent<Refers>();
		Refers component3 = totalMedal.transform.GetChild(2).GetComponent<Refers>();
		component.CGet<CImage>("Icon").SetSprite((num > 0) ? "sp_icon_renwutexing_10" : ((num < 0) ? "sp_icon_renwutexing_4" : "sp_icon_renwutexing_7"));
		component.CGet<TextMeshProUGUI>("Value").text = $" x{Mathf.Abs(num)}";
		component2.CGet<CImage>("Icon").SetSprite((num2 > 0) ? "sp_icon_renwutexing_9" : ((num2 < 0) ? "sp_icon_renwutexing_3" : "sp_icon_renwutexing_6"));
		component2.CGet<TextMeshProUGUI>("Value").text = $" x{Mathf.Abs(num2)}";
		component3.CGet<CImage>("Icon").SetSprite((num3 > 0) ? "sp_icon_renwutexing_11" : ((num3 < 0) ? "sp_icon_renwutexing_5" : "sp_icon_renwutexing_8"));
		component3.CGet<TextMeshProUGUI>("Value").text = $" x{Mathf.Abs(num3)}";
	}

	private void SetAttributeValue()
	{
		Traverse traverse = Traverse.Create(characterMajorAttribute);
		Traverse traverse2 = Traverse.Create(characterSecondaryAttribute);
		AttributeItem[] array = (AttributeItem[])traverse.Field("_mainAttributeItems").GetValue();
		AttributeItem[] array2 = (AttributeItem[])traverse.Field("_atkHitAttributeItems").GetValue();
		AttributeItem[] array3 = (AttributeItem[])traverse.Field("_atkPenetrabilityItems").GetValue();
		AttributeItem[] array4 = (AttributeItem[])traverse.Field("_defHitAttributeItems").GetValue();
		AttributeItem[] array5 = (AttributeItem[])traverse.Field("_defPenetrabilityItems").GetValue();
		AttributeSlider[] array6 = (AttributeSlider[])traverse2.Field("_attributeSliders").GetValue();
		short[] array7;
		short[] array8;
		short[] array9;
		short[] array10;
		short[] array11;
		short[] array12;
		if (characterDataController.characterDataShortDict.ContainsKey(CharacterDataType.MainAttribute))
		{
			array7 = characterDataController.characterDataShortDict[CharacterDataType.MainAttribute].ToArray();
			array8 = characterDataController.characterDataShortDict[CharacterDataType.AtkHitAttribute].ToArray();
			array9 = characterDataController.characterDataShortDict[CharacterDataType.DefHitAttribute].ToArray();
			array10 = characterDataController.characterDataShortDict[CharacterDataType.AtkPenetrability].ToArray();
			array11 = characterDataController.characterDataShortDict[CharacterDataType.DefPenetrability].ToArray();
			array12 = characterDataController.characterDataShortDict[CharacterDataType.SecondaryAttribute].ToArray();
		}
		else
		{
			array7 = new short[6];
			array8 = (array9 = new short[4]);
			array10 = (array11 = new short[2]);
			array12 = new short[10];
		}
		for (int i = 0; i < 6; i++)
		{
			array[i].UpdateValue(array7[i]);
		}
		for (int j = 0; j < 4; j++)
		{
			array2[j].UpdateValue(array8[j]);
			array4[j].UpdateValue(array9[j]);
		}
		for (int k = 0; k < 2; k++)
		{
			array3[k].UpdateValue(array10[k]);
			array5[k].UpdateValue(array11[k]);
		}
		for (int l = 0; l < 10; l++)
		{
			array6[l].Value = array12[l];
		}
	}

	private void SetLifeSkillBookValue()
	{
		if (!(_lifeSkillBookGo == null))
		{
			if (characterDataController.characterDataNameDict.ContainsKey(CharacterDataType.LifeSkillBookName))
			{
				string text = "<color=yellow>" + characterDataController.characterDataNameDict[CharacterDataType.LifeSkillBookName][0] + "</color>";
				_lifeSkillBookGo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
				string text2 = characterDataController.characterDataNameDict[CharacterDataType.LifeSkillBookType][0];
				//_lifeSkillBookGo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = text2;
			}
			else
			{
				_lifeSkillBookGo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "无";
				//_lifeSkillBookGo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "无";
			}
		}
	}

	private void SetCombatSkillBookValue()
	{
		if (_combatSkillBookGo == null)
		{
			return;
		}
		if (characterDataController.characterDataNameDict.ContainsKey(CharacterDataType.CombatSkillBookName))
		{
			string text = "<color=yellow>" + characterDataController.characterDataNameDict[CharacterDataType.CombatSkillBookName][0] + "</color>";
			_combatSkillBookGo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
			string text2 = characterDataController.characterDataNameDict[CharacterDataType.CombatSkillBookPageType][0];
			string text3 = text2;
			for (int i = 1; i < 6; i++)
			{
				string text4 = ((characterDataController.characterDataNameDict[CharacterDataType.CombatSkillBookPageType][i] == "正") ? "<color=#00ffffff>正</color>" : "<color=#ff0000ff>逆</color>");
				text3 = text3 + " " + text4;
			}
			//_combatSkillBookGo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = text3;
		}
		else
		{
			_combatSkillBookGo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "无";
			//_combatSkillBookGo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "无";
		}
	}

	private void ReSetPosition(float hight)
	{
		_currentPostion = new Vector2(_initialPostion.x, _currentPostion.y) - new Vector2(0f, hight) - new Vector2(0f, _gameObjectMargin.y);
	}

	public void Open()
	{
		_maskGo.SetActive(value: true);
		UpdateData();
	}

	public void Close()
	{
		_maskGo.SetActive(value: false);
	}

	public void SetRootCanvas(Canvas canvas)
	{
		_rootCanvas = canvas;
		_layer = canvas.gameObject;
	}
}
