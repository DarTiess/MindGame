using System;
using System.Collections.Generic;
using UnityEngine;

namespace Building.StatesBuildings
{
    public class StateMachine
    {
        private State _currentState;
        private Dictionary<Type, State> _states = new Dictionary<Type, State>();
        public event Action<State> OnChangeState;

        public void AddState(State state)
        {
            _states.Add(state.GetType(), state);
        }

        public void SetState<T>() where T : State
        {
            var type = typeof(T);
            if (_currentState != null && _currentState.GetType() == type)
            {
                return;
            }

            if (_states.TryGetValue(type, out var newstate))
            {
                ChangeCurrentState(newstate);
            }
        }
        
        public void ShowInfo()
        {
            _currentState?.ShowInfo();
        }

        public void Touch(Ray ray, Camera camera)
        {
            _currentState?.Touch(ray, camera);
        }

        public void SetSaveState(State state)
        {
            if (_currentState != null)
            {
                return;
            }
            ChangeCurrentState(state);
        }

        private void ChangeCurrentState(State state)
        {
            OnChangeState?.Invoke(state);
            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }
    }
}