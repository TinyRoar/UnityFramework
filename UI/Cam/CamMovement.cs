using UnityEngine;
using System.Collections;
using TinyRoar.Framework;

// Move Camera Script
[RequireComponent(typeof(CamConfig))]
public class CamMovement : MonoBehaviour, IBaseCam
{

    [SerializeField]
    private float MinX;
    [SerializeField]
    private float MaxX;
    [SerializeField]
    private float MinY;
    [SerializeField]
    private float MaxY;

    private float MinXtemp;
    private float MaxXtemp;
    private float MinYtemp;
    private float MaxYtemp;

    [SerializeField]
    private bool YisZ = true;

    [SerializeField]
    private Vector2 Speed = new Vector2(1, 1);

    [SerializeField]
    private Vector2 SpeedMobileMultiply = new Vector2(1, 1);

    [SerializeField]
    private int mouseMinY = -999;

    // orthographic Cam
    private float OneSizeInWidth = 3.333333f;
    private float OneSizeInHeight = 2.1f;

    //private Vector2 _positionOld;

    private Camera _cameraComponent;

    void Start()
    {
        _cameraComponent = this.GetComponent<Camera>();

        #if UNITY_EDITOR
            SpeedMobileMultiply = new Vector2(1, 1);
        #endif

        UpdateMinMax();

        CenterCamera();

        UpdateMovement();
    }

    public void DoEnable()
    {
        Inputs.Instance.OnLeftMouseMoveLate -= OnLeftMouseMove;
        Inputs.Instance.OnLeftMouseMoveLate += OnLeftMouseMove;
    }

    public void DoDisable()
    {
        Inputs.Instance.OnLeftMouseMoveLate -= OnLeftMouseMove;
    }

    void OnDestroy()
    {
        //DoDisable();
    }

    public void UpdateMinMax()
    {
        //int groveWidth = Config.Instance.GroveWidth;
        float camWidth = _cameraComponent.orthographicSize * OneSizeInWidth; // orthographicSize * 3.3 = camWidth
        MinXtemp = MinX + (camWidth / 2);
        MaxXtemp = MaxX - (camWidth / 2);
        float camHeight = _cameraComponent.orthographicSize * OneSizeInHeight; // orthographicSize * 2.2 = camWidth
        MinYtemp = MinY + (camHeight / 2);
        MaxYtemp = MaxY - (camHeight / 2);
    }

    public void UpdateMovement()
    {
        OnLeftMouseMove();
    }

    private void OnLeftMouseMove()
    {
        // int variable
        var mousePos = Vector2.zero;

        // get mouse pos
        mousePos.x = Input.GetAxis("Mouse X");
        mousePos.y = Input.GetAxis("Mouse Y");

        // get touch pos if available
        if (Input.touchCount > 0)
        {
            //Debug.Log(Input.GetTouch(0).phase);

            //if(Input.GetTouch(0).phase == TouchPhase.Began)
            //{
            //_positionOld = Input.touches[0].position;
            //}

            if (
                Input.GetTouch(0).phase == TouchPhase.Moved
                ||
                (Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Moved)
            )
            {

                //Debug.Log(Input.touches[0].deltaPosition);
                mousePos.x = Input.touches[0].deltaPosition.x;
                mousePos.y = Input.touches[0].deltaPosition.y;

                mousePos *= _cameraComponent.orthographicSize / 25;

                /*Debug.Log(Input.touches[0].position);

                Vector2 diff = Input.touches[0].position - positionOld;
                diff.x /= Screen.width;
                diff.y /= Screen.height;
                mousePos = diff;
                Debug.Log(mousePos);
                Debug.Log("---");*/

            }
            else
            {
                return;
            }

        }

        //Debug.Log(mousePos);
        mousePos *= -1;

        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < mouseMinY)
            return;
        

        // do camera movement
        move(mousePos);
    }

    private void move(Vector2 newPos)
    { 
        // set values
        Vector3 pos = transform.position;
        pos.x += newPos.x * Speed.x / Screen.width * Config.Instance.BaseScreenSize.x * SpeedMobileMultiply.x;
        float z = newPos.y * Speed.y / Screen.height * Config.Instance.BaseScreenSize.y * SpeedMobileMultiply.y;
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
        newPos *= speed * 5;
        //newPos *= Config.Instance.SeedingCameraSpeed;
        newPos *= Time.deltaTime;
        newPos *= Screen.width / Config.Instance.BaseScreenSize.x;
        move(newPos);
    }

    public void CenterCamera()
    {
        Vector3 pos = transform.position;
        //pos.x = Config.Instance.GroveWidth / 2;
        MoveAbsolute(pos);
    }

}