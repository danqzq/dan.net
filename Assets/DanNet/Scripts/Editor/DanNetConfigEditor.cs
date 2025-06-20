using Dan.Net;
using UnityEditor;

namespace DanEditor
{
    [CustomEditor(typeof(DanNetConfig))]
    public class DanNetConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var serverUrl = serializedObject.FindProperty("serverUrl");
            if (serverUrl.stringValue.Contains("://"))
            {
                EditorGUILayout.HelpBox("The server URL should not contain a protocol (e.g., http:// or https://). Please remove it.", MessageType.Warning);
            }
            base.OnInspectorGUI();
        }
    }
}