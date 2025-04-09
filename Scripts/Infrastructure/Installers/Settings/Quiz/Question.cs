using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Installers.Settings.Quiz
{
    [Serializable]
    public class Question: IDisposable
    {
        [SerializeField] private string quest;
        [SerializeField] private List<Answer> answers;
        [SerializeField] private int coins=10;
        public string Quest { get => quest; set => quest = value; }
        public List<Answer> Answers
        {
            get =>
                answers;
            set => answers = value;
        }
        public int Coins => coins;

        public void Dispose()
        {
            answers.Clear();
        }
    }
}