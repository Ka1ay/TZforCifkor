using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<RequestQueue>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<WeatherService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<DogService>().AsSingle();
    }
}

