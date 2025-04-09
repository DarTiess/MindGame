using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels
{
    public class CoinsDisplay: MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinsTxt;
        [SerializeField] private Image _background;
        [SerializeField] private Color _winColor;
        [SerializeField] private Color _looseColor;

        private float _currentCoins;
        public void SetCoins(float coins)
        {
            _currentCoins = coins;
        }

        public void ShowCoinsChange(float coins)
        {
            if (_currentCoins > coins)
            {
                ShowChanges(coins, _looseColor);
                gameObject.SetActive(true);
            }
            else if(_currentCoins<coins)
            {
                ShowChanges(coins, _winColor);
                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if(gameObject==null)
                return;
            gameObject.SetActive(false);
        }

        private void ShowChanges(float coins, Color color)
        {
            _background.color = color;
            var addCoins = coins - _currentCoins;
            _currentCoins = coins;
            _coinsTxt.text = addCoins.ToString();
        }
    }
}