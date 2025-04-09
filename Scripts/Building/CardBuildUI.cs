using System;
using System.Collections.Generic;
using Building.StatesBuildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Building
{
    [RequireComponent(typeof(Button))]
    public class CardBuildUI : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _isReadyImage;
        [SerializeField] private Image _buildImage;
        [SerializeField] private TMP_Text _prizeTxt;
        [SerializeField] private Image _background;
        [SerializeField] private Sprite _activeUI;
        [SerializeField] private Sprite _desactiveUI;
        private int _indexImage;
        private Button _cardButton;
        private bool _isDone;
        public event Action OnActivateBuild;

        private void Start()
        {
            _cardButton = GetComponent<Button>();
            _cardButton.onClick.AddListener(ActivateBuild);
        }

        private void ActivateBuild()
        {
            if(_isDone)
                return;
            OnActivateBuild?.Invoke();
            OnClicked();
        }

        public void Initialize(BuildState buildState)
        {
            _indexImage = (int)buildState.BuildType;
            _background.sprite = _desactiveUI;
            _buildImage.sprite = buildState.StartBuildSprite;
            _prizeTxt.text = buildState.Prize.ToString();
            for (int i = 0; i <(int)buildState.BuildType; i++)
            {
                _isReadyImage[i].SetActive(true);
            }
        }

        public void OnClicked()
        {
            _background.sprite = _activeUI;
        }

        public void Done()
        {
            for (int i = 0; i < _isReadyImage.Count; i++)
            {
                _isReadyImage[i].SetActive(true);
            }

            _isDone = true;
        }

        public void Ready()
        {
            if(_indexImage>0)
                _isReadyImage[_indexImage-1].SetActive(true);
        }
    }
}