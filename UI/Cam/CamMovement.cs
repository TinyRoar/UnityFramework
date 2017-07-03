using UnityEngine;
using System.Collections;
using TinyRoar.Framework;
using System;
using System.Collections.Generic;

/*
 * Move Camera Script
 * Must be refactored
 * ... and splited in AnimationCamera, 2DCamera, 3DCamera, etc.
 */
namespace TinyRoar.Framework
{
    [RequireComponent(typeof (CamConfig))]
    public class CamMovement : MonoBehaviour, ICam
    {
        [SerializeField] private float MinX;
        [SerializeField] private float MaxX;
        [SerializeField] private float MinY;
        [SerializeField] private float MaxY;

        private float MinXtemp;
        private float MaxXtemp;
        private float MinYtemp;
        private float MaxYtemp;

        [SerializeField] private bool YisZ = true;
        [SerializeField] private Vector2 Speed = new Vector2(1, 1);
        [SerializeField] private Vector2 SpeedMobileMultiply = new Vector2(1, 1);
        [SerializeField] private int mouseMinY = -999;

        // orthographic Cam
        private float OneSizeInWidth = 1.1f;
        private float OneSizeInHeight = 2f;

        [SerializeField] private Camera _cameraComponent;
        [SerializeField] private bool NewMovement;
        [SerializeField] private bool MovementViaAnimation;
        [SerializeField] private string animationName = "";
        [SerializeField] private List<Layer> WorkOnlyFollowingLayerOpened;

        private Animator _animator;
        private Vector3 Difference;
        private Vector3 Origin;
        private bool _drag;
        private float step = 0;
        private float size = 1f;
        private float stop;
        private float stopDefault = 0.9999f;
        private string DatakeyAnimCam = "AnimationCameraTimelinePosition";

        void Awake()
        {
            if(_cameraComponent == null)
                _cameraComponent = this.GetComponent<Camera>();
            if (MovementViaAnimation)
            {
                _animator = this.GetComponent<Animator>();
                _animator.speed = 0;
            }
        }

        void Start()
        {

			#if UNITY_EDITOR
				SpeedMobileMultiply = new Vector2(1, 1);
			#endif

            UpdateMinMax();

            CenterCamera();

            Events.Instance.OnLayerChange += OnLayerChange;

            if (MovementViaAnimation && DataManagement.Instance.CheckItem(DatakeyAnimCam))
            {
                float oldValue = DataManagement.Instance.Get(DatakeyAnimCam).Float;
                step = oldValue;
                SetAnimationValue(oldValue);
            }

            ResetStop();
            UpdateMovement();
        }

        private bool _isEnabled = false;
        private void OnLayerChange(Layer layer, UIAction action)
        {
            // check for disable
            if (action == UIAction.Show)
            {
                if (!_isEnabled)
                    return;
                bool layerInList = false;
                for (var i = 0; i < WorkOnlyFollowingLayerOpened.Count; i++)
                {
                    var pauseLayer = WorkOnlyFollowingLayerOpened[i];
                    if (layer == pauseLayer)
                    {
                        layerInList = true;
                        break;
                    }
                }
                if(!layerInList)
                    CamConfig.Instance.SetEnabled(false);
            }
            else
            {
                if (_isEnabled)
                    return;
                // check for all other disabled
                var layerList = LayerManager.Instance.GetAllLayersWithAction(UIAction.Show, WorkOnlyFollowingLayerOpened);
                if (layerList.Count != 0)
                    return;
                CamConfig.Instance.SetEnabled(true);
            }
        }

        public void DoEnable()
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

        public void DoDisable()
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

        public void UpdateMinMax()
        {
            float camWidth = _cameraComponent.orthographicSize * OneSizeInWidth;
            MinXtemp = MinX + (camWidth/2);
            MaxXtemp = MaxX - (camWidth/2);
            float camHeight = _cameraComponent.orthographicSize * OneSizeInHeight;
            MinYtemp = MinY + (camHeight/2);
            MaxYtemp = MaxY - (camHeight/2);
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
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            // int variable
            var mousePos = Vector2.zero;

            // get mouse pos
            mousePos.x = Input.GetAxis("Mouse X");
            mousePos.y = Input.GetAxis("Mouse Y");

            // get touch pos if available
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

                    mousePos *= _cameraComponent.orthographicSize/25;

                }
                else
                {
                    return;
                }

            }

            mousePos *= -1;

            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < mouseMinY)
                return;

            // do camera movement
            if (MovementViaAnimation)
            {
                DoMovementViaAnimation(mousePos);
            }
            else if (NewMovement)
            {
                Difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
                MoveAbsolute(Origin - Difference);
            }
            else
            {
                Move(mousePos);
            }
        }

        private void Move(Vector2 newPos)
        {
            // set values
            Vector3 pos = transform.position;
            pos.x += newPos.x*Speed.x/Screen.width*Config.Instance.BaseScreenSize.x*SpeedMobileMultiply.x;
            float z = newPos.y*Speed.y/Screen.height*Config.Instance.BaseScreenSize.y*SpeedMobileMultiply.y;
            if (YisZ)
                pos.z += z;
            else
                pos.y += z;

            MoveAbsolute(pos);
        }

        private void MoveAbsolute(Vector3 pos)
        {
            // clamp
            if (pos.x < MinXtemp)
                pos.x = MinXtemp;
            if (pos.x > MaxXtemp)
                pos.x = MaxXtemp;
            if (YisZ)
            {
                if (pos.z > MaxYtemp)
                    pos.z = MaxYtemp;
                if (pos.z < MinYtemp)
                    pos.z = MinYtemp;
            }
            else
            {
                if (pos.y > MaxYtemp)
                    pos.y = MaxYtemp;
                if (pos.y < MinYtemp)
                    pos.y = MinYtemp;
            }

            // set to UI
            transform.position = pos;
        }

        public void DoMovement(Direction dir, float speed = 1)
        {
            DoMovement(Vector2i.FromDirection(dir), speed);
        }

        public void DoMovement(Vector2i direction, float speed = 1)
        {
            Vector2 newPos = direction.ToVector2();
            newPos *= speed*5;
            //newPos *= Config.Instance.SeedingCameraSpeed;
            newPos *= Time.deltaTime;
            newPos *= Screen.width/Config.Instance.BaseScreenSize.x;
            Move(newPos);
        }

        public void CenterCamera()
        {
            Vector3 pos = transform.position;
            //pos.x = Config.Instance.GroveWidth / 2;
            MoveAbsolute(pos);
        }

        private void DoMovementViaAnimation(Vector3 mousePos)
        {
            var mouseRelative = mousePos.y / Screen.height;
            step += mouseRelative * size;
            //var stopTmp = stop - 0.00001f; // dont use "1" as end -> instead use "0.9999"
            step = Mathf.Clamp(step, 0, stop);
            DataManagement.Instance.Set(DatakeyAnimCam, step);
            SetAnimationValue(step);
        }

        private void SetAnimationValue(float step)
        {
            _animator.Play(animationName, 0, step);
        }

        void OnTriggerEnter(Collider other)
        {
            Print.Log("OnTriggerEnter");
            stop = step - 0.001f;
        }

        public void ResetStop()
        {
            stop = stopDefault;
        }

    }
}