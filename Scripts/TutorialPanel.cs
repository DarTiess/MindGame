using System;
using System.Collections;
using DG.Tweening;
using Infrastructure.UIPanels.Answers;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class TutorialPanel: MonoBehaviour
{
    [SerializeField] private GameObject _handObject;
    [SerializeField] private ScratchCardMaskUGUI _scratchCardMask;
    private Sequence _seq;
    public event Action FinishedFreeze;


    public void Show(float freezeTimer)
    {
        _scratchCardMask.gameObject.SetActive(true);
        Debug.Log("MakeFreeze");
        gameObject.SetActive(true);
        _scratchCardMask.RevealProgressChanged += Finished;
        var startPositionY = _handObject.transform.position.y;
        var startPositionX = _handObject.transform.position.x;
       _seq = DOTween.Sequence(); 
       _seq.Append(_handObject.transform.DOMoveY(startPositionY-280.3f, 0.5f)).SetLoops(-1,LoopType.Yoyo);
       _seq.Insert(0, _handObject.transform.DOMoveX(startPositionX - 250.3f, 0.5f)).SetLoops(-1,LoopType.Yoyo);
        StartCoroutine(Freeze(freezeTimer));
    }

    private void Finished(float progress, bool revealed)
    {
        if (revealed)
        {
            _seq.Kill(_handObject);
            FinishedFreeze?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private IEnumerator Freeze(float freezeTimer)
    {
        yield return new WaitForSeconds(freezeTimer);
      //  DOTween.Kill(_handObject.transform);
        _seq.Kill(_handObject);
       // DOTween.Kill(_seq);
        gameObject.SetActive(false);
    }
}