using System;
using System.Collections.Generic;
using System.IO;
using Network;
using Newtonsoft.Json;
using UnityEngine;

namespace Infrastructure.Installers.Settings.Build
{
    [Serializable]
    public class BuildSetting
    {
        private BuildConfig _buildStates;
        public BuildConfig BuildStates => _buildStates;

        public BuildSetting()
        {
            _buildStates = new BuildConfig();
        }

        public void SaveChanges()
        {
            if (_buildStates == null)
            {
                AddStates();
            }
#if UNITY_EDITOR
            var jsonSer = JsonConvert.SerializeObject(_buildStates, Formatting.Indented);
            string path = $"Assets/Resources/GameData/{Constants.BUILDS_CONFIGS}.json";
            File.WriteAllText(path, jsonSer);
#else
            var jsonSer = JsonConvert.SerializeObject(_buildStates, Formatting.Indented);
            string path = Application.persistentDataPath+$"{Constants.BUILDS_CONFIGS}.json";
            File.WriteAllText(path, jsonSer);
#endif
        }

        public void Load()
        {
            string json;
#if UNITY_EDITOR
            string paths = $"Assets/Resources/GameData/{Constants.BUILDS_CONFIGS}.json";

#else
           string paths = Application.persistentDataPath+$"{Constants.BUILDS_CONFIGS}.json";
#endif

            if (File.Exists(paths))
            {
#if UNITY_EDITOR
                json = Resources.Load<TextAsset>("GameData/" + Constants.BUILDS_CONFIGS).text;
                Debug.Log("Resource file read Builds");
#else
               json = File.ReadAllText(paths);
               Debug.Log("Applic file read Builds");
#endif
            }
            else
            {
                AddStates();
                SaveChanges();
#if UNITY_EDITOR
                json = Resources.Load<TextAsset>("GameData/" + Constants.BUILDS_CONFIGS)?.text;
#else
               json = File.ReadAllText(paths);
#endif
            }

            BuildConfig buildConfig = JsonUtility.FromJson<BuildConfig>(json);
            if (_buildStates != null)
            {
                _buildStates = buildConfig;
            }
            else
            {
                AddStates();
            }

            Debug.Log("Complete Load Saves Builds");
        }

        public void CreateBuildState()
        {
            Load();
        }

        private void AddStates()
        {
            if (_buildStates == null)
            {
                _buildStates = new BuildConfig();
            }

            if (_buildStates.State == null)
            {
                _buildStates.State = new List<string>();
            }

            for (int i = 0; i < 5; i++)
            {
                _buildStates.State.Add("StartState");
            }
        }
    }
}