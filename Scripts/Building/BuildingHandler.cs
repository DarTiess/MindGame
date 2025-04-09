using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using Infrastructure.Installers.Settings.Build;
using UnityEngine;
using Zenject;

namespace Building
{
    public class BuildingHandler : MonoBehaviour
    {
        [SerializeField] private List<BuildView> _buildings;
        [SerializeField] private List<CardBuildUI> _buildingsUI;
        [SerializeField] private MaskSprite _maskSprite;
        [SerializeField] private int _poolSize;
        private ObjectPoole<MaskSprite> _poole;
        private BuildsService _buildsService;
        private Vector3 _lastMaskPosition;
        public event Action FinishedDraw;

        [Inject]
        public void Construct(BuildsService buildsService)
        {
            _buildsService = buildsService;
        }

        private void Start()
        {
            _poole = new ObjectPoole<MaskSprite>();
            _poole.CreatePool(_maskSprite, _poolSize, transform);
            if (_buildings == null)
                _buildings = FindObjectsOfType<BuildView>().ToList();
            
            for (int i = 0; i < _buildings.Count; i++)
            {
                _buildings[i].Initialize(this, _buildsService, i, _buildingsUI[i]);
            }
            _lastMaskPosition = Vector3.zero;
        }

        public void GetMask(Vector3 position)
        {
            if (_lastMaskPosition == Vector3.zero)
            {
                PlaceMask(position);
            }
            else
            {
                Vector3 offset = (position - _lastMaskPosition) / 2f;
                Vector3 newPosition = _lastMaskPosition + offset;
                PlaceMask(newPosition);
            }
        }
        private void PlaceMask(Vector3 position)
        {
            var mask = _poole.GetObject();
            mask.transform.position = position;
            _lastMaskPosition = position; 
        }
        public void HideMask()
        {
            _poole.HideAll();
            FinishedDraw?.Invoke();
        }

        public void IsOpened(int number)
        {
            if(number +1>=_buildings.Count)
                return;
            _buildings[number + 1].OpeneBuild();
        }
    }
}