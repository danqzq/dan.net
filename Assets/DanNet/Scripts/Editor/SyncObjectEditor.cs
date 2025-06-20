using Dan.Net;
using UnityEditor;

namespace DanEditor
{
    [CustomEditor(typeof(SyncObject))]
    public class SyncObjectEditor : Editor
    {
        private SerializedProperty _id;

        private void OnEnable() 
        {
            _id = serializedObject.FindProperty("ID");
        }
        
        public override void OnInspectorGUI()
        {
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                EditorGUILayout.PropertyField(_id);
            }
            else
            {
                EditorGUILayout.LabelField("ID", "set at runtime");
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}