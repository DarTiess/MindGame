using System;
using System.IO;
using GoogleSheets;
using Network;
using UnityEngine;

namespace Infrastructure.Installers.Settings.Network
{
    [Serializable]
    public class NetworkAddress
    {
        private NetworkAddressSheet _networkAddressSheet;
        public string Address => _networkAddressSheet.Address;
        public string UdpPort => _networkAddressSheet.UdpPort;
        public string WsPort => _networkAddressSheet.WsPort;

        public void SetNetworkAddress()
        {
            string json;
            string paths =  Application.persistentDataPath+$"{Constants.GAME_DTO}.json";
            if (File.Exists(paths))
            {
                json = File.ReadAllText(paths);
                Debug.Log("Applic file read");
            }
            else
            {
                json=Resources.Load<TextAsset>("GameData/"+Constants.GAME_DTO).text;
                Debug.Log("Resource file read");

            }
            Debug.Log("Set Network Adress"); 
            NetworkSettings addressList= JsonUtility.FromJson<NetworkSettings>(json);
              _networkAddressSheet =addressList.NetworkAddressSheet[0];
        }
    }
}