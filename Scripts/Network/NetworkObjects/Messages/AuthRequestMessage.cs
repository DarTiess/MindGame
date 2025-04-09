using Mirror;

namespace Network.NetworkObjects.Messages
{
    public struct AuthRequestMessage : NetworkMessage
    {
        public string authUsername;
    }
}