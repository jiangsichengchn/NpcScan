using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.Linq;
using GameData.Domains.Organization.Display;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
using TaiwuModdingLib.Core.Utils;
using TMPro;
using UnityEngine;

namespace ExchangeBook;

[PluginConfig("ExchangeBookPlus", "p", "1.0")]
public class MainPatch : TaiwuRemakeHarmonyPlugin
{
	private static short SettlementId;

	private static bool OnlyInSect;

	private static CButton exchangeCombatSkillBookBtn;

	private static CButton exchangeLifeSkillBookBtn;

	public override void OnModSettingUpdate()
	{
		ModManager.GetSetting(base.ModIdStr, "OnlyInSect", ref OnlyInSect);
	}

	public override void Initialize()
	{
		base.HarmonyInstance.PatchAll(typeof(MainPatch));
		UIBuilder.PrepareMaterial();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(UI_SettlementInformation), "OnInit")]
	public static void UI_SettlementInformation_OnInit_Postfix(ArgumentBox argsBox, UI_SettlementInformation __instance)
	{
		if (exchangeCombatSkillBookBtn == null)
		{
			GameObject gameObject = __instance.transform.Find("AnimationRoot/BackGround/BackPanel/Supprot/ShowCombatSkillTree/").gameObject;
			GameObject gameObject2 = Object.Instantiate(gameObject, gameObject.transform.parent);
			gameObject2.GetComponent<RectTransform>().anchoredPosition = new Vector2(300f, 16.8f);
            gameObject2.GetComponentInChildren<TextMeshProUGUI>().SetCharArray("门派换书".ToCharArray());
            gameObject2.GetComponentInChildren<TextMeshProUGUI>().SetAllDirty();
            exchangeCombatSkillBookBtn = gameObject2.GetComponent<CButton>();
			exchangeCombatSkillBookBtn.name = "ExchangeCombatSkillBook";
			exchangeCombatSkillBookBtn.ClearAndAddListener(delegate
			{
				OnClick(__instance, isCombatSkill: true);
			});

            gameObject2.SetActive(!OnlyInSect);
		}
		if (exchangeLifeSkillBookBtn == null)
		{
			GameObject gameObject3 = __instance.transform.Find("AnimationRoot/BackGround/BackPanel/Supprot/ShowCombatSkillTree/").gameObject;
			GameObject gameObject4 = Object.Instantiate(gameObject3, gameObject3.transform.parent);
			gameObject4.GetComponent<RectTransform>().anchoredPosition = new Vector2(550f, 16.8f);
            gameObject4.GetComponentInChildren<TextMeshProUGUI>().SetCharArray("技艺换书".ToCharArray());
            gameObject4.GetComponentInChildren<TextMeshProUGUI>().SetAllDirty();
            exchangeLifeSkillBookBtn = gameObject4.GetComponent<CButton>();
			exchangeLifeSkillBookBtn.name = "ExchangeLifeSkillBook";
			exchangeLifeSkillBookBtn.ClearAndAddListener(delegate
			{
				OnClick(__instance, isCombatSkill: false);
			});
			gameObject4.SetActive(!OnlyInSect);
		}
		if (OnlyInSect)
		{
			SettlementId = 0;
			argsBox?.Get("SettlementId", out SettlementId);
			if (SettlementId != 0)
			{
				exchangeCombatSkillBookBtn?.gameObject.SetActive(value: true);
				exchangeLifeSkillBookBtn?.gameObject.SetActive(value: true);
			}
		}
	}

	public static void OnClick(UI_SettlementInformation instance, bool isCombatSkill)
	{
		int curSettlementInDisplay = (int)instance.GetFieldValue("_curSettlementInDisplay");
		if (curSettlementInDisplay != -1)
		{
			List<SettlementDisplayData> enumerable = (List<SettlementDisplayData>)instance.GetFieldValue("_visitedSettlements");
			SettlementDisplayData settlementDisplayData = enumerable.First((SettlementDisplayData data) => data.SettlementId == curSettlementInDisplay);
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.Set("OrganizationId", curSettlementInDisplay);
			argumentBox.Set("OrganizationName", Organization.Instance[settlementDisplayData.OrgTemplateId].Name);
			argumentBox.Set("IsCombatSkill", isCombatSkill);
			UI_ExchangeBookPlus.GetUI().SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UI_ExchangeBookPlus.GetUI());
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(UI_SettlementInformation), "OnClickSettlement")]
	public static void UI_SettlementInformation_OnClickSettlement_Postfix(int ____curSettlementInDisplay)
	{
		if (OnlyInSect)
		{
			exchangeCombatSkillBookBtn?.gameObject.SetActive(____curSettlementInDisplay == SettlementId);
			exchangeLifeSkillBookBtn?.gameObject.SetActive(____curSettlementInDisplay == SettlementId);
		}
	}
}
