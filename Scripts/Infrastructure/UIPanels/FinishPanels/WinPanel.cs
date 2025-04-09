using System;
using System.Collections.Generic;
using DG.Tweening;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.UIPanels.PlayerPanel;
using Network.NetworkObjects;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.FinishPanels
{
    public class WinPanel: PanelBase
    {

        [FormerlySerializedAs("name")] [SerializeField] private TextMeshProUGUI _playerName;
        [FormerlySerializedAs("coins")] [SerializeField] private TextMeshProUGUI _coins;
        [SerializeField] private SkeletonView _skeletonView;
        [SerializeField] private LeaderBoard _leaderBoard;
        [SerializeField] private Button _leaderBoardButton;
        public override event Action ClickedPanel;
        protected override void OnClickedPanel()
        {
            ClickedPanel?.Invoke();
        }

        public void Initialize(float animationDuration,float rotateAngle, Players player, List<PlayersAnswered> playersAnswereds)
        {
            Debug.Log("Initi Win panel");
            if (player == null || playersAnswereds==null)
                return;
            
            if(_playerName==null || _coins==null||
               _skeletonView==null || _leaderBoard==null||
               _leaderBoardButton==null)
                return;
            
            _leaderBoardButton.onClick.AddListener(ShowLeaderBoard);
            _leaderBoard.OnRestartClicked += OnClickedPanel;
            _leaderBoard.UpdateLeaderBoard(playersAnswereds,1);
            _playerName.text = player.Name;
           _skeletonView.SetSkeletonSkin(player.AvatarName);
            _coins.text = Mathf.RoundToInt(player.Coins).ToString();
            _coins.transform.DOShakeRotation(animationDuration, rotateAngle);
        }
       

        private void ShowLeaderBoard()
        {
            if(_leaderBoard!=null)
                _leaderBoard.gameObject.SetActive(true);
        }
    }
}