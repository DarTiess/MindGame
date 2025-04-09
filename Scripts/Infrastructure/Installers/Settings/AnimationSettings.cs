using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AnimationSettings
{
    [Header("PlayButtonAnimation")] 
    [SerializeField] private float _playBtnClickedPosition;
    [SerializeField] private float  _playBtnDuration;
    [SerializeField] private Ease  _playBtnEase;
    
    [Header("FinishPanelAnimation")] 
    [SerializeField] private float _finishPanelCoinsDuration;
    [SerializeField] private float animationScale;
    [SerializeField] private float _finishPanelCoinsRotateAngle;

    [Header("PlayersPanelAnimation")] 
    [SerializeField] private float _playerUIDuration; 
    [SerializeField] private float playerUICoinsRotateAngle;
    [SerializeField] private float _playerUIResizeDuration;
    [SerializeField] private float _playerUICoinsShakeDuration;
    [SerializeField] private float _playerUICoinsChangeDuration;
    [SerializeField] private Ease _playerUIJumpEase;
   
   
    [Header("Question")]
    [SerializeField] private float questionMoveDuration; 
    [SerializeField] private float questionResizeDuration;
    [SerializeField] private Ease _questionEase;

    [Header("Answer")]
    [SerializeField] private float _answerScaleDuration;
    [SerializeField] private float _answerClickedDuration;
    [SerializeField] private float _answerClickedScale;
    [SerializeField] private Ease _answerEase;

    [Header("Timer")] 
    [SerializeField] private float _timerClockScale;
    [SerializeField] private float _timerClockScaleDuration;
    [SerializeField] private float _timerClockHandSpeed;
    [SerializeField] private Ease _clockEase;
    [SerializeField] private Ease _timerEase;

    public float PlayBtnClickedPosition => _playBtnClickedPosition;
    public float PlayBtnDuration => _playBtnDuration;
    public Ease PlayBtnEase => _playBtnEase;
    public float FinishPanelCoinsDuration => _finishPanelCoinsDuration;
    public float FinishPanelCoinsRotateAngle => _finishPanelCoinsRotateAngle;
    public float PlayerUIDuration => _playerUIDuration;
    public float PlayerUICoinsRotateAngle => playerUICoinsRotateAngle;
    public Ease PlayerUIJumpEase => _playerUIJumpEase;
    public float PlayerUIResizeDuration => _playerUIResizeDuration;
    public float PlayerUICoinsShakeDuration => _playerUICoinsShakeDuration;
    public float PlayerUICoinsChangeDuration => _playerUICoinsChangeDuration;
    public float QuestionMoveDuration => questionMoveDuration;
    public float QuestionResizeDuration => questionResizeDuration;
    public Ease QuestionEase => _questionEase;
    public float AnswerScaleDuration => _answerScaleDuration;
    public float AnswerClickedDuration => _answerClickedDuration;
    public float AnswerClickedScale => _answerClickedScale;
    public Ease AnswerEase => _answerEase;
    public float TimerClockScale => _timerClockScale;
    public float TimerClockScaleDuration => _timerClockScaleDuration;
    public float TimerClockHandSpeed => _timerClockHandSpeed;
    public Ease ClockEase => _clockEase;
    public Ease TimerEase => _timerEase;
}