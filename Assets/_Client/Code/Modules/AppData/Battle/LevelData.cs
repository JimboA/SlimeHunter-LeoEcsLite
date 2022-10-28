using UnityEngine;

namespace Client.AppData
{
    [CreateAssetMenu(menuName = "Game/LevelData")]
    public class LevelData : ScriptableObject
    {
        public string Name; 
        public int Num; 
        public BoardData Board;
        public WinLoseConditions WinLose;
        public LevelScenario Scenario;
        public bool IsComplete;
    }
}