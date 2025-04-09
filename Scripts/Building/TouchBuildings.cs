using UnityEngine;
using UnityEngine.EventSystems;

namespace Building
{
    public class TouchBuildings : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        private bool _isClicked;
        private BuildView _startBuildViewView;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isClicked)
                return;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10000f))
            {
                if (hit.transform.gameObject.TryGetComponent(out BuildView scratchEffect))
                {
                    _startBuildViewView = scratchEffect;
                    _isClicked = _startBuildViewView.ShowInfo();
                    if (_isClicked)
                    {
                        _startBuildViewView.FinishedDraw += OnFinishedDraw;
                    }
                    Debug.DrawLine(ray.origin, hit.point, Color.red);
                }
            }
        }

        private void OnFinishedDraw(BuildView buildView)
        {
            buildView.FinishedDraw -= OnFinishedDraw;
            _isClicked = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isClicked)
                return;
            Ray ray = _camera.ScreenPointToRay(eventData.position);
            _startBuildViewView.Touch(ray, _camera);
        }
    }
}