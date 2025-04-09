using System.Collections;
using System.Collections.Generic;
using Mirror;
using Network.NetworkObjects.Messages;
using UnityEngine;

namespace Network.NetworkObjects
{
    public class PlayerAuth : NetworkAuthenticator
    {
        public string _playerName;
        private readonly HashSet<NetworkConnection> _connectionsPendingDisconnect = new HashSet<NetworkConnection>();
        internal static readonly HashSet<string> _playerNames = new HashSet<string>();
        public bool OnGame { get; set; }


        //server
        public override void OnStartServer()
        {
            NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
        }

        private void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequestMessage msg)
        {
            Debug.Log($"Authentication Request: {msg.authUsername}");

            if (_connectionsPendingDisconnect.Contains(conn))
                return;


            if (!_playerNames.Contains(msg.authUsername))
            {
                if (OnGame)
                {
                    _connectionsPendingDisconnect.Add(conn);

                    AuthResponseMessage authResponseMessage = new AuthResponseMessage
                    {
                        code = 200,
                        message = "Username already in use...try again"
                    };

                    conn.Send(authResponseMessage);

                    conn.isAuthenticated = false;

                    StartCoroutine(DelayedDisconnect(conn, 1f));
                }
                else
                {
                    _playerNames.Add(msg.authUsername);
                    conn.authenticationData = msg.authUsername;

                    AuthResponseMessage authResponseMessage = new AuthResponseMessage
                    {
                        code = 100,
                        message = "Success"
                    };

                    conn.Send(authResponseMessage);
                    ServerAccept(conn);
                }
            }
            else
            {
                _connectionsPendingDisconnect.Add(conn);

                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    code = 200,
                    message = "Username already in use...try again"
                };

                conn.Send(authResponseMessage);

                conn.isAuthenticated = false;

                StartCoroutine(DelayedDisconnect(conn, 1f));
            }
        }

        IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            ServerReject(conn);

            yield return null;
            _connectionsPendingDisconnect.Remove(conn);
        }


        //client
        public override void OnStartClient()
        {
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
        }

        public void OnAuthResponseMessage(AuthResponseMessage msg)
        {
            if (msg.code == 100)
            {
                Debug.Log($"Authentication Response: {msg.code} {msg.message}");
                ClientAccept();
            }
            else
            {
                Debug.LogError($"Authentication Response: {msg.code} {msg.message}");
                //  _playerNames.Remove(_playerName);
                NetworkManager.singleton.StopHost();
            }
        }

        public void SetPlayerName(string username)
        {
            _playerName = username;
        }

        public override void OnClientAuthenticate()
        {
            NetworkClient.Send(new AuthRequestMessage { authUsername = _playerName });
        }
    }
}