using System;

namespace Infrastructure.Installers.Settings.Quiz
{
    [Serializable]
    public class QuestionSheets
    {
        public string Question;
        public string RightAnswer;
        public string WrongAnswer1;
        public string WrongAnswer2;
        public string WrongAnswer3;
    }
}