using System;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace BoardEditor
{
    [CustomGridBrush(false, false, false, "BattleBoardBrush")]
    public class BoardBrush : GridBrush
    {
        [SerializeField] private BoardCreator _creator;

        private void OnEnable()
        {
            if (_creator == null)
                _creator = FindObjectOfType<BoardCreator>();
            
            if(_creator == null)
                Debug.LogError($"Can't find BoardCreator script on scene: {SceneManager.GetActiveScene().name}");
        }
        
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            if (!_creator.CheckBoardBounds(position))
                return;

            base.Paint(gridLayout, brushTarget, position);
        }
    }
    
    [CustomEditor(typeof(BoardBrush))]
    public class SlimeHunterBrushDrawer : GridBrushEditor
    {
        public void OnEnable()
        {
            base.OnEnable();
            Selection.selectionChanged += Repaint;
        }

        public void OnDisable()
        {
            base.OnDisable();
            Selection.selectionChanged -= Repaint;
        }
    }
}