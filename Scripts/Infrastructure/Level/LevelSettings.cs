using UnityEngine;

namespace Infrastructure.Level
{
    [System.Serializable]
    public class LevelSettings
    {
        [SerializeField] private string _networkScene;
        [SerializeField] private string _baseScene;
        [SerializeField] private string _coreScene;
        [SerializeField] private string _buildScene;

        public string BaseScene => _baseScene;
        public string CoreScene => _coreScene;
        public string BuildScene => _buildScene;
        public string NetworkScene => _networkScene;
    }
}