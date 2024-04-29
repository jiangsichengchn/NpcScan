using FrameWork;
using GameData.GameDataBridge;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RandomReadingStrategies;

[PluginConfig("RandomReadingStrategies", "宇文述雪", "1.0.3")]
public class RandomReadingStrategies : TaiwuRemakePlugin
{
	private Harmony _harmony;

	private static bool needInitValue;

	public static bool nextTimeAddEvent;

	public static bool everyTimeAddEvent;

	public override void Initialize()
	{
		_harmony = new Harmony(base.ModIdStr);
		_harmony.PatchAll(typeof(RandomReadingStrategies));
		//GEvent.Add(UiEvents.OnUIElementHide, OnUIElementShow);
		//GEvent.Add(UiEvents.TopUiChanged, OnUIElementHide);
		nextTimeAddEvent = PlayerPrefs.GetInt("NextTimeReadEvent", 0) == 1;
		everyTimeAddEvent = PlayerPrefs.GetInt("EveryTimeReadEvent", 0) == 1;
		needInitValue = true;
	}

	public override void Dispose()
	{
		if (_harmony != null)
		{
			_harmony.UnpatchSelf();
			_harmony = null;
		}
		//GEvent.Remove(UiEvents.OnUIElementHide, OnUIElementShow);
		//GEvent.Remove(UiEvents.TopUiChanged, OnUIElementHide);
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(UIManager), "ShowUI")]
	public static void ShowUI_Postfix(UIManager __instance, UIElement elem)
	{
        if ("UI_ReadingEvent".Equals(elem.Name))
        {
            UI_ReadingEvent uiInstance2 = elem.UiBaseAs<UI_ReadingEvent>();
            if (uiInstance2 != null)
            {
                Debug.Log("random 4");
                GameObject gameObject = Object.Instantiate(uiInstance2.transform.Find("MainWindow/StrategyHolder/StrategyToggle_0/Bg").gameObject);
                gameObject.name = "RandomBtn";
                Image img2 = gameObject.GetComponent<Image>();
                img2.raycastTarget = true;
                CButton RandomBtn = gameObject.AddComponent<CButton>();
                GameObject go2 = Object.Instantiate(uiInstance2.transform.Find("MainWindow/StrategyHolder/StrategyToggle_0/Name").gameObject);
                go2.name = "Name";
                go2.transform.SetParent(gameObject.transform, worldPositionStays: false);
                TextMeshProUGUI text = go2.GetComponent<TextMeshProUGUI>();
                text.text = "灵光\n再闪";
                gameObject.transform.SetParent(uiInstance2.transform.Find("MainWindow/StrategyHolder"), worldPositionStays: false);
                gameObject.transform.localPosition = new Vector3(0f, 100f, 0f);
                RandomBtn.onClick.AddListener(delegate
                {
                    GameDataBridge.AddMethodCall(uiInstance2.Element.GameDataListenerId, 5, 33, Traverse.Create(uiInstance2).Field("_curPage").GetValue<byte>());
                });
            }
        }
    }

	public static void OnUIElementShow(ArgumentBox argbox)
	{
		Debug.Log("random 1");
        if (argbox == null)
            return;
        Debug.Log("random 2");
        if (!argbox.Get("Element", out UIElement uielement))
		{
			return;
		}
        Debug.Log("random 3");
        if (needInitValue)
		{
			needInitValue = false;
			SetValueToBackend();
		}
        Debug.Log("random " + uielement.Name);
        if ("UI_ReadingEvent".Equals(uielement.Name))
		{
			UI_ReadingEvent uiInstance2 = uielement.UiBaseAs<UI_ReadingEvent>();
			if (uiInstance2 != null)
			{
                Debug.Log("random 4");
                GameObject gameObject = Object.Instantiate(uiInstance2.transform.Find("MainWindow/StrategyHolder/StrategyToggle_0/Bg").gameObject);
				gameObject.name = "RandomBtn";
				Image img2 = gameObject.GetComponent<Image>();
				img2.raycastTarget = true;
				CButton RandomBtn = gameObject.AddComponent<CButton>();
				GameObject go2 = Object.Instantiate(uiInstance2.transform.Find("MainWindow/StrategyHolder/StrategyToggle_0/Name").gameObject);
				go2.name = "Name";
				go2.transform.SetParent(gameObject.transform, worldPositionStays: false);
				TextMeshProUGUI text = go2.GetComponent<TextMeshProUGUI>();
				text.text = "灵光\n再闪";
				gameObject.transform.SetParent(uiInstance2.transform.Find("MainWindow/StrategyHolder"), worldPositionStays: false);
				gameObject.transform.localPosition = new Vector3(0f, 100f, 0f);
				RandomBtn.onClick.AddListener(delegate
				{
					GameDataBridge.AddMethodCall(uiInstance2.Element.GameDataListenerId, 5, 33, Traverse.Create(uiInstance2).Field("_curPage").GetValue<byte>());
				});
			}
		}
		else if ("UI_Reading".Equals(uielement.Name))
		{
			UI_Reading uiInstance = uielement.UiBaseAs<UI_Reading>();
			if (uiInstance != null)
			{
				TextMeshProUGUI txtPrefab = uiInstance.transform.Find("MainWindow/Background/TitleImg/ImgTitle36/Title").GetComponent<TextMeshProUGUI>();
				GameObject btnGo = new GameObject("ToggleNext");
				RectTransform rectTran = btnGo.AddComponent<RectTransform>();
				Transform root = uiInstance.transform.Find("MainWindow");
				rectTran.SetParent(root, worldPositionStays: false);
				rectTran.anchorMin = new Vector2(0.5f, 0f);
				rectTran.anchorMax = new Vector2(0.5f, 0f);
				rectTran.sizeDelta = new Vector2(50f, 50f);
				rectTran.anchoredPosition = new Vector2(-250f, 60f);
				Image img = btnGo.AddComponent<Image>();
				img.raycastTarget = true;
				img.color = Color.gray;
				Toggle tog = btnGo.AddComponent<Toggle>();
				tog.targetGraphic = img;
				GameObject go = new GameObject("Select");
				rectTran = go.AddComponent<RectTransform>();
				rectTran.SetParent(btnGo.transform, worldPositionStays: false);
				rectTran.sizeDelta = new Vector2(46f, 46f);
				img = go.AddComponent<Image>();
				img.color = Color.green;
				img.raycastTarget = true;
				tog.graphic = img;
				tog.isOn = nextTimeAddEvent;
				go = new GameObject("Txt");
				TextMeshProUGUI txt = go.AddComponent<TextMeshProUGUI>();
				txt.font = txtPrefab.font;
				txt.text = "下次必闪";
				txt.alignment = TextAlignmentOptions.MidlineLeft;
				rectTran = go.GetComponent<RectTransform>();
				rectTran.SetParent(btnGo.transform, worldPositionStays: false);
				rectTran.sizeDelta = new Vector2(200f, 50f);
				rectTran.pivot = new Vector2(0f, 0.5f);
				rectTran.anchoredPosition = new Vector2(30f, 0f);
				tog.onValueChanged.AddListener(delegate(bool isOn)
				{
					PlayerPrefs.SetInt("NextTimeReadEvent", isOn ? 1 : 0);
					nextTimeAddEvent = isOn;
				});
				btnGo = new GameObject("ToggleNext");
				rectTran = btnGo.AddComponent<RectTransform>();
				rectTran.SetParent(root, worldPositionStays: false);
				rectTran.anchorMin = new Vector2(0.5f, 0f);
				rectTran.anchorMax = new Vector2(0.5f, 0f);
				rectTran.sizeDelta = new Vector2(50f, 50f);
				rectTran.anchoredPosition = new Vector2(50f, 60f);
				img = btnGo.AddComponent<Image>();
				img.raycastTarget = true;
				img.color = Color.gray;
				tog = btnGo.AddComponent<Toggle>();
				tog.targetGraphic = img;
				go = new GameObject("Select");
				rectTran = go.AddComponent<RectTransform>();
				rectTran.SetParent(btnGo.transform, worldPositionStays: false);
				rectTran.sizeDelta = new Vector2(46f, 46f);
				img = go.AddComponent<Image>();
				img.color = Color.green;
				img.raycastTarget = true;
				tog.graphic = img;
				tog.isOn = everyTimeAddEvent;
				go = new GameObject("Txt");
				txt = go.AddComponent<TextMeshProUGUI>();
				txt.font = txtPrefab.font;
				txt.text = "次次必闪";
				txt.alignment = TextAlignmentOptions.MidlineLeft;
				rectTran = go.GetComponent<RectTransform>();
				rectTran.SetParent(btnGo.transform, worldPositionStays: false);
				rectTran.sizeDelta = new Vector2(200f, 50f);
				rectTran.pivot = new Vector2(0f, 0.5f);
				rectTran.anchoredPosition = new Vector2(30f, 0f);
				tog.onValueChanged.AddListener(delegate(bool isOn)
				{
					PlayerPrefs.SetInt("EveryTimeReadEvent", isOn ? 1 : 0);
					everyTimeAddEvent = isOn;
				});
			}
		}
		else if ("UI_MonthNotify".Equals(uielement.Name) && nextTimeAddEvent)
		{
			nextTimeAddEvent = false;
			PlayerPrefs.SetInt("NextTimeReadEvent", 0);
		}
	}

	public static void OnUIElementHide(ArgumentBox argbox)
	{
		if (argbox == null)
			return;
		if (argbox.Get("Element", out UIElement uielement) && "UI_Reading".Equals(uielement.Name))
		{
			SetValueToBackend();
		}
	}

	public static void SetValueToBackend()
	{
		GameDataBridge.AddMethodCall(-1, 528, 1, nextTimeAddEvent);
		GameDataBridge.AddMethodCall(-1, 528, 2, everyTimeAddEvent);
	}
}
