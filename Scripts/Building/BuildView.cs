using System;
using System.Collections;
using System.Collections.Generic;
using Building.StatesBuildings;
using Building.Trigger;
using Infrastructure.EventsBus;
using Infrastructure.Installers.Settings.Build;
using Infrastructure.Installers.Settings.MainPlayer;
using UnityEngine;
using Zenject;
using StateMachine = Building.StatesBuildings.StateMachine;

namespace Building
{
    public class BuildView : MonoBehaviour, ICoroutineRunner, ITriggerObserver, IDisposable
    {
        [SerializeField] private BuildDisplay _buildDisplay;
        [SerializeField] private SpriteRenderer _scratchObject;
        [SerializeField] private SpriteRenderer _buildObj;

        [SerializeField] private List<BuildState> _buildStates;

        private BoxCollider _collider;
        private StateMachine _stateMachine;
        private BuildingHandler _buildingHandler;
        private IEventBus _eventBus;

        private BuildsService _buildService;
        private int _number;
        private State _currentState;
        private List<State> _statesList;
        private CardBuildUI _cardCardBuildUI;
        private BuildType _stateUI = BuildType.StartState;
        private IMainPlayerLoader _mainPlayerLoader;
        private bool _isOpened;

        public event Action<Collider> TriggerEnter;
        public event Action<BuildView> FinishedDraw;

        [Inject]
        public void Construct(IEventBus eventBus, IMainPlayerLoader mainPlayerLoader, BuildsService buildsService)
        {
            _eventBus = eventBus;
            _mainPlayerLoader = mainPlayerLoader;
            _buildService = buildsService;
        }

        private void Start()
        {
            _collider = GetComponent<BoxCollider>();
            _buildDisplay.Hide();
            _scratchObject.gameObject.SetActive(false);
        }

        public void Initialize(BuildingHandler buildingHandler, BuildsService buildsService, int index,
            CardBuildUI cardBuildUI)
        {
            _buildingHandler = buildingHandler;
            _number = index;
            if (_number ==0)
            {
                _isOpened = true;
            }
            else
            {
               // if(!_isOpened)
                  //  _buildObj.color=Color.grey;
            }
            _buildingHandler.FinishedDraw += OnFinishedDraw;
            _cardCardBuildUI = cardBuildUI;
            _cardCardBuildUI.OnActivateBuild += ActivateBuild;
            CreateStateMachine(_buildingHandler);
        }

        public bool ShowInfo()
        {
            if (!_isOpened)
                return false;
            if (_mainPlayerLoader.PlayerCoins >= _buildStates[(int)_stateUI].Prize)
            {
                _stateMachine.ShowInfo();
                _cardCardBuildUI.OnClicked();
                return true;
            }
            else
            {
                _buildDisplay.NotEnough(_buildStates[(int)_stateUI].Prize);
            }

            return false;
        }

        public void OnStopCoroutine(IEnumerator getMask)
        {
            StopCoroutine(getMask);
        }

        public void EnableTrigger()
        {
            _collider.enabled = true;
        }

        public void DisableTrigger()
        {
            if (_collider == null)
            {
                _collider = GetComponent<BoxCollider>();
            }

            _collider.enabled = false;
            _scratchObject.gameObject.SetActive(false);
            _buildingHandler.IsOpened(_number);
            _cardCardBuildUI.Done();
        }

        public void Touch(Ray ray, Camera camera)
        {
            if(_isOpened)
                _stateMachine.Touch(ray, camera);
        }

        public void Dispose()
        {
            _stateMachine.OnChangeState -= ChangeState;
        }

        private void ActivateBuild()
        {
            if(_isOpened)
                ShowInfo();
        }

        private void OnTriggerEnter(Collider other) =>
            TriggerEnter?.Invoke(other);

        private void OnFinishedDraw()
        {
            FinishedDraw?.Invoke(this);
        }

        private void CreateStateMachine(BuildingHandler buildingHandler)
        {
            _stateMachine = new StateMachine();
            var startState = new StartState(_stateMachine, _buildDisplay, this, _buildObj, _buildStates[0],
                buildingHandler, _scratchObject, transform, this, _eventBus);
            var secondState = new SecondState(_stateMachine, _buildDisplay, this, _buildObj, _buildStates[1],
                buildingHandler, _scratchObject, transform, this, _eventBus);
            var thirdState = new ThirdState(_stateMachine, _buildDisplay, this, _buildObj, _buildStates[2],
                buildingHandler, _scratchObject, transform, this, _eventBus);
            var finalState = new FinishedState(_stateMachine, _buildDisplay, this, _buildObj, _buildStates[3]);
            _stateMachine.AddState(startState);
            _stateMachine.AddState(secondState);
            _stateMachine.AddState(thirdState);
            _stateMachine.AddState(finalState);
            if (_buildService.Setting.BuildStates == null)
            {
                _buildService.Setting.CreateBuildState();
            }

            string savedStateName = _buildService.Setting.BuildStates.State[_number];
            Debug.Log(savedStateName);
            if (!string.IsNullOrEmpty(savedStateName))
            {
                Debug.Log("State is EXIST");
                switch (savedStateName)
                {
                    case nameof(StartState):
                        _currentState = startState;
                        _stateUI = BuildType.StartState;
                        break;
                    case nameof(SecondState):
                        OpeneBuild();
                        _currentState = secondState;
                        _stateUI = BuildType.SecondState;
                        break;
                    case nameof(ThirdState):
                        OpeneBuild();
                        _currentState = thirdState;
                        _stateUI = BuildType.ThirdState;
                        break;
                    case nameof(FinishedState): 
                        OpeneBuild();
                        _currentState = finalState;
                        _stateUI = BuildType.FinishedState;
                        break;
                }
            }
            else
            {
                Debug.Log("State NOTis EXIST");
                _currentState = startState;
                _stateUI = BuildType.StartState;
            }

            _buildService.Setting.BuildStates.State[_number] = _stateUI.ToString();
            _cardCardBuildUI.Initialize(_buildStates[(int)_stateUI]);
            _stateMachine.SetSaveState(_currentState);
            _stateMachine.OnChangeState += ChangeState;
        }

        private void ChangeState(State state)
        {
            if (_stateUI == BuildType.FinishedState)
            {
                _cardCardBuildUI.Initialize(_buildStates[(int)_stateUI]);
                _cardCardBuildUI.Done();
            }
            else
            {
                _stateUI++;
                _buildService.ChangeState(_stateUI.ToString(), _number);
                _cardCardBuildUI.Ready();
                _cardCardBuildUI.Initialize(_buildStates[(int)_stateUI]);
            }
        }

        public void OpeneBuild()
        {
          //  _buildObj.color=Color.white;
            _isOpened = true;
        }
    }
}