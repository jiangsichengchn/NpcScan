using GameData.Common;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using HarmonyLib;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaiwuModdingLib.Core.Plugin;
using System.IO;
using System.Text;
using GameData.Domains.Taiwu;
using GameData.Domains;
using GameData.Domains.Character;
using TaiwuModdingLib.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using GameData.Domains.Map;
using System.IO.MemoryMappedFiles;

namespace NpcScan
{
    [PluginConfig("NPCScanBckend", "发射的熟鸡蛋", "1.0")]
    public class NPCScanBackend : TaiwuRemakePlugin
    {
        private Harmony harmony;
        
        public override void Dispose()
        {
            harmony.UnpatchSelf();
        }

        public override void Initialize()
        {
            harmony = Harmony.CreateAndPatchAll(typeof(NPCScanBackend));
            Init();
        }

        public override void OnLoadedArchiveData()
        {
            base.OnLoadedArchiveData();
            if (memoryMappedFile != null)
            {
                memoryMappedFile.Dispose();
                memoryMappedFile = null;
            }           
        }

        private void Init()
        {           
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameDataBridge), "ProcessMethodCall")]
        public static bool ReturnCharactersData(Operation operation, RawDataPool argDataPool, DataContext context, NotificationCollection ____pendingNotifications)
        {
            if (operation.DomainId == 97)
            {               
                int returnOffset = GetCharactersData(operation, argDataPool, ____pendingNotifications.DataPool, context);
                if (returnOffset >= 0)
                    ____pendingNotifications.Notifications.Add(Notification.CreateMethodReturn(operation.ListenerId, operation.DomainId, operation.MethodId, returnOffset));
                return false;
            }
            return true;
        }

        private static MemoryMappedFile memoryMappedFile;

        private static int GetCharactersData(Operation operation, RawDataPool argDataPool, RawDataPool returnDataPool, DataContext context)
        {
            Dictionary<int, Character> AliveCharacterDic = (Dictionary<int, Character>)Traverse.Create(DomainManager.Character).Field("_objects").GetValue();
            Dictionary<int, DeadCharacter> DeadCharacterDic = (Dictionary<int, DeadCharacter>)Traverse.Create(DomainManager.Character).Field("_deadCharacters").GetValue();           
            Dictionary<int, Grave> GraveDic = (Dictionary<int, Grave>)Traverse.Create(DomainManager.Character).Field("_graves").GetValue();           
            List<CharacterData> characterDataList = new List<CharacterData>(AliveCharacterDic.Count + DeadCharacterDic.Count);

            foreach (var character in AliveCharacterDic.Values)
            {
                CharacterData characterData = new CharacterData();
                characterData.SetData(character);

                if (characterData.creatingType != 2 && characterData.creatingType != 3)
                    characterDataList.Add(characterData);
            }
            foreach (var (id, character) in DeadCharacterDic)
            {
                CharacterData characterData = new CharacterData();
                Location lcoation = new Location(-1, -1);
                if (GraveDic.ContainsKey(id))
                    lcoation = GraveDic[id].GetLocation();
                characterData.SetDeadData(id, character, lcoation);

                characterDataList.Add(characterData);
            }

            var options = new JsonSerializerOptions();
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string jsonContent = JsonSerializer.Serialize(characterDataList, options);
            byte[] data = Encoding.Unicode.GetBytes(jsonContent);

            if (memoryMappedFile == null)
                memoryMappedFile = MemoryMappedFile.CreateOrOpen("NpcScanData", data.LongLength);            
            using (var accessor = memoryMappedFile.CreateViewAccessor())
            {
                accessor.WriteArray(0, data, 0, data.Length);
            }

            //File.WriteAllText("../NpcScanData.json", JsonSerializer.Serialize(characterDataList, options));

            int num = 0;

            return num;
        }
    }
}
