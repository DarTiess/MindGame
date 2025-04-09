using System;
using System.Collections;
using System.IO;
using Building;
using GoogleSheets;
using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using Infrastructure.Installers.Settings.MainPlayer;
using Network;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

[Serializable]
public class EnergyService
{
    private EnergySheetsList _energySheets;
    private IEventBus _eventBus;
    private IMainPlayerLoader _mainPlayerLoader;
    private ICoroutineRunner _coroutineRunner;
    private bool _inCore;

    public void SetEnergiConfig(IEventBus eventBus, IMainPlayerLoader mainPlayerLoader, ICoroutineRunner coroutineRunner)
    {
        _eventBus = eventBus;
        _mainPlayerLoader = mainPlayerLoader;
        _coroutineRunner = coroutineRunner;
        _eventBus.Subscribe<PlayGame>(AddEnergyForStartPlaying);
        _eventBus.Subscribe<LevelStart>(AddEnergyForMinutes);
        string json;
        string paths =  Application.persistentDataPath+$"{Constants.GAME_DTO}.json";
        if (File.Exists(paths))
        {
            json = File.ReadAllText(paths);
            Debug.Log("Applic file read");
        }
        else
        {
            json=Resources.Load<TextAsset>("GameData/"+Constants.GAME_DTO).text;
            Debug.Log("Resource file read");

        }
        Debug.Log("Set Energy"); 
        EnergySettings energySettings= JsonUtility.FromJson<EnergySettings>(json);
        _energySheets =energySettings.EnergySheetsList[0];
    }

    private void AddEnergyForMinutes(LevelStart obj)
    {
        _inCore = false;
        _coroutineRunner.StartCoroutine(EnergyRecoveryRoutine());
    }
    
    private IEnumerator EnergyRecoveryRoutine()
    {
        while (!_inCore)
        {
            yield return new WaitForSeconds(_energySheets.RecoverDuration*60);
            if (_mainPlayerLoader.PlayerEnergy < _energySheets.Maximum)
            {
                _mainPlayerLoader.AddPlayerEnergy(1);
                Debug.Log("RECOVERY ROUTINE");
            }
        }
    }

    private void AddEnergyForStartPlaying(PlayGame obj)
    {
        _inCore = true;
        if (_mainPlayerLoader.PlayerEnergy + _energySheets.CoreEnter < _energySheets.Maximum)
        {
            _mainPlayerLoader.AddPlayerEnergy(_energySheets.CoreEnter);
        }
        else
        {
            _mainPlayerLoader.AddPlayerEnergy(_energySheets.Maximum-_mainPlayerLoader.PlayerEnergy);
        }
    }
}