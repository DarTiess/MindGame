using System;
using System.Collections.Generic;
using Infrastructure.Installers.Settings.Players;
using Network.NetworkObjects;

public interface IQuizHandler
{
    public event Action<Players> FinishChoosingPlayer; 
    public event Action<Players> WinPlayer; 
    public event Action<Players> LoosePlayer;

    void PlayerAnswer(bool isRight, float coins, string players);
   
    void CompleteQuiz(List<PlayersAnswered> playersAnswereds, Players player);
    void Init(PlayerSettings playerSettings);
    void PlayerVaBanque(Players player);
}