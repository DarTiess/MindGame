using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Installers.Settings.Quiz
{
    [System.Serializable]
    public class QuestionsList
    {
        [SerializeField] private List<Question> questions=new List<Question>();

        public List<Question> Questions { get => questions; set => questions = value; }
    }
}