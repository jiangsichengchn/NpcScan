using TaiwuModdingLib.Core.Plugin;
using HarmonyLib;
using GameData.Domains.Character;
using GameData.Domains;
using GameData.Domains.Global.Inscription;
using GameData.Utilities;

namespace InscribeTaiwuBackend
{
    [PluginConfig("InscribeTaiwuBackend", "发射的熟鸡蛋", "1.0")]
    public class InscribeTaiwuBackend : TaiwuRemakePlugin
    {
        private Harmony harmony;

        public override void Dispose()
        {
            harmony.UnpatchSelf();
        }

        public override void Initialize()
        {
            harmony = Harmony.CreateAndPatchAll(typeof(InscribeTaiwuBackend));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterDomain), "GetInscriptionStatus")]
        public static void GetInscriptionStatus_Postfix(ref sbyte __result, int charId)
        {
            int taiwuCharId = DomainManager.Taiwu.GetTaiwuCharId();
            if (charId == taiwuCharId)
            {
                uint worldId = DomainManager.World.GetWorldId();
                InscribedCharacterKey key = new InscribedCharacterKey(worldId, charId);
                __result = (sbyte)((!DomainManager.Global.IsCharacterInscribed(key)) ? 1 : 2);
            }
        }
    }
}
