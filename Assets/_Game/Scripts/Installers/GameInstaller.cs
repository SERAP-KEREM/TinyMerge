using _Main._Data;
using _Main._InputSystem;
using _Main._Management;
using UnityEngine;
using Zenject;

namespace _Game._Zenject
{
    /// <summary>
    /// Installer class for binding game managers and ScriptableObjects using Zenject.
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        #region Serialized Fields

        [Header("Managers")]
        [Tooltip("Manager responsible for handling items.")]
        [SerializeField]
        private ItemManager _itemManager;

        [Tooltip("Manager responsible for handling levels.")]
        [SerializeField]
        private LevelManager _levelManager;

        [Tooltip("Manager responsible for handling particles.")]
        [SerializeField]
        private ParticleManager _particleManager;

        [Tooltip("Manager responsible for handling tiles.")]
        [SerializeField]
        private TileManager _tileManager;

        [Tooltip("Manager responsible for handling special skills.")]
        [SerializeField]
        private SpecialSkillManager _specialSkillManager;

        [Tooltip("Manager responsible for handling audio.")]
        [SerializeField]
        private AudioManager _audioManager;

        [Header("Scriptable Objects")]
        [Tooltip("ScriptableObject storing player input data.")]
        [SerializeField]
        private PlayerInput _playerInput;

        [Tooltip("ScriptableObject storing global game data.")]
        [SerializeField]
        private GameData _gameDataAsset;

        #endregion

        #region Zenject Installation

        /// <summary>
        /// Installs all bindings required for the game.
        /// </summary>
        public override void InstallBindings()
        {
            InstallManagers();
            InstallScriptableObjects();
        }

        #endregion

        #region Manager Bindings

        /// <summary>
        /// Binds all game managers to the Zenject container.
        /// </summary>
        private void InstallManagers()
        {
            // Bind managers provided via inspector
            Container.Bind<ItemManager>()
                .FromInstance(_itemManager)
                .AsSingle();

            Container.Bind<LevelManager>()
                .FromInstance(_levelManager)
                .AsSingle();

            Container.Bind<ParticleManager>()
                .FromInstance(_particleManager)
                .AsSingle();

            Container.Bind<TileManager>()
                .FromInstance(_tileManager)
                .AsSingle();

            // Bind TimeManager as a new GameObject in the scene
            Container.BindInterfacesAndSelfTo<TimeManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("TimeManager")
                .AsSingle()
                .NonLazy();

            // Bind SpecialSkillManager as a new prefab instance
            Container.BindInterfacesAndSelfTo<SpecialSkillManager>()
                .FromComponentInNewPrefab(_specialSkillManager)
                .AsSingle()
                .NonLazy();

            // Bind UIManager from an existing component in the hierarchy
            Container.BindInterfacesAndSelfTo<UIManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            // Bind AudioManager as a new prefab instance
            Container.Bind<AudioManager>()
                .FromComponentInNewPrefab(_audioManager)
                .AsSingle()
                .NonLazy();
        }

        #endregion

        #region ScriptableObject Bindings

        /// <summary>
        /// Binds all ScriptableObjects to the Zenject container.
        /// </summary>
        private void InstallScriptableObjects()
        {
            // Bind PlayerInput ScriptableObject
            Container.Bind<PlayerInput>()
                .FromScriptableObject(_playerInput)
                .AsSingle();

            // Bind GameData ScriptableObject
            Container.Bind<GameData>()
                .FromScriptableObject(_gameDataAsset)
                .AsSingle();
        }

        #endregion
    }
}