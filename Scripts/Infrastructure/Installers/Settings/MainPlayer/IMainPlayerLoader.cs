using System;
using UnityEngine;

namespace Infrastructure.Installers.Settings.MainPlayer
{
    public interface IMainPlayerLoader
    {
        string PlayerName { get; }
        string PlayerAvatar { get; }
        int PlayerCoins { get; }
        string PlayerID { get; }
        int PlayerEnergy { get; }
        event Action<int> ChangeCoins;
        event Action<string, int> ChangePlayerInfo;
        void AddPlayerCoins(Players.Players currentPlayerCoins, int indexPlayer);
        void MakeScreenshot();
        void InitializeTexture(RenderTexture renderTexture);
        void SetPlayerName(string inputNameText);

        void SetPlayerAvatar(int avatarIndex);
        bool ScreenshotSaved();
        Texture2D GetScreenshot(int width, int height);
        void AddPlayerEnergy(int energySheetsCoreEnter);
    }
}