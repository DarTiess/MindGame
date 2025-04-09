using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using UnityEngine;
using Zenject;

namespace Infrastructure.Level
{
    public class LoadBaseScene: MonoBehaviour
    {
        private IEventBus _eventBus;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Start()
        {
            _eventBus.Invoke(new LevelStart());
        }
    }
}