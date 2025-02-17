using UnityEngine;
using Zenject;

namespace _Main.Data
{
    public class GameData : IInitializable
    {
        private readonly LevelConfig _levelConfig;

        public LevelConfig CurrentLevel => _levelConfig;

        [Inject]
        public GameData(LevelConfig levelConfig)
        {
            _levelConfig = levelConfig;
        }

        public void Initialize()
        {
            Debug.Log($"GameData initialized with level: {_levelConfig.name}");
        }
    }
}