using System;
using System.Collections;
using System.Collections.Generic;
using Infrastructure.Installers.Settings.MainPlayer;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.PlayerPanel
{
    public class MainPlayerInfoUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private SkeletonView _skeletonView;
        [SerializeField] private TMP_Text _playerCoins;
        [Header("Animation")] 
        [SerializeField] private float _duration = 0.5f;

        private int _currentCoins;
        private IMainPlayerLoader _mainPlayerLoader;
        private Skin _characterSkin;

        public void Initialize(IMainPlayerLoader mainPlayerLoader)
        {
            gameObject.SetActive(true);
            _mainPlayerLoader = mainPlayerLoader;
            _mainPlayerLoader.ChangeCoins += OnChangeCoins;
            _playerName.text = _mainPlayerLoader.PlayerName;
            _skeletonView.SetSkeletonSkin(_mainPlayerLoader.PlayerAvatar);
            //  _playerSkeleton.initialSkinName = _mainPlayerLoader.PlayerAvatar;
            _playerCoins.text = _mainPlayerLoader.PlayerCoins.ToString();
            _currentCoins = _mainPlayerLoader.PlayerCoins;
        }

        private void OnDisable()
        {
            if(_mainPlayerLoader!=null)
                _mainPlayerLoader.ChangeCoins -= OnChangeCoins;

        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnChangeCoins(int coins)
        {
           //  _playerCoins.text = coins.ToString();
            StopCoroutine(AnimateCoinsChange(coins));
            StartCoroutine(AnimateCoinsChange(coins));
        }

        private IEnumerator AnimateCoinsChange(int targetCoins)
        {
            int startCoins = _currentCoins;
            float elapsed = 0f;

            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                _currentCoins = Mathf.RoundToInt(Mathf.Lerp(startCoins, targetCoins, elapsed / _duration));
                _playerCoins.text = _currentCoins.ToString();
                yield return null; 
            }

            _currentCoins = targetCoins;
            _playerCoins.text = _currentCoins.ToString();
        }
    }
}