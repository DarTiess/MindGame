using System;
using System.Collections.Generic;
using System.Linq;
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
    public class LoosePanel : PanelBase
    {
        [FormerlySerializedAs("name")] [SerializeField]
        private TextMeshProUGUI _playerName;

        [FormerlySerializedAs("coins")] [SerializeField]
        private TextMeshProUGUI _coins;

        [SerializeField] private SkeletonView _skeletonView;

        [SerializeField] private LeaderBoard _leaderBoard;
        [SerializeField] private Button _leaderBoardButton;

        public override event Action ClickedPanel;

        protected override void OnClickedPanel()
        {
            ClickedPanel?.Invoke();
        }

        public void Initialize(float animationDuration, float rotateAngle, Players player,
            List<PlayersAnswered> playersAnswereds)
        {
            if (player == null || playersAnswereds == null)
                return;

            _leaderBoardButton.onClick.AddListener(ShowLeaderBoard);
            _leaderBoard.OnRestartClicked += OnClickedPanel;
            var tempList = playersAnswereds.OrderByDescending(x => x.Coins).ToList();
            int place = tempList.FindIndex(x => x.Name == player.Name);
            if (place > 1)
            {
                ShowLeaderBoard();
            }
            else
            {
                if (_coins == null || _playerName == null)
                    return;

                _playerName.text = player.Name;
              //  _playerAvatar.sprite = player.GetAvatarSprite();

              _skeletonView.SetSkeletonSkin(player.AvatarName);
                _coins.text = Mathf.RoundToInt(player.Coins).ToString();
                _coins.transform.DOShakeRotation(animationDuration, rotateAngle);
            }

            _leaderBoard.UpdateLeaderBoard(playersAnswereds, place + 1);
        }
     


        private void ShowLeaderBoard()
        {
            _leaderBoardButton.gameObject.SetActive(false);
            _leaderBoard.gameObject.SetActive(true);
        }
    }
}