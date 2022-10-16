using GameData.Serializer;
using HarmonyLib;
using Newtonsoft.Json;
using NpcScan.Controller;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiwuModdingLib.Core.Plugin;
using UnityEngine;

namespace NpcScan
{
    [PluginConfig("NPCScan", "发射的熟鸡蛋", "1.0")]
    public class MainEntry : TaiwuRemakePlugin
    {
        private GameObject gameObject;

        public override void Dispose()
        {
            if (gameObject != null)
                UnityEngine.Object.Destroy(gameObject);
            File.Delete("NpcScanData.json");
        }

        public override void Initialize()
        {
            if (!ABResourceManager.IsInit)
                ABResourceManager.Init(ModIdStr);
            if (gameObject == null)
            {
                gameObject = new GameObject("NpcScanEmpty");
                gameObject.AddComponent<NpcScanController>();
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                BindKeyCode();
            }                     
        }

        public override void OnModSettingUpdate()
        {
            BindKeyCode();
        }

        public void BindKeyCode()
        {
            NpcScanController controller = gameObject.GetComponent<NpcScanController>();
            string mainFormKey = "F3";
            if (ModManager.GetSetting(ModIdStr, "MainFormKey", ref mainFormKey))
            {
                if (Enum.TryParse(mainFormKey, out KeyCode keycode))
                {
                    controller.mainFormKey = keycode;
                }
            }
        }
    }
}
