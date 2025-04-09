using System;
using UnityEngine.Serialization;

namespace Infrastructure.Installers.Settings.Players
{
    [Serializable]
    public class WinStreak
    {
        public int Level;
        [FormerlySerializedAs("Level")] public int QuestionAmount;
        [FormerlySerializedAs("Amount")] public float IncreaseAmount;
    }
}