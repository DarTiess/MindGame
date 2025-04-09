using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Installers.Settings.MainPlayer;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.Installers.Settings.Quiz;
using ModestTree;
using Network.NetworkObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuizHandler : IQuizHandler
{
    private PlayerSettings _playerSettings;
    private bool _isPlayerChoosed;
    private IMainPlayerLoader _mainPlayerLoader;
    private char[] _ansIcons =new[] { 'a', 'b', 'c', 'd' };

    public event Action<float> PlayerTakeCoins;
    public event Action<Players> FinishChoosingPlayer;
    public event Action<Players> WinPlayer;
    public event Action<Players> LoosePlayer;

    private float LoseStreak => _playerSettings.LoseStreak;

    public QuizHandler(IMainPlayerLoader mainPlayerLoader, PlayerSettings playerSettings)
    {
        _mainPlayerLoader = mainPlayerLoader;
        _playerSettings = playerSettings;
    }

    public void Init(PlayerSettings playerSettings)
    {
        _playerSettings = playerSettings;
    }

    public void PlayerVaBanque(Players player)
    {
        player.VaBanque = true;
    }

    public void PlayerAnswer(bool isRight, float coins, string playerName)
    {
        var player = _playerSettings.PlayersNames.Find(x => x.Name == playerName);
        if (isRight)
        {
            CountWin(coins, player);
        }
        else
        {
            CountLoose(player);
        }

        _isPlayerChoosed = true;
        MakeChoose(player);
    }

    public void CompleteQuiz(List<PlayersAnswered> playersAnswereds, Players player)
    {
        var index = playersAnswereds.IndexOf(playersAnswereds.Find(x => x.Name == player.Name));
        _mainPlayerLoader.AddPlayerCoins(player, index);
    }

    private void CountLoose(Players players)
    {
        players.WinLevel = 0;
        if (players.LoseStreak == 0)
        {
            players.LoseStreak = 1;
        }
        else if (players.LoseStreak == 1)
        {
            players.LoseStreak = 2;
        }
        else
        {
            players.LoseStreak += LoseStreak;
        }

        if (players.VaBanque)
        {
            players.VaBanque = false;
            players.Coins = 0;
        }
    }

    private void CountWin(float coins, Players players)
    {
        players.WinLevel++;

        var winlevel = players.WinLevel;
        var playerCoins = players.Coins;
        if (players.LoseStreak > 0)
        {
            var loseStreak = players.LoseStreak;
            coins *= loseStreak;

            players.LoseStreak = 0;
        }

        var winsStreak = _playerSettings.WinStreaks.FindLast(x => x.QuestionAmount <= winlevel);
        if (winsStreak != null)
            coins *= winsStreak.IncreaseAmount;

        playerCoins += coins;
        if (players.VaBanque)
        {
            playerCoins *= 2;
            players.VaBanque = false;
        }

        playerCoins = (float)Math.Round(playerCoins, 0);
        players.Coins = playerCoins;

        PlayerTakeCoins?.Invoke(playerCoins);
    }


    private async void MakeChoose(Players players)
    {
        if (_isPlayerChoosed)
        {
            _isPlayerChoosed = false;
            await Task.Delay(500);
            FinishChoosingPlayer?.Invoke(players);
        }
    }

    public void BotAnswer(PlayersAnswered bot, Question question, float speed, float maxCoins)
    {
        float rnd = Random.Range(0, speed);
        speed = bot.Coins > maxCoins ? speed-rnd : speed+rnd;
        Debug.Log("SPEED "+ speed+" = "+bot.Name);

        int botAnsw = _ansIcons.IndexOf(bot.AnswerIcon);
        if (question.Answers[botAnsw].RightVariant==true)
        {
            CountWinBot(question.Coins+speed, bot);
        }
        else
        {
            CountLooseBot(bot);
        }
       
    }

    private void CountLooseBot(PlayersAnswered bot)
    {
        bot.WinLevel = 0;
        if (bot.LoseStreak == 0)
        {
            bot.LoseStreak = 1;
        }
        else if (bot.LoseStreak == 1)
        {
            bot.LoseStreak = 2;
        }
        else
        {
            bot.LoseStreak += LoseStreak;
           
        }
        Debug.Log("BOT LOOSE "+bot.Name);
    }

    private void CountWinBot(float coins, PlayersAnswered bot)
    {
        bot.WinLevel++;

        var winlevel = bot.WinLevel;
        var playerCoins = bot.Coins;
        if (bot.LoseStreak > 0)
        {
            var loseStreak = bot.LoseStreak;
           coins *= loseStreak;

            bot.LoseStreak = 0;
        }

        var winsStreak = _playerSettings.WinStreaks.FindLast(x => x.QuestionAmount <= winlevel);
        if (winsStreak != null)
            coins *= winsStreak.IncreaseAmount;

        playerCoins += coins;

        playerCoins = (float)Math.Round(playerCoins, 0);
        bot.Coins = playerCoins;
        Debug.Log("BOT WIN "+bot.Name);
    }

    public char BotChoose(PlayersAnswered bot, Question question, float maxCoins)
    {
        char icon = ' ';
        float botChance = _playerSettings.BotStartChance;
        float chance = bot.Coins > maxCoins ? botChance-_playerSettings.BotChance : botChance+_playerSettings.BotChance;
     Debug.Log("CHANCE BOT = "+ chance);
        //  int botChance = _playerSettings.BotChance;
        var rnd = Random.Range(0, 100);
        if (rnd <= chance)
        {
            bot.RightAnswers++;  
            icon = _ansIcons[question.Answers.FindIndex(x => x.RightVariant == true)];
        }
        else
        {
            icon = FindWrongAnswer(question, _ansIcons);
        }

        return icon;
    }

    private static char FindWrongAnswer(Question question, char[] ansIcons)
    {
        int rndAns;
        char icon;
        do
        {
            rndAns = Random.Range(0, ansIcons.Length);
            icon = ansIcons[rndAns];
        } while (question.Answers[rndAns].RightVariant == true);

        return icon;
    }
}