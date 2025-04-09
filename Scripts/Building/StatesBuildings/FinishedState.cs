using Building.Trigger;
using UnityEngine;

namespace Building.StatesBuildings
{
    public class FinishedState : State
    {
        private bool _activeState;
        private Sprite _endBuildSprite;

        public FinishedState(StateMachine stateMachine,
            BuildDisplay buildDisplay, ITriggerObserver trigger,
            SpriteRenderer buildObj, BuildState buildState) : base(stateMachine, buildDisplay, trigger, buildObj)
        {
            _endBuildSprite = buildState.EndBuildSprite;
        }

        public override void Enter()
        {
            _activeState = true;
            _trigger.DisableTrigger();
            _buildView.sprite = _endBuildSprite;
            _buildDisplay.Finished();
            Debug.Log("FinishedState ENTER");
        }
    }
}