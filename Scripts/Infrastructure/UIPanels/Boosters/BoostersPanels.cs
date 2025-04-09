using System;
using System.Collections.Generic;
using Infrastructure.Installers.Settings.Build;
using Infrastructure.Installers.Settings.Players;
using UnityEngine;
using Zenject;

namespace Infrastructure.UIPanels.Boosters
{
    public class BoostersPanels : MonoBehaviour
    {
        [SerializeField] private Sprite _activeButtonSprite;
        [SerializeField] private Sprite _inactiveButtonSprite;
        [SerializeField] private List<BoosterView> _boosters;
        [SerializeField] private BoosterView _allInBooster;
        private BoosterType _boosterType;
        private int _indexBooster;
        private IInventoryService _inventoryService;
        public event Action HideWrongAnswer;
        public event Action VaBank;
        public event Action FreezeEnemy;
        public event Action BombEnemy;
        public event Action DisplayEnemiesAnswers;
        public event Action RotateEnemiesAnswers;
        public event Action ComicEnemiesAnswers;
        public event Action MirrorEnemiesAttack;

        [Inject]
        public void Construct(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        private void Start()
        {
            ActivateAllButtons();
        }

        public void Init()
        {
            _indexBooster = 0;
            for (int i = 0; i < _inventoryService.Boosters.Count; i++)
            {
                if (_indexBooster >= _boosters.Count)
                    return;
                if (_inventoryService.Boosters[i].IsUsed)
                {
                    _boosters[_indexBooster].Initialize(_inventoryService.Boosters[i], _activeButtonSprite);
                    _boosters[_indexBooster].Clicked += OnClickedBooster;
                    _indexBooster++;
                }
            }
            _allInBooster.InitializeAllInBooster(_activeButtonSprite);
            _allInBooster.Clicked += OnClickedBooster;
        }

        public void InactiveButtonSprite(BoosterType type)
        {
            foreach (BoosterView boosterView in _boosters)
            {
                boosterView.InactiveButton(_inactiveButtonSprite, type);
            }

            _boosterType = BoosterType.None;
        }

        public void ShowAllInBooster()
        {
            _allInBooster.Show();
        }

        private void OnClickedBooster(BoosterType booster, BoosterView boosterView)
        {
            if(booster==_boosterType)
                return;
            switch (booster)
            {
                case BoosterType.ShowHalf:
                    Hide2WrongAnswer();
                    break;
                case BoosterType.Freeze:
                    MakeFreezeEnemy();
                    break;
                case BoosterType.VocabularyBomb:
                    MakeBombEnemy();
                    break;
                case BoosterType.Loupe:
                    DisplayAnswers();
                    break;
                case BoosterType.AllIn:
                    AllInClicked();
                    break;
                case BoosterType.Rotate:
                   MakeRotateEnemy();
                    break;
                case BoosterType.Comic:
                    MakeComicEnemy();
                    break;
                case BoosterType.Mirror:
                    MirrorAll();
                    break;
                default:
                    Debug.Log("No booster recognize");
                    break;
            }
        }


        private void DisplayAnswers()
        {
            _boosterType = BoosterType.Loupe;
            DisplayEnemiesAnswers?.Invoke();
        }

        private void MakeBombEnemy()
        {
            _boosterType = BoosterType.VocabularyBomb;
            BombEnemy?.Invoke();
        }

        private void MakeFreezeEnemy()
        {
            _boosterType = BoosterType.Freeze;
            FreezeEnemy?.Invoke();
        }

        private void AllInClicked()
        {
            VaBank?.Invoke();
        }

        private void Hide2WrongAnswer()
        {
            _boosterType = BoosterType.ShowHalf;
            HideWrongAnswer?.Invoke();
        }

        private void MakeRotateEnemy()
        {
            _boosterType = BoosterType.Rotate;
            RotateEnemiesAnswers?.Invoke();
        }

        private void MakeComicEnemy()
        {
            _boosterType = BoosterType.Comic;
            ComicEnemiesAnswers?.Invoke();
        }

        private void MirrorAll()
        {
            _boosterType = BoosterType.Mirror;
            MirrorEnemiesAttack?.Invoke();
        }

        private void ActivateAllButtons()
        {
            foreach (BoosterView boosterView in _boosters)
            {
                boosterView.ActivateButton();
            }
        }
    }
}