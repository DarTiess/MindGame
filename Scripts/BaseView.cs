using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.UIPanels;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class BaseView : MonoBehaviour
{
    [FormerlySerializedAs("enterPanel")] [Header("Panels")] [SerializeField]
    private InputNamePanel _inputNamePanel;

    private IEventBus _eventBus;
    private PlayerSettings _playerSettings;

    [Inject]
    public void Construct(IEventBus eventBus, PlayerSettings playerSettings)
    {
        _eventBus = eventBus;
        _playerSettings = playerSettings;
    }

    private void Start()
    {
        _inputNamePanel.InputName += OnInputName;
        EnterGame();
    }

    private void EnterGame()
    {
        _inputNamePanel.Show();
    }

    private void OnInputName(string name)
    {
        _playerSettings.CurrentPlayer = name;

        _inputNamePanel.InputName -= OnInputName;
        _eventBus.Invoke(new BootCoreScene());
    }
}