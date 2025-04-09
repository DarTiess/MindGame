using System;
using UnityEngine;

namespace Infrastructure.UIPanels.Answers
{
    public abstract class ScratchCardMask : MonoBehaviour
    {
        public Action<float, bool> RevealProgressChanged;
        
        public bool IsRevealed => GetRevealProgress() >= revealedAt;
        
        [Header("Mask")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        protected float revealedAt = 0.75f;

        public abstract float GetRevealProgress();

        protected void OnRevealProgressChanged()
        {
            var progress = GetRevealProgress();
            var isRevealed = progress >= revealedAt;
            
            RevealProgressChanged?.Invoke(progress, isRevealed);
        }
    }
}