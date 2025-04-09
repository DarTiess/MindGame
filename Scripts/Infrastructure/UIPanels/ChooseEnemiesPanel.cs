using System;
using DG.Tweening;
using Infrastructure.Installers.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels
{
    public class ChooseEnemiesPanel : PanelBase
    {
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private Text enemyName;
        private string _name;
        public override event Action ClickedPanel;

        private void OnEnable()
        {
            HideButton();
            ChooseEnemyName();
        }

        protected override void OnClickedPanel()
        {
           ClickedPanel?.Invoke();
        }

        public void Initialize(IPersonSettings enemySettings, IPersonSettings playerSettings)
        {
            playerName.text = playerSettings.CurrentPlayer;
            playerName.color =playerSettings.Color;
            _name = enemySettings.CurrentPlayer;
            enemyName.color = enemySettings.Color;
        }

        private void ChooseEnemyName()
        {
            enemyName.DOText(_name, 1.5f, true, ScrambleMode.All)
                     .OnComplete(() =>
                     {
                         ShowButton();
                     });
        }
    }
}