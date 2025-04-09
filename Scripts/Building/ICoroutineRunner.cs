using System.Collections;
using UnityEngine;

namespace Building
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
        void OnStopCoroutine(IEnumerator coroutine);
    }
}