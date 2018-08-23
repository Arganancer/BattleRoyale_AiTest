using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace Playmode.Util.Information
{
    public class Comment : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private string text;
#endif
    }

#if UNITY_EDITOR

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Comment))]
    public class CommentEditor : Editor
    {
        private SerializedProperty text;

        private void OnEnable()
        {
            text = serializedObject.FindProperty("text");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            text.stringValue = EditorGUILayout.TextArea(text.stringValue, GUILayout.MaxHeight(75));
            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}