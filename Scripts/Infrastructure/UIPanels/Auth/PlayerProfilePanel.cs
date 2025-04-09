using System;
using DG.Tweening;
using Infrastructure.Installers.Settings.MainPlayer;
using Infrastructure.UIPanels.PlayerPanel;
using Spine;
using Spine.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.Auth
{
    public class PlayerProfilePanel: MonoBehaviour
    {
        [Header("Name")]
        [SerializeField] private TMP_Text _name;
        [SerializeField] private GameObject _namePanel;
        [SerializeField] private TMP_InputField _inputName;
        [SerializeField] private Button _changeNameButton;
        [SerializeField] private Button _saveNameButton;
        [Header("Avatar")]
        [SerializeField] private SkeletonView _skeletonView;
        [SerializeField] private AvatarPanel _avatarPanel;
        [SerializeField] private Button _changeAvatarButton;
        [Header("Id")]
        [SerializeField] private TMP_Text _id;
        [SerializeField] private Image _idIsCoppiedImage;
        [SerializeField] private Button _idCoppyButton;
        [Header("QR")]
        [SerializeField] private Image _qrCodePanel;
        [SerializeField] private Button _qrOpenButton;
        [Space(20)] 
        [SerializeField] private GameObject _backFade;
        [SerializeField] private TMP_Text _coins;
        [SerializeField] private TMP_Text _partyRating;
        [SerializeField] private Button _closeButton;
        
        private IMainPlayerLoader _mainPlayerLoader;
        public event Action ChangedAvatar;
        public event Action ChangedName;

        private void Start()
        {
           
           HidePanel();
        }

        public void ShowPanel(IMainPlayerLoader mainPlayerLoader)
        {
            _closeButton.onClick.AddListener(HidePanel);
            _changeNameButton.onClick.AddListener(EditName);
            _saveNameButton.onClick.AddListener(SaveName);
            _changeAvatarButton.onClick.AddListener(EditAvatar);
            _idCoppyButton.onClick.AddListener(CoppyId);
            _qrOpenButton.onClick.AddListener(ShowQr);
            _mainPlayerLoader = mainPlayerLoader;
            _name.text = mainPlayerLoader.PlayerName;
           _skeletonView.SetSkeletonSkin(mainPlayerLoader.PlayerAvatar);
          // _skeleton.initialSkinName = mainPlayerLoader.PlayerAvatar;
            _coins.text = mainPlayerLoader.PlayerCoins.ToString();
            _id.text = mainPlayerLoader.PlayerID;
            gameObject.SetActive(true);
        }
     
        private void HidePanel()
        {
            HideAll();
            gameObject.SetActive(false);
        }

        private void HideAll()
        {
            _backFade.SetActive(false);
            _namePanel.SetActive(false);
            _avatarPanel.Hide();
            _idIsCoppiedImage.gameObject.SetActive(false);
            _qrCodePanel.gameObject.SetActive(false);
        }

        private void EditName()
        {
            _backFade.SetActive(true);
            _namePanel.SetActive(true);
        }

        private void SaveName()
        {
            _name.text = _inputName.text;
            _mainPlayerLoader.SetPlayerName(_inputName.text);
            ChangedName?.Invoke();
            HideAll();
        }

        private void EditAvatar()
        {
            Debug.Log("Clicked Avatar");
            _avatarPanel.Show();
            _avatarPanel.SetAvatar += ChangeAvatar; 
            _backFade.SetActive(true);
        }

        private void ChangeAvatar(int avatarIndex)
        {
            _avatarPanel.SetAvatar -= ChangeAvatar;
            _mainPlayerLoader.SetPlayerAvatar(avatarIndex);
            _skeletonView.SetSkeletonSkin(_mainPlayerLoader.PlayerAvatar);
            ChangedAvatar?.Invoke();
            HideAll();
        }

        private void ShowQr()
        {
            _backFade.SetActive(true);
            _qrCodePanel.gameObject.SetActive(true);
            _qrCodePanel.DOFade(0, 0.5f).SetLoops(2, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    HideAll();
                });
        }

        private void CoppyId()
        {
            _backFade.SetActive(true);
            _idIsCoppiedImage.gameObject.SetActive(true);
            _idIsCoppiedImage.DOFade(0, 0.5f).SetLoops(2, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    HideAll();
                });
        }
    }
}