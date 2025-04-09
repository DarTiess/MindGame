using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Network.NetworkObjects;
using UnityEngine;

namespace Infrastructure.UIPanels.PlayerPanel
{
    public class PlayerUIPanel : MonoBehaviour
    {
        [SerializeField] private List<PlayerAnsweredUI> _playerAnsweredUis;
        [SerializeField] private ColumsPanel _columsPanel;
        private List<PlayersAnswered> _playersList;
        private string _currentPlayer;
        public event Action<string> PlayerChooseEnemyToAttack;

        public void SetPlayers(List<PlayersAnswered> playersList,
            float moveDuration,float resizeDuration, float coinsShakeDuration, float coinsChangeDuration, float rotateAngle,
             Ease jumpEase, string currentPlayer)
        {
            _playersList = playersList;
            _currentPlayer = currentPlayer;
            for (int i = 0; i < _playersList.Count; i++)
            {
                if (_playerAnsweredUis[i] == null)
                    return;
                _playerAnsweredUis[i]
                    .SetPlayer(_playersList[i], moveDuration,resizeDuration,coinsShakeDuration,coinsChangeDuration, rotateAngle, jumpEase, _currentPlayer, i);
                _columsPanel.ShowColum(i);
            }
        }

        public void CountCoins(List<PlayersAnswered> playersList)
        {
            _playersList.Clear();
            _playersList = playersList;
            var list = _playersList.OrderByDescending(x => x.Coins).ToList();
            _columsPanel.SetColumsStatus(list);

            StartCoroutine(MoveUIPlayer(list));
        }

        public void DisplayEnemiesEnabled()
        {
            for (int i = 0; i < _playerAnsweredUis.Count; i++)
            {
                if (_currentPlayer != _playerAnsweredUis[i].PlayerName
                    && _playerAnsweredUis[i].gameObject.activeInHierarchy)
                {
                    _playerAnsweredUis[i].LightOn();
                    _playerAnsweredUis[i].OnMakeChooseEnemy += MakeChooseEnemy;
                }
            }
        }

        public void ShowEnemiesAnswers(List<PlayersAnswered> playersAnsweredsList)
        {
            for (int i = 0; i < _playerAnsweredUis.Count; i++)
            {
                if (_currentPlayer != _playerAnsweredUis[i].PlayerName
                    && _playerAnsweredUis[i].gameObject.activeInHierarchy)
                {
                    _playerAnsweredUis[i].ShowAnswer(playersAnsweredsList[i].AnswerIcon);
                }
            }
        }

        private IEnumerator MoveUIPlayer(List<PlayersAnswered> list)
        {
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < list.Count(); i++)
            {
                foreach (PlayerAnsweredUI playerAnsweredUi in _playerAnsweredUis)
                {
                    if (playerAnsweredUi.PlayerName == list[i].Name)
                        playerAnsweredUi.MoveTo(_columsPanel.PlayerPosition(i).position, list[i]);
                }
            }
        }

        private void MakeChooseEnemy(string enemyName)
        {
            
            for (int i = 0; i < _playerAnsweredUis.Count; i++)
            {
                if (_currentPlayer != _playerAnsweredUis[i].PlayerName
                    && _playerAnsweredUis[i].gameObject.activeInHierarchy)
                {
                    _playerAnsweredUis[i].OnMakeChooseEnemy -= MakeChooseEnemy;
                    _playerAnsweredUis[i].LightOff();
                }
            }

            PlayerChooseEnemyToAttack?.Invoke(enemyName);
        }

        public void TakeFreeze()
        {
            for (int i = 0; i < _playerAnsweredUis.Count; i++)
            {
                if (_playerAnsweredUis[i].PlayerName == _currentPlayer)
                {
                    _playerAnsweredUis[i].TakeFreeze();
                }
            }
        }

        public void TakeAttacking(float freezeTimer)
        {
            _playerAnsweredUis.ForEach(x=>x.TakeAttack(freezeTimer));
          
        }

        public void ShowBombOnEnemy(string enemyName)
        {
            _playerAnsweredUis.Find(x => x.PlayerName == enemyName).ShowBombOnEnemy();
        }

        public void ShowFreezeOnEnemy(string enemyName)
        {
            _playerAnsweredUis.Find(x => x.PlayerName == enemyName).ShowFreezeOnEnemy();
        }

        public void EndingFreeze()
        {
            //_playerAnsweredUis.Find(x => x.PlayerName == enemyName).ShowFreezeOnEnemy();
        }

        public void ShowReflect(string playerName)
        {
            _playerAnsweredUis.Find(x => x.PlayerName == playerName).ShowReflect();
        }
    }
}