using Mirror;

namespace Network.NetworkObjects.Messages
{
    public struct ReadyToStartMessage : NetworkMessage
    {
        public bool IsReady { get; set; }
    }
}