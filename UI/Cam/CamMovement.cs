using UnityEngine;
using System;

/*
 * Move Camera Script
 * Must be refactored
 * ... and splited in AnimationCamera, 2DCamera, 3DCamera, etc.
 */
namespace TinyRoar.Framework
{
    [RequireComponent(typeof (CamConfig))]
    public abstract class CamMovement : MonoBehaviour, ICam
    {

        [SerializeField]
        private int mouseMinY = -999;
        [SerializeField]
        protected Camera _cameraComponent;

        private bool _drag;
        protected Vector3 _origin;
        public bool _isEnabled;

        protected virtual void Awake()
        {
            if (_cameraComponent == null)
                _cameraComponent = this.GetComponent<Camera>();
        }

        protected virtual void Start()
        {
            UpdateMovement();
        }

        public virtual void DoEnable()
        {
            _isEnabled = true;
            Inputs.Instance.OnLeftMouseMoveLate -= OnLeftMouseMove;
            Inputs.Instance.OnLeftMouseMoveLate += OnLeftMouseMove;
            Inputs.Instance.OnLeftMouseUp -= OnLeftMouseUp;
            Inputs.Instance.OnLeftMouseUp += OnLeftMouseUp;
        }

        private void OnLeftMouseUp()
        {
            _drag = false;
        }

        public virtual void DoDisable()
        {
            _isEnabled = false;
            Inputs.Instance.OnLeftMouseMoveLate -= OnLeftMouseMove;
            Inputs.Instance.OnLeftMouseUp -= OnLeftMouseUp;
        }

        void OnDestroy()
        {
            try
            {
                DoDisable();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        public void UpdateMovement()
        {
            OnLeftMouseMove();
        }

        private void OnLeftMouseMove()
        {
            if (Camera.main == null)
                return;

            if (_drag == false)
            {
                _drag = true;
                _origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            // int variable
            var mousePos = Vector2.zero;

            // get mouse pos
            mousePos.x = Input.GetAxis("Mouse X");
            mousePos.y = Input.GetAxis("Mouse Y");

            if (Input.touchCount > 0)
            {

                if (
                    Input.GetTouch(0).phase == TouchPhase.Moved
                    ||
                    (Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Moved)
                )
                {
                    mousePos.x = Input.touches[0].deltaPosition.x;
                    mousePos.y = Input.touches[0].deltaPosition.y;
                    mousePos *= _cameraComponent.orthographicSize / 25;
                }
                else
                {
                    return;
                }

            }

            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < mouseMinY)
                return;

            mousePos *= -1;
            DoAction(mousePos);
        }

        protected abstract void DoAction(Vector2 mousePos);

    }
}