using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class ExitHost : NetworkBehaviour
    {
        [SerializeField] private Button exitButton;
        public event Action Exit;

        private void Start()
        {
            exitButton.onClick.AddListener(ExitServer);
        }

        private void ExitServer()
        {
            Exit?.Invoke();
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopClient();
            }
            else if (NetworkServer.active)
            {
                NetworkManager.singleton.StopServer();
            }
        }
    }
}