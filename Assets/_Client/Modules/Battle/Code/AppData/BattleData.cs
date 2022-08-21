using System;
using System.Collections.Generic;
using System.Diagnostics;
using Client.AppData.Blueprints;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Client.AppData
{
    [CreateAssetMenu(menuName = "Game/BattleData")]
    public class BattleData : ScriptableObject
    {
        [SerializeReference] public List<LevelData> levels;
        [SerializeField] public int currentLevelId;
        [SerializeField] private List<Blueprint> characters;
        public LevelData CurrentLevel => levels[currentLevelId];

        private Dictionary<int, int> _names = new Dictionary<int, int>();

        private void OnEnable()
        {
            for (int i = 0; i < characters.Count; i++)
            {
                var key = characters[i].name;
                DebugNoName(key);
                _names.Add(key.GetHashCode(), i);
            }
        }

        public bool TryGet(string name, out Blueprint blueprint)
        {
            var key = name.GetHashCode();

            if (_names.TryGetValue(key, out int index))
            {
                blueprint = characters[index];
                return true;
            }

            blueprint = default;
            return false;
        }

        [Conditional("DEBUG")]
        private void DebugNoName(string name)
        {
            if (name == String.Empty)
            {
                Debug.LogError($"Character has no name", this);
                throw new NullReferenceException();
            }
        }
    }
}
