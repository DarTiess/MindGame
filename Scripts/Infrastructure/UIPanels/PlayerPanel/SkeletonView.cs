using System.Threading.Tasks;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Infrastructure.UIPanels.PlayerPanel
{
    public class SkeletonView: MonoBehaviour
    {
        [SerializeField] private SkeletonGraphic _skeleton;

        public void SetSkeletonSkin(string skinAvatar)
        {
            Skeleton skeleton = _skeleton.Skeleton;
            SkeletonData skeletonData = skeleton.Data;
         //   var characterSkin = new Skin("character-base");
            //characterSkin.AddSkin(skeletonData.FindSkin(skinAvatar));
            skeleton.SetSkin(skeletonData.FindSkin(skinAvatar));
            skeleton.SetSlotsToSetupPose();

        }

        public async void StartAnimation(int time)
        {
            await Task.Delay(time*200);
            //   _skeleton.AnimationState.SetAnimation(0, "empty", true); 
            _skeleton.AnimationState.SetAnimation(0, "start", false); 
            _skeleton.AnimationState.AddAnimation(0, "idle", true, 1); 
        }

        public void Jump(int i)
        {
            if (i < 0)
            {
                _skeleton.AnimationState.SetAnimation(0, "come_L", false); 
               // _skeleton.AnimationState.AddAnimation(0, "idle", true, 1); 
            }
            else
            {
                _skeleton.AnimationState.SetAnimation(0, "come_R", false); 
            }
        }

        public void IdleAnimation()
        {
            _skeleton.AnimationState.AddAnimation(0, "idle", true, 1); 
        }

        public async void FreezeAnimation(float freezeTimer)
        {
            _skeleton.AnimationState.SetAnimation(0, "freez_activate", true);
            await Task.Delay((int)(freezeTimer * 1000));
            _skeleton.AnimationState.SetAnimation(0, "freez_ending", false);
            IdleAnimation();

        }

        public void BombAnimation()
        {
            _skeleton.AnimationState.SetAnimation(0, "exp_bomb", false);
            IdleAnimation();
        }

        public void IdleBombAnimation()
        {
            _skeleton.AnimationState.SetAnimation(0, "idle_bomb", true);
        }

        public void IdleFreezeAnimation()
        {
            _skeleton.AnimationState.SetAnimation(0, "freez_debuff", true);

        }

        public void Reflecting()
        {
            Debug.Log("REFLECTING SKELETON ANIM");
        }
    }
}