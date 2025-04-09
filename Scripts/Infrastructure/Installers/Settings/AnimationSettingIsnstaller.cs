using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "AnimationSettingsInstaller", menuName = "Installers/AnimationSettingIsnstaller")]
public class AnimationSettingIsnstaller : ScriptableObjectInstaller<AnimationSettingIsnstaller>
{
    public AnimationSettings AnimationSettings;
    public override void InstallBindings()
    {
        Container.BindInstance(AnimationSettings);
    }
}