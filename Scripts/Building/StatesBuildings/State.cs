using Building.Trigger;
using UnityEngine;

namespace Building.StatesBuildings
{
    public abstract class State
    {
        protected readonly StateMachine _stateMachine;
        protected SpriteRenderer _buildView;
        protected BuildDisplay _buildDisplay;
        protected ITriggerObserver _trigger;
      
        protected State(StateMachine stateMachine,
            BuildDisplay buildDisplay,ITriggerObserver trigger,
            SpriteRenderer buildObj)
        {
            _stateMachine = stateMachine;
            _buildView = buildObj;
            _buildDisplay = buildDisplay;
            _trigger = trigger;
        }

        public virtual void Enter(){}
        public virtual void Exit(){}


        public virtual void ShowInfo() {}

        public virtual void Touch(Ray ray, Camera camera){}
    }
}