using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyRoar.Framework;

/*
 * Pinch to Zoom
 */
[RequireComponent(typeof(CamConfig))]
public class CamZoom : MonoBehaviour, IBaseCam
{
    [SerializeField]
    private float ZoomSpeed = 0.5f;

    [SerializeField]
    private Vector2 Size = new Vector2(20, 50);
    [SerializeField]
    private float BaseSize = 40f;

    private float orthographicSize;

    private CamMovement CamMovement;
    private Camera cameraComponent
    {
        get;
        set;
    }

    void Start ()
	{
        cameraComponent = this.GetComponent<Camera>();
        UpdateCamZoom(BaseSize);
        CamMovement = this.GetComponent<CamMovement>();
    }

    public void DoEnable()
    {
        Inputs.Instance.OnTouchMove -= TouchMove;
        Inputs.Instance.OnTouchMove += TouchMove;
        Updater.Instance.OnLateUpdate -= DoUpdate;
        Updater.Instance.OnLateUpdate += DoUpdate;
    }

    public void DoDisable()
    {
        Inputs.Instance.OnTouchMove -= TouchMove;
        Updater.Instance.OnLateUpdate -= DoUpdate;
    }

    void OnDestroy()
    {
        //DoDisable();
    }

    void TouchMove()
    {
        if (Input.touchCount != 2)
            return;

        // get two touches
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // get the position
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // get previous values
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;

        // get current values
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // calc difference
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        float newSize = deltaMagnitudeDiff * ZoomSpeed;
        newSize += cameraComponent.orthographicSize;
        newSize = Mathf.Clamp(newSize, Size.x, Size.y);

        // set
        UpdateCamZoom(newSize);

    }

    void DoUpdate()
    {
        // mouse wheel zoom
        float newSize = Mathf.Clamp(orthographicSize - (Input.GetAxis("Mouse ScrollWheel")*2), Size.x, Size.y);
        
        // set
        UpdateCamZoom(newSize);

    }

    private void UpdateCamZoom(float newSize)
    {
        if (orthographicSize == newSize)
            return;

        // set
        orthographicSize = newSize;
        cameraComponent.orthographicSize = orthographicSize;

        if (CamMovement != null)
        {
            CamMovement.UpdateMinMax();
            CamMovement.UpdateMovement();
        }
    }
}
