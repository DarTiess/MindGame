using System;
using TMPro;
using UnityEngine;

namespace Infrastructure.UIPanels
{
    public class InputNamePanel : PanelBase
    {
        [SerializeField] private TMP_InputField inputName;
        
        public override event Action ClickedPanel;
        public event Action<string> InputName;

        private void OnEnable()
        {
            HideButton();
        }

        private void Update()
        {
            if (inputName.text != "")
            {
                ShowButton();
            }
        }

        protected override void OnClickedPanel()
        {
           ClickedPanel?.Invoke();
           InputName?.Invoke(inputName.text);
        }
    }
}