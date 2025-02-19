using UnityEngine;
using Zenject;
using _Game.Scripts.Management;
using _Game.Scripts._helpers;
using _Game.Scripts.Data;

public class GameInstaller : MonoInstaller
{
    [Header("Managers")]
    [SerializeField] private ItemManager _itemManager;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private ParticleManager _particleManager;
    [SerializeField] private TileManager _tileManager;
    [Header("Scriptable Objects")]
    [SerializeField] private PlayerInput _playerInput;
    public override void InstallBindings()
    {
        InstallManagers();
        InstallScriptableObjects();
    }

    private void InstallManagers()
    {
        // Core Managers
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
    }

    private void InstallScriptableObjects()
    {
        Container.Bind<PlayerInput>()
            .FromScriptableObject(_playerInput)
            .AsSingle();
    }
}