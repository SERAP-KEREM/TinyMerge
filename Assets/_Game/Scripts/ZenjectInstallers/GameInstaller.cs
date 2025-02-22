using _Main._Data;
using _Main._InputSystem;
using _Main._Management;
using TriInspector;
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
        [SerializeField, PropertyTooltip("Manager responsible for handling items.")]
        private ItemManager _itemManager;

        [SerializeField, PropertyTooltip("Manager responsible for handling levels.")]
        private LevelManager _levelManager;

        [SerializeField, PropertyTooltip("Manager responsible for handling particles.")]
        private ParticleManager _particleManager;

        [SerializeField, PropertyTooltip("Manager responsible for handling tiles.")]
        private TileManager _tileManager;

        [SerializeField, PropertyTooltip("Manager responsible for handling special skills.")]
        private SpecialSkillManager _specialSkillManager;

        [SerializeField, PropertyTooltip("Manager responsible for handling audio.")]
        private AudioManager _audioManager;

        [Header("Scriptable Objects")]
        [SerializeField, PropertyTooltip("ScriptableObject storing player input data.")]
        private PlayerInput _playerInput;

        [SerializeField, PropertyTooltip("ScriptableObject storing global game data.")]
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
            Container.Bind<ItemManager>().FromInstance(_itemManager).AsSingle();
            Container.Bind<LevelManager>().FromInstance(_levelManager).AsSingle();
            Container.Bind<ParticleManager>().FromInstance(_particleManager).AsSingle();
            Container.Bind<TileManager>().FromInstance(_tileManager).AsSingle();

            // Bind TimeManager as a new GameObject in the scene
            Container.BindInterfacesAndSelfTo<TimeManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("TimeManager")
                .AsSingle()
                .NonLazy();

            // Bind SpecialSkillManager from an existing component in the hierarchy
            Container.Bind<SpecialSkillManager>().FromInstance(_specialSkillManager).AsSingle();

            // Bind UIManager from an existing component in the hierarchy
            Container.Bind<UIManager>().FromComponentInHierarchy().AsSingle();

            // Bind AudioManager from an existing component in the hierarchy
            Container.Bind<AudioManager>().FromInstance(_audioManager).AsSingle();
        }

        #endregion

        #region ScriptableObject Bindings

        /// <summary>
        /// Binds all ScriptableObjects to the Zenject container.
        /// </summary>
        private void InstallScriptableObjects()
        {
            // Bind PlayerInput ScriptableObject
            Container.Bind<PlayerInput>().FromScriptableObject(_playerInput).AsSingle();

            // Bind GameData ScriptableObject
            Container.Bind<GameData>().FromScriptableObject(_gameDataAsset).AsSingle();
        }

        #endregion
    }
}