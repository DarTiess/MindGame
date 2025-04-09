using System;
using UnityEngine;

namespace Infrastructure.Installers.Settings.Quiz
{
    [Serializable]
    public class Answer
    {
        [SerializeField] private string description;
        [SerializeField] private bool rightVariant;
    
        public string Description { get => description; set => description = value; }
        public bool RightVariant { get => rightVariant; set => rightVariant = value; }
        
    }
}