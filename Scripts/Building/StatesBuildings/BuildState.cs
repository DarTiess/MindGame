using System;
using UnityEngine;

namespace Building.StatesBuildings
{
    [Serializable]
    public class BuildState
    {
        [SerializeField] private int _prize;
        [SerializeField] private int _buildReward;
        [SerializeField] private BuildType _buildType;
        [SerializeField] private Sprite _scratchSprite;
        [SerializeField] private Sprite _startBuildSprite;
        [SerializeField] private Sprite _endBuildSprite;

        public int Prize => _prize;
        public BuildType BuildType => _buildType;
        public Sprite StartBuildSprite => _startBuildSprite;
        public Sprite ScratchSprite => _scratchSprite;
        public Sprite EndBuildSprite => _endBuildSprite;
        public int BuildReward => _buildReward;
    }
}