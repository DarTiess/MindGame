using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.Answers
{
    public class AnswerHistoryPanel: MonoBehaviour
    {
        [SerializeField] private List<Image> _answerHystory;
        [SerializeField] private Sprite _rightAnswered;
        [SerializeField] private Sprite _wrongAnswered;
        private int _index=0;
        public void MakeInscribe(bool isRight)
        {
            var answeredSprite = isRight ? _rightAnswered : _wrongAnswered;
            _answerHystory[_index].sprite = answeredSprite;
            _index++;
            
            if (_index >= _answerHystory.Count)
                _index = 0;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}