using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
using UnityEngine;

namespace InscribeTaiwu
{
    [PluginConfig("InscribeTaiwu", "发射的熟鸡蛋", "1.0")]
    public class InscribeTaiwu : TaiwuRemakePlugin
    {
        private Harmony harmony;

        public override void Dispose()
        {
            harmony.UnpatchSelf();
        }

        public override void Initialize()
        {
            harmony = Harmony.CreateAndPatchAll(typeof(InscribeTaiwu));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UI_CharacterMenuInfo), "OnCurrentCharacterChange")]
        public static void OnCurrentCharacterChange_Postfix(UI_CharacterMenuInfo __instance)
        {
            int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
            bool isTaiwu = __instance.CharacterMenu.CurCharacterId == taiwuCharId;
            if (isTaiwu)
            {
                CButton inscribeBtn = __instance.CGet<CButton>("InscribeBtn");
                inscribeBtn.gameObject.SetActive(true);
                inscribeBtn.interactable = true;

                Refers btnRefers = inscribeBtn.GetComponent<Refers>();
                btnRefers.CGet<GameObject>("Label").SetActive(inscribeBtn.interactable);
                btnRefers.CGet<GameObject>("LabelDisable").SetActive(!inscribeBtn.interactable);
                btnRefers.CGet<CImage>("Icon").SetSprite(inscribeBtn.interactable ? "charactermenu3_01_gn_icon_1_0" : "charactermenu3_01_gn_icon_1_1");
            }
        }
    }
}
