using System;
using System.Collections.Generic;
using System.Linq;
using Network.NetworkObjects;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Infrastructure.UIPanels.FinishPanels
{
    public class LeaderBoard : MonoBehaviour
    {
        [SerializeField] private List<StatisticsUi> _listUiZone;
        [SerializeField] private Button _restartButton;
        [SerializeField] private TMP_Text _placeTxt;
        public event Action OnRestartClicked;

        private void Start()
        {
            _restartButton.onClick.AddListener(Restart);
        }

        public void UpdateLeaderBoard(List<PlayersAnswered> playersAnswereds, int place)
        {
            _placeTxt.text = place + " PLACE";
            var tempList = playersAnswereds.OrderByDescending(x => x.Coins).ToList();
            for (int i = 0; i < tempList.Count; i++)
            {
                if (_listUiZone[i].Zone == null || _listUiZone[i].Name == null ||
                    _listUiZone[i].Power == null || _listUiZone[i]._skeletonView == null)
                    return;

                _listUiZone[i].Zone.SetActive(true);
                _listUiZone[i].Name.text = tempList[i].Name;
                _listUiZone[i].Power.text = Mathf.RoundToInt(tempList[i].Coins).ToString();
                _listUiZone[i].SetSkeletonSkin(tempList[i].GetAvatarSprite());
               // _listUiZone[i].Avatar.initialSkinName = tempList[i].GetAvatarSprite();
            }
        }
       
        private void Restart()
        {
            OnRestartClicked?.Invoke();
        }
    }
}