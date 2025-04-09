using System;
using UnityEngine;

namespace Infrastructure.Installers.Settings.Build
{
    [Serializable]
    public class BuildsService
    {
        private BuildSetting _setting;
        public BuildSetting Setting => _setting;

        public BuildsService()
        {
            _setting = new BuildSetting();
        }

        public void ChangeState(string stateName, int number)
        {
            _setting.BuildStates.State[number] = stateName;
            Debug.Log("STATE " + stateName);
            _setting.SaveChanges();
        }

        public void LoadSaves()
        {
            if (_setting == null)
                _setting = new BuildSetting();
            _setting.Load();
        }
    }
}