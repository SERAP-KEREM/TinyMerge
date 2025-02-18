using UnityEngine;
using Zenject;
using _Game.Scripts.Management;
using _Game.Scripts._helpers;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private ItemManager _itemManager;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private ParticleManager _particleManager;
    [SerializeField] private TileManager _tileManager;

    public override void InstallBindings()
    {
        // Ba??ml?l?klar? manuel olarak ba?la
        Container.Bind<ItemManager>().FromInstance(_itemManager).AsSingle();
        Container.Bind<LevelManager>().FromInstance(_levelManager).AsSingle();
        Container.Bind<TileManager>().FromInstance(_tileManager).AsSingle();
        Container.Bind<ParticleManager>().FromInstance(_particleManager).AsSingle(); // Tekil (Singleton)

        // E?er AudioManager, TimeManager veya UIManager kullan?lacaksa bunlar? da ekleyebilirsiniz:
        // Container.Bind<AudioManager>().FromInstance(_audioManager).AsSingle();
        // Container.Bind<TimeManager>().FromInstance(_timeManager).AsSingle();
        // Container.Bind<UIManager>().FromInstance(_uiManager).AsSingle();
    }
}
