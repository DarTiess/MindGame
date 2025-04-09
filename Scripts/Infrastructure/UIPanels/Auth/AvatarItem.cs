using System;
using Infrastructure.UIPanels.PlayerPanel;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.Auth
{
    public class AvatarItem:MonoBehaviour
    {
      //  private Image _image;
           [SerializeField]  private SkeletonView _skeletonView;
       [SerializeField] private Button _button;
        private int _indexAvatar;
        public event Action<int> MakeChooseAvatar;

        public void SetSprite( string skinAvatar, int indexAvatar)
        {
          //  _skeletonGraphic = GetComponent<SkeletonGraphic>();
      // _skeletonGraphic.enabled = true;
          _skeletonView.SetSkeletonSkin(skinAvatar);
        //  _button = GetComponent<Button>();
          _button.onClick.AddListener(ChooseAvatar);
          _indexAvatar = indexAvatar;
        }

        private void ChooseAvatar()
        {
            Debug.Log("CLICKED AVATAR");
            MakeChooseAvatar?.Invoke(_indexAvatar);
        }

        public void Show()
        {
            //_skeletonGraphic.skeletonDataAsset.
        }
    }
}