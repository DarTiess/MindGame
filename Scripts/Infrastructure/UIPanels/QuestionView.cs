using System.Linq;
using DG.Tweening;
using Infrastructure.UIPanels.Answers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Infrastructure.UIPanels
{
    public class QuestionView: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _questionTxt;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private AnswerHistoryPanel _answerHistoryPanel;
        [SerializeField] private float _bounceEffect;
        private TMP_FontAsset _normalTextStyle;
        private bool _started;

        public void Restart(TMP_FontAsset tmpFontAsset)
        {
            _questionTxt.transform.DORotate(new Vector3(0, 0, 0f), 0.01f);
           ChangeFont(tmpFontAsset);
        }

        public void SetText(string questText, float moveDuration, float resizeDuration , Ease ease)
        {
            _answerHistoryPanel.Hide();
            _questionTxt.gameObject.SetActive(false); 
            _questionTxt.text = questText;
            if (!_started)
            {
                transform.DOLocalMoveY(25, moveDuration);
                _started = true;
            }
            _rect.DOSizeDelta(new Vector2(916f, 22f), 0.1f)
                .OnComplete(() =>
                {
                    _rect.DOSizeDelta(new Vector2(916f, 549+(549/_bounceEffect)), resizeDuration)
                        .SetEase(ease)
                        .OnComplete(() =>
                        {
                            _rect.DOSizeDelta(new Vector2(916f, 549f), 0.07f);
                            _questionTxt.gameObject.SetActive(true);
                            //  _answerHistoryPanel.Show();
                       
                        });
                });
            // 
         
        }

        public void MakeBomb()
        {
            string vowels = "уеаоыэяиюУЕЫАОЭЯИЮ";

            string questText = _questionTxt.text;
            questText = new string(questText.Where(c => !vowels.Contains(c)).ToArray());
            _questionTxt.text = questText;
        }

        public void RotateText(float animationDuration)
        {
            _questionTxt.transform.DORotate(new Vector3(0, 0, -180f), animationDuration);
        }

        public void ChangeFont(TMP_FontAsset comicSansFont)
        {
            _questionTxt.font = comicSansFont;
        }
    }
}