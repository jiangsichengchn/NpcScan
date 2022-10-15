using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace NpcScan
{
    /// <summary>
    /// AB包管理器
    /// </summary>
    public static class ABResourceManager
    {
        /// <summary>
        /// 加载的ModAB包，由于在为单个Mod使用，所以不需要考虑AB包的依赖关系
        /// </summary>
        private static AssetBundle _assetBundle = null;
        
        private static bool _isInit = false;
        private static bool _isStartInit = false;
        
        private static CancellationTokenSource _token;

        public static AssetBundle AssetBundle => _assetBundle;
        public static bool IsInit => _isInit;
        public static string modPath;

        public static void Init(string modId)
        {
            if(_isInit || _isStartInit)
                throw new Exception("ABResourceManager已经初始化过了");
            
            
            var index = ModManager.EnabledMods.FindIndex(mod => mod.ToString() == modId);
            if (index == -1)
            {
                throw new Exception($"加载AB包失败，Mod {modId} 不存在或未启用！");
            }
            var curModInfo = ModManager.GetModInfo(ModManager.EnabledMods[index]);
            var modDir = curModInfo.DirectoryName;
            modPath = modDir;
            var abPath = Path.Combine(modDir, "Bundle/asset.ab");
            
            if(!File.Exists(abPath))
                throw new Exception($"加载AB包失败，AB包：{abPath} 不存在！");
            
            _assetBundle = AssetBundle.LoadFromFile(abPath);
            
            _isInit = true;
        }
        
        public static void InitAsync(string modId, Action callback)
        {
            if(_isInit || _isStartInit)
                throw new Exception("ABResourceManager已经初始化过了");
            
            _isStartInit = true;
            
            var index = ModManager.EnabledMods.FindIndex(mod => mod.ToString() == modId);
            if (index == -1)
            {
                throw new Exception($"加载AB包失败，Mod {modId} 不存在或未启用！");
            }
            var curModInfo = ModManager.GetModInfo(ModManager.EnabledMods[index]);
            var modDir = curModInfo.DirectoryName;
            var abPath = Path.Combine(modDir, "Bundle/asset.ab");
            
            _token = new CancellationTokenSource();
            
            var token = _token;
            AssetBundle.LoadFromFileAsync(abPath).completed += operation =>
            {
                var ab = (operation as AssetBundleCreateRequest).assetBundle;
                
                if(token.IsCancellationRequested)
                {
                    ab.Unload(true);
                    return;
                }
                
                _assetBundle = ab;
                
                _isStartInit = false;
                _isInit = true;
                callback?.Invoke();
            };
        }
        
        public static void UnInit()
        {
            _isInit = false;
            _isStartInit = false;
            if (_token != null)
            {
                _token.Cancel();
                _token = null;
            }
            if (_assetBundle != null)
            {
                _assetBundle.Unload(true);
            }
            _assetBundle = null;
        }

        public static T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            if (!_isInit)
            {
                throw new Exception("AB包未加载！");
            }
            return _assetBundle.LoadAsset<T>(assetName);
        }
        
        public static IEnumerable<T> LoadAssets<T>(string assetName) where T : UnityEngine.Object
        {
            if (!_isInit)
            {
                throw new Exception("AB包未加载！");
            }
            return _assetBundle.LoadAssetWithSubAssets<T>(assetName);
        }

        public static void LoadAssetAsync<T>(string assetName, Action<T> callback) where T : UnityEngine.Object
        {
            if (!_isInit)
            {
                throw new Exception("AB包未加载！");
            }
            _assetBundle.LoadAssetAsync<T>(assetName).completed += operation =>
            {
                callback?.Invoke((operation as AssetBundleRequest).asset as T);
            };
        }
        
        public static void LoadAssetsAsync<T>(string assetName, Action<IEnumerable<T>> callback) where T : UnityEngine.Object
        {
            if (!_isInit)
            {
                throw new Exception("AB包未加载！");
            }
            _assetBundle.LoadAssetWithSubAssetsAsync<T>(assetName).completed += operation =>
            {
                callback?.Invoke((operation as AssetBundleRequest).allAssets as IEnumerable<T>);
            };
        }
        
        public static GameObject Instantiate(string assetName)
        {
            if (!_isInit)
            {
                throw new Exception("AB包未加载！");
            }
            return GameObject.Instantiate(_assetBundle.LoadAsset<GameObject>(assetName));
        }
        
        public static GameObject LoadGameObject(string assetName)
        {
            if (!_isInit)
            {
                throw new Exception("AB包未加载！");
            }
            return _assetBundle.LoadAsset<GameObject>(assetName);
        }

        public static void LoadGameObjectAsync(string assetName, Action<GameObject> callback)
        {
            if (!_isInit)
            {
                throw new Exception("AB包未加载！");
            }
            _assetBundle.LoadAssetAsync<GameObject>(assetName).completed += operation =>
            {
                var go = (operation as AssetBundleRequest).asset as GameObject;
                callback?.Invoke(go);
            };
        }
        
        public static bool ContainsAsset(string assetName)
        {
            if (!_isInit)
            {
                throw new Exception("AB包未加载！");
            }
            return _assetBundle.Contains(assetName);
        }
    }
}