using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "EnergySettingsInstaller", menuName = "Installers/EnergySettingsInstaller")]
public class EnergySettingsInstaller : ScriptableObjectInstaller<EnergySettingsInstaller>
{
    public EnergyService energyService;
    public override void InstallBindings()
    {
        Container.BindInstance(energyService);
    }
}