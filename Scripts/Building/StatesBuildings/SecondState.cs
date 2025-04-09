using System;
using System.Collections;
using Building.Trigger;
using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using Network;
using TMPro;
using UnityEngine;

namespace Building.StatesBuildings
{
    public class SecondState : State, IDisposable
    {
        private SpriteRenderer _scratchObj;
        private BuildingHandler _buildingHandler;
        private TMP_Text _prizeText;
        private int _prize;
        private int _buildReward;
        private Sprite _scratchSprite;
        private bool _isPressed;
        private bool _active;
        private Transform _origin;
        private int _isTriggered;

        private ICoroutineRunner _coroutine;
        private bool _activeState;
        private IEventBus _eventBus;
        private Sprite _endBuildSprite;
        private Sprite _startBuildSprite;

        public SecondState(StateMachine stateMachine,
            BuildDisplay buildDisplay, ITriggerObserver trigger,
            SpriteRenderer buildObj, BuildState buildState, BuildingHandler buildingHandler,
            SpriteRenderer scratchObj, Transform origin,
            ICoroutineRunner coroutineRunner, IEventBus eventBus) : base(stateMachine, buildDisplay, trigger, buildObj)
        {
            _scratchObj = scratchObj;
            _buildView = buildObj;
            _startBuildSprite = buildState.StartBuildSprite;
            _endBuildSprite = buildState.EndBuildSprite;
            _scratchSprite = buildState.ScratchSprite;
            _buildingHandler = buildingHandler;
            _buildDisplay = buildDisplay;
            _prize = buildState.Prize;
            _buildReward = buildState.BuildReward;

            _scratchSprite = buildState.ScratchSprite;
            _origin = origin;
            _trigger.TriggerEnter += OnTrigger;
            _coroutine = coroutineRunner;
            _eventBus = eventBus;
        }

        public void Dispose()
        {
            _trigger.TriggerEnter -= OnTrigger;
        }


        public override void Enter()
        {
            _scratchObj.sprite = _scratchSprite;
            _buildView.sprite = _startBuildSprite;
            _isTriggered = 0;
            Debug.Log("SecondState ENTER");
        }

        public override void Exit()
        {
            _buildDisplay.Hide();
            _activeState = false;
        }

        public override void ShowInfo()
        {
            _activeState = true;
            _scratchObj.gameObject.SetActive(true);
            _buildView.sprite = _endBuildSprite;
            _buildDisplay.Show(_prize);
        }

        public override void Touch(Ray ray, Camera camera)
        {
            Vector3 rayPoint = ray.GetPoint(Vector3.Distance(_origin.position, camera.transform.position));

            Vector3 position = new Vector3(rayPoint.x, rayPoint.y, rayPoint.z);
            if (_origin.gameObject.activeInHierarchy )
                GetMask(position);
        }

        private void OnTrigger(Collider other)
        {
            if (!_activeState)
                return;
            if (other.gameObject.TryGetComponent(out MaskSprite mask))
            {
                _isTriggered++;
                if (_isTriggered > Constants.BUILD_TRIGGER)
                {
                    _buildingHandler.HideMask();
                    _scratchObj.gameObject.SetActive(false);
                    _eventBus.Invoke(new FinishedBuild(_prize, _buildReward));
                    _stateMachine.SetState<ThirdState>();
                }
            }
        }

        private void GetMask(Vector3 position)
        {
            _buildingHandler.GetMask(position);
        }
    }
}