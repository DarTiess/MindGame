using UnityEngine;
using UnityEngine.Serialization;

namespace Network.NetworkObjects
{
    [System.Serializable]
    public class PlayersAnswered
    {
        public string Name;
        public float Coins;
        public float LoseStreak;
        public float Total;
        public string AvatarName;
        public char AnswerIcon;
        public bool IsBot;
        public int WinLevel;
        public int RightAnswers; 
        public bool IsAttackedFreeze;
        public bool IsAttackedBomb;
        public bool MakeAttacked;
        public bool IsAttackComic;
        public bool IsAttackedRotate;
        public bool IsMirror;

        public string GetAvatarSprite()
        {
            return AvatarName;
           // return Resources.Load<Sprite>(AvatarName);
        }
    }
}