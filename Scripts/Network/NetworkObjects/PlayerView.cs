using Infrastructure.UIPanels.PlayerPanel;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Network.NetworkObjects
{
    public class PlayerView: MonoBehaviour
    {
        [FormerlySerializedAs("playerStatusText")] [SerializeField] private TextMeshProUGUI _playerStatusText;
        [FormerlySerializedAs("playerNameText")] [SerializeField] private TextMeshProUGUI _playerNameText;
        [FormerlySerializedAs("playerCoinsText")] [SerializeField] private TextMeshProUGUI _playerCoinsText;
       // [SerializeField] private Image _playerAvatar;
        [SerializeField] private SkeletonView _skeletonView;
        

        public void OnPlayerNumberChanged(string newPlayerStatus)
        {
            _playerStatusText.text = newPlayerStatus;
        }
        
        public void OnPlayerNameChanged(string newPlayerNumber)
        {
            _playerNameText.text =newPlayerNumber;
        }
        public void OnPlayerCoinsChanged(float newPlayerCoins)
        {
            _playerCoinsText.text =newPlayerCoins.ToString();
        }

        public void OnPlayerAvatarChanged(string newPlayerAvatar)
        {
           // _playerAvatar.sprite = newPlayerAvatar;
           _skeletonView.SetSkeletonSkin(newPlayerAvatar);
        }
      
    }
}