using UnityEditor;
using UnityEngine;

namespace BoardEditor
{
    [CustomEditor(typeof(BoardCreator))]
    public class BoardCreatorInspector : UnityEditor.Editor
    {
        private BoardCreator _creator;

        private void OnEnable()
        {
            _creator = (BoardCreator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(5);
            if (GUILayout.Button("Save Board"))
            {
                _creator.GenerateBoard();
            }
        }
    }
}