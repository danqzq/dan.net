using UnityEngine;

namespace Dan.Net
{
    public class DanNetConfig : ScriptableObject
    {
        [Header("Connection Settings")]
        public string serverUrl = "localhost:3000";
        public bool isSecure = false;

        [Header("Stream Settings")]
        public float dataSendRate = 20;
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/DanNet/Show Config")]
        private static void ShowAsset()
        {
            var asset = Resources.Load<DanNetConfig>("DanNetConfig");
            if (asset == null)
            {
                CreateAsset();
            }
            
            UnityEditor.Selection.activeObject = asset;
        }
        
        [UnityEditor.MenuItem("Tools/DanNet/Create Config")]
        private static void CreateAsset()
        {
            if (Resources.Load<DanNetConfig>("DanNetConfig") != null)
            {
                return;
            }
            
            var asset = CreateInstance<DanNetConfig>();
            UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/Resources/DanNetConfig.asset");
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}