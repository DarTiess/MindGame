using System.Collections.Generic;
using System.IO;
using System.Linq;
using GoogleSheets;
using Network;
using UnityEngine;
using Random = System.Random;

namespace Infrastructure.Installers.Settings.Quiz
{
    [System.Serializable]
    public class QuestionsSettings
    {
        [SerializeField] private QuestionsList questionsList;
        private TextAsset filePath;
        private List<string> _chunks = new List<string>();
        public QuestionsList Questions
        {
            get
            {
                return questionsList;
            }
            set => questionsList = value;
        }

        public TextAsset FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
            }
        }


        public void SetQuestionsList()
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
            Debug.Log("Set Quest List");
            var questions = JsonUtility.FromJson<SpreadshetContent>(json);
           
            if(questionsList.Questions.Count>0)
                questionsList.Questions.Clear();
         
            List<QuestionSheets> sheetsList = questions.QuestionSheetsList;
            var tempList = new List<Question>();
            for (int i=0;i<sheetsList.Count;i++)
            {
                Question quest = new Question
                {
                    Quest = sheetsList[i].Question
                };
                var answ1 = new Answer { Description = sheetsList[i].RightAnswer, RightVariant = true };
                var answ2 = new Answer { Description = sheetsList[i].WrongAnswer1 };
                var answ3 = new Answer { Description = sheetsList[i].WrongAnswer2 };
                var answ4 = new Answer { Description = sheetsList[i].WrongAnswer3 };

                  
                List<Answer> answers = new List<Answer> { answ1, answ2, answ3, answ4 };
                
                var rnd = new Random();
                quest.Answers = answers.OrderBy(item => rnd.Next()).ToList();
               
                tempList.Add(quest);
            }
            
            
            var rndQuest = new Random();
            questionsList.Questions =  tempList.OrderBy(item => rndQuest.Next()).ToList();
        }

        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public static QuestionsSettings Deserialize(string json)
        {
            return JsonUtility.FromJson<QuestionsSettings>(json);
        }

        public void AddChunk(string chunk, int index, int totalChunks)
        {
            if (_chunks.Count == 0)
            {
                _chunks = new List<string>(new string[totalChunks]);
            }
            _chunks[index] = chunk;
            if (!_chunks.Contains(null))
            {
                string combined = NetworkUtils.CombineStrings(_chunks);
                var deserialized = JsonUtility.FromJson<QuestionsSettings>(combined);
                questionsList = deserialized.questionsList;
                filePath = deserialized.filePath;
            }
        }
    }
}