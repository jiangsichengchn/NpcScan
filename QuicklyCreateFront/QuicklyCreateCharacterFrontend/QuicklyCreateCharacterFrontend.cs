using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
using UnityEngine;

namespace QuicklyCreateCharacterFrontend;

[PluginConfig("QuicklyCreateCharacter", "Senior kangbazi", "1.0.0")]
public class QuicklyCreateCharacterFrontend : TaiwuRemakePlugin
{
	private Harmony harmony;

	private static bool bool_Toggle_Total;

	private static bool bool_IsEnterNewGame;

	private static GameObject guideGo;

	private static GameObject UGUIGo;

	private static GameObject emptyGo;

	private static CharacterDataController dataController_Instance;

	private static UIController uiController_Instance;

	public override void OnModSettingUpdate()
	{
		ModManager.GetSetting(base.ModIdStr, "Toggle_Total", ref bool_Toggle_Total);
	}

	public override void Initialize()
	{
		harmony = Harmony.CreateAndPatchAll(typeof(QuicklyCreateCharacterFrontend));
		bool_IsEnterNewGame = false;
	}

	public override void Dispose()
	{
		if (harmony != null)
		{
			harmony.UnpatchSelf();
		}
		if (emptyGo != null)
		{
			Object.Destroy(emptyGo);
		}
		if (UGUIGo != null)
		{
			Object.Destroy(UGUIGo);
		}
		bool_IsEnterNewGame = false;
	}

	public override void OnLoadedArchiveData()
	{
		if (emptyGo != null)
		{
			Object.Destroy(emptyGo);
		}
		if (UGUIGo != null)
		{
			Object.Destroy(UGUIGo);
		}
		bool_IsEnterNewGame = false;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(UI_NewGame), "Awake")]
	public static void UI_NewGame_Awake_PostPatch(UI_NewGame __instance)
	{
		if (bool_Toggle_Total)
		{
			bool_IsEnterNewGame = true;
			CToggleGroup cToggleGroup = __instance.CGet<CToggleGroup>("SwitchMode");
			Canvas componentInParent = cToggleGroup.transform.GetComponentInParent<Canvas>();
			UGUIGo = new GameObject("mainWindowGoForQCCF");
			dataController_Instance = UGUIGo.AddComponent<CharacterDataController>();
			dataController_Instance.UI_NewGame_Member = __instance;
			RollAttributeWindow rollAttributeWindow = UGUIGo.AddComponent<RollAttributeWindow>();
			rollAttributeWindow.characterDataController = dataController_Instance;
			rollAttributeWindow.SetRootCanvas(componentInParent);
			guideGo = UIFactory.GetCommonButtonGo("人物属性", rollAttributeWindow.Open, hasIcon: false);
			guideGo.transform.SetParent(cToggleGroup.transform, worldPositionStays: false);
			Vector2 sizeDelta = guideGo.transform.GetComponent<RectTransform>().sizeDelta;
			Vector2 vector = new Vector2(500f, 500f) + sizeDelta / 2f - new Vector2(0f, sizeDelta.y);
			guideGo.transform.localPosition = Vector3.zero;
			guideGo.name = "guideGoForQCCF";
			bool flag = false;
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(UI_NewGame), "UpdateScrolls")]
	public static void UI_NewGame_UpdateScrolls_PostPatch(UI_NewGame __instance)
	{
		if (bool_Toggle_Total && bool_IsEnterNewGame)
		{
			dataController_Instance.DoRollCharacterData();
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(UI_NewGame), "OnClickOpenInscriptionWindow")]
	public static void UI_NewGame_OnClickOpenInscriptionWindow_PostPatch(UI_NewGame __instance)
	{
		if (bool_Toggle_Total && bool_IsEnterNewGame)
		{
			if (emptyGo != null)
			{
				Object.Destroy(emptyGo);
			}
			if (UGUIGo != null)
			{
				Object.Destroy(UGUIGo);
			}
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(UI_NewGame), "OnDestroy")]
	public static bool UI_NewGame_OnLoadFinish_PrePatch(UI_NewGame __instance)
	{
		if (!bool_Toggle_Total)
		{
			return true;
		}
		if (!bool_IsEnterNewGame)
		{
			return true;
		}
		if (emptyGo != null)
		{
			Object.Destroy(emptyGo);
		}
		if (UGUIGo != null)
		{
			Object.Destroy(UGUIGo);
		}
		return true;
	}
}
