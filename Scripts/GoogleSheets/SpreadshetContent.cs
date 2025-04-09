using System;
using System.Collections.Generic;
using Infrastructure.Installers.Settings.Quiz;

namespace GoogleSheets
{
    [Serializable]
    public class SpreadshetContent
    {
        public List<QuestionSheets> QuestionSheetsList;
    }
}