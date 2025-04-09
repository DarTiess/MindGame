using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels
{
    public class TimerView: MonoBehaviour
    {
        [SerializeField] private Slider _timer;
        [SerializeField] private GameObject _clocks;
        [SerializeField] private GameObject _clockHand;
        private float _clockHandSpeed;
        private float _scaleClockValue;
        private float _scaleAnimationDuration;
        private float _maxTimeToAnswer;
        private bool _move;
        private Ease _clockEase;
        private Ease _timerEase;

        public event Action Completed;
        public float RestTime =>_maxTimeToAnswer - _timer.value;

        private void Update()
        {
            _clockHand.transform.RotateAround(_clockHand.transform.position, Vector3.back, _clockHandSpeed * Time.deltaTime);
        }

        public void Initialize(float maxTimeToAnswer, float scaleClockValue, float scaleDuration, float clockHandSpeed, Ease timerEase, Ease clockEase)
        {
            _maxTimeToAnswer = maxTimeToAnswer;
            _scaleClockValue = scaleClockValue;
            _scaleAnimationDuration = scaleDuration;
            _clockHandSpeed = clockHandSpeed;
            _timerEase = timerEase;
            _clockEase = clockEase;
            _timer.maxValue = _maxTimeToAnswer;
            _timer.value = 0;
        }

        public void SetTimer()
        {
            DOTween.Kill(_timer);
            _timer.value = 0;
            _move = true;
          //  _clockHand.transform.RotateAround(new Vector3(0,0,-360f));
            _timer.DOValue(_maxTimeToAnswer, _maxTimeToAnswer).SetEase(_timerEase)
                .OnComplete(() =>
                {
                    Completed?.Invoke();
                });
            DOVirtual.DelayedCall(_maxTimeToAnswer / 6, ScaleClocks);
            DOVirtual.DelayedCall((_maxTimeToAnswer * 5) / 6, ScaleClocks);
        }

        private void ScaleClocks()
        {
            _clocks.transform.DOScale(new Vector3(_scaleClockValue, _scaleClockValue, _scaleClockValue), _scaleAnimationDuration).SetLoops(2, LoopType.Yoyo);
        }
    }
}