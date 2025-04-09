using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.PlayerPanel
{
    public class Colum : MonoBehaviour
    {
        [SerializeField] private Image _progressBar;
        [SerializeField] private RectTransform _playerPosition;
        [SerializeField] private float _hightStatus;
        [SerializeField] private float _mediumStatus;
        [SerializeField] private float _lowStatus;
        [SerializeField] private float _nullStatus;
        [SerializeField] private Sprite _hightIcon;
        [SerializeField] private Sprite _mediumIcon;
        [SerializeField] private Sprite _lowIcon;
        [SerializeField] private Sprite _nullIcon;

        public RectTransform PlayerPosition => _playerPosition;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetStatus(ColumStatus status)
        {
            switch (status)
            {
                case ColumStatus.Hight:
                     _progressBar.transform.DOLocalMoveY(_hightStatus, 0.5f);
                    break;
                case ColumStatus.Medium:
                    _progressBar.transform.DOLocalMoveY(_mediumStatus, 0.5f);
                    break;
                case ColumStatus.Low:
                    _progressBar.transform.DOLocalMoveY(_lowStatus, 0.5f);
                    break;
                case ColumStatus.None:
                    _progressBar.transform.DOLocalMoveY(_nullStatus, 0.5f);
                    break;
                default:
                    Debug.Log("No such status colum");
                    break;
            }
        }

        public void SetIcon(int i)
        {
            switch (i)
            {
                case 0:
                    _progressBar.sprite = _hightIcon;
                    break;
                case 1:
                    _progressBar.sprite = _mediumIcon;
                    break;
                case 2:
                    _progressBar.sprite = _lowIcon;
                    break;
                case 3:
                    _progressBar.sprite = _nullIcon;
                    break;
            }
        }
    }
}