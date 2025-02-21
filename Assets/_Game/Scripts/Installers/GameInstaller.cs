using UnityEngine;
using Zenject;
using _Game.Scripts.Management;
using _Game.Scripts._helpers;
using _Game.Scripts.Data;
using _Game.Scripts.Items;
using _Game.Scripts.Services;
using _Game.Scripts.UI;

public class GameInstaller : MonoInstaller
{
    [Header("Managers")]
    [SerializeField] private ItemManager _itemManager;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private ParticleManager _particleManager;
    [SerializeField] private TileManager _tileManager;
    [SerializeField] private SpecialSkillManager _specialSkillManager;

    [Header("Scriptable Objects")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameData _gameDataAsset;

    public override void InstallBindings()
    {
        InstallManagers();
        InstallScriptableObjects();
    }

    private void InstallManagers()
    {
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

        Container.BindInterfacesAndSelfTo<TimeManager>()
           .FromNewComponentOnNewGameObject()
           .WithGameObjectName("TimeManager")
           .AsSingle()
           .NonLazy();

        Container.BindInterfacesAndSelfTo<SpecialSkillManager>()
            .FromComponentInNewPrefab(_specialSkillManager)
            .AsSingle()
            .NonLazy();

        Container.BindInterfacesAndSelfTo<UIManager>()
           .FromComponentInHierarchy()
           .AsSingle();
    }

    private void InstallScriptableObjects()
    {
        Container.Bind<PlayerInput>()
            .FromScriptableObject(_playerInput)
            .AsSingle();

        Container.Bind<GameData>()
            .FromScriptableObject(_gameDataAsset)
            .AsSingle();
        
     
    }
}