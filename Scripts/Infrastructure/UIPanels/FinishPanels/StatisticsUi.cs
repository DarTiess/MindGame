using System;
using Infrastructure.UIPanels.PlayerPanel;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.FinishPanels
{
    [Serializable]
    public class StatisticsUi
    {
        public GameObject Zone;
        public Text Name;
        public Text Power;
        public SkeletonView _skeletonView;

        public void SetSkeletonSkin(string getAvatarSprite)
        {
          _skeletonView.SetSkeletonSkin(getAvatarSprite);
        }
    }
}