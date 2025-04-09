using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.Auth
{
    public class AuthenticationPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputName;
        [SerializeField] private AvatarPanel _avatarPanel;
        [SerializeField] private Button _saveButton;
        private int _avatarIndex;
        public event Action<string, int> OnSavePlayerInfo;

        private void Start()
        {
            _saveButton.onClick.AddListener(SavePlayerInfo);
            _inputName.onValueChanged.AddListener(ShowClientButton);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void SavePlayerInfo()
        {
            Hide();
            OnSavePlayerInfo?.Invoke(_inputName.text, _avatarIndex);
        }

        private void ShowClientButton(string arg0)
        {
            _avatarPanel.SetAvatar += SetAvatar;
            _avatarPanel.Show();
            _saveButton.gameObject.SetActive(true);
        }

        private void SetAvatar(int avatarIndex)
        {
            _avatarIndex = avatarIndex;
        }
    }
}