using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Building
{
    public class BuildDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _prizeText;
        [SerializeField] private Image _finishedIcon;

        [FormerlySerializedAs("_errorPanel")] 
        [SerializeField] private Image _prizePanel;

        private bool _isFinished;

        private void Start()
        {
            _finishedIcon.gameObject.SetActive(false);
        }

        public void Show(int prize)
        {
            _prizeText.color = Color.black;
            _prizeText.text = prize.ToString();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Finished()
        {
            gameObject.SetActive(true);
            _prizePanel.gameObject.SetActive(false);
            _finishedIcon.gameObject.SetActive(true);
            _isFinished = true;
        }

        public void NotEnough(int prize)
        {
            if (_isFinished)
                return;
            gameObject.SetActive(true);
            _prizeText.color = Color.red;
            _prizeText.text = prize.ToString();
            _prizePanel.gameObject.SetActive(true);
            _prizePanel.DOFade(0, 0.5f)
                .OnComplete(() =>
                {
                    _prizePanel.gameObject.SetActive(false);
                    _prizePanel.DOFade(1, 0.01f);
                    gameObject.SetActive(false);
                });
        }
    }
}