using Mirror;

namespace Network.NetworkObjects.Messages
{
    public struct AuthResponseMessage : NetworkMessage
    {
        public byte code;
        public string message;
    }
}