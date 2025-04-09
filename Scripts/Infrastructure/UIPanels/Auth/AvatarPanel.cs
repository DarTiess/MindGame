using System;
using System.Collections.Generic;
using Infrastructure.Installers.Settings.Players;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Infrastructure.UIPanels.Auth
{
    public class AvatarPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private AvatarItem _avatarPrefab;
        private PlayerSettings _playerSettings;
        private List<AvatarItem> _avatarsList;
        private int _indexAvatar;
        public event Action<int> SetAvatar;

        [Inject]
        public void Construct(PlayerSettings playerSettings)
        {
            _playerSettings = playerSettings;
        }

        private void Awake()
        {
               _avatarsList = new List<AvatarItem>(_playerSettings.Avatars.Count);
              
               _indexAvatar = 0;
               foreach (var avatars in _playerSettings.Avatars)
               {
                   AvatarItem avatar = Instantiate(_avatarPrefab, _container);
                  // var avatar = avatarImage.GetComponent<AvatarItem>();
                   avatar.SetSprite(avatars, _indexAvatar);
                   _avatarsList.Add(avatar);
                  // avatar.MakeChooseAvatar += ChooseAvatar;
   
                   _indexAvatar++;
               } 
               gameObject.SetActive(false);
        }

        private void ChooseAvatar(int avatarIndex)
        {
            SetAvatar?.Invoke(avatarIndex);
            foreach (var avatarItem in _avatarsList)
            {
                avatarItem.MakeChooseAvatar -= ChooseAvatar;
            }
        }

        public void Show()
        {
             Debug.Log("Show");
            gameObject.SetActive(true);
            foreach (var avatarItem in _avatarsList)
            {
                avatarItem.MakeChooseAvatar += ChooseAvatar;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}