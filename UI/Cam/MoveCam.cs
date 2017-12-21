using System.Collections;
using System.Collections.Generic;
using TinyRoar.Framework;
using UnityEngine;

public class MoveCam : CamMovement
{
    [SerializeField]
    private float DontReactTop = 0.9f;

    [SerializeField]
    private bool YisZ = true;
    [SerializeField]
    private Vector2 Speed = new Vector2(1, 1);
    [SerializeField]
    private Vector2 SpeedMobileMultiply = new Vector2(1, 1);

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
    private Vector3 Difference;

    // orthographic Cam
    [SerializeField]
    private float OneSizeInWidth = 1.1f;
    [SerializeField]
    private float OneSizeInHeight = 2f;

    [SerializeField]
    private bool NewMovement;

    protected override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        SpeedMobileMultiply = new Vector2(1, 1);
#endif
        UpdateMinMax();
        CenterCamera();
    }

    public void UpdateMinMax()
    {
        float camWidth = _cameraComponent.orthographicSize * OneSizeInWidth;
        MinXtemp = MinX + (camWidth / 2);
        MaxXtemp = MaxX - (camWidth / 2);
        float camHeight = _cameraComponent.orthographicSize * OneSizeInHeight;
        MinYtemp = MinY + (camHeight / 2);
        MaxYtemp = MaxY - (camHeight / 2);
    }

    protected override void DoAction(Vector2 mousePos)
    {
        if (!_isEnabled)
            return;

        if (!gameObject.activeInHierarchy)
            return;

        if (_cameraComponent.ScreenToViewportPoint(Input.mousePosition).y >= DontReactTop)
            return;

        if (NewMovement)
        {
            Difference = (_cameraComponent.ScreenToWorldPoint(Input.mousePosition)) - _cameraComponent.transform.position;
            MoveAbsolute(_origin - Difference);
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
        newPos *= Time.deltaTime;
        newPos *= Screen.width / Config.Instance.BaseScreenSize.x;
        Move(newPos);
    }

    public void CenterCamera()
    {
        Vector3 pos = transform.position;
        MoveAbsolute(pos);
    }

}
