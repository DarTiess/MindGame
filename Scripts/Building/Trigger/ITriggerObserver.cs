using System;
using UnityEngine;

namespace Building.Trigger
{
    public interface ITriggerObserver
    {
        event Action<Collider> TriggerEnter;
        void EnableTrigger();
        void DisableTrigger();
    }
}