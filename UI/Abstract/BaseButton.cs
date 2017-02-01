using TinyRoar.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class BaseButton : MonoBehaviour, IPointerDownHandler
{
    // Config
    [SerializeField]
    private bool UseScrollRect = false;
    [SerializeField]
    protected bool MultipleClickable = false;
    [SerializeField]
    protected string Sound = "ButtonStandard";

    // internal
    protected Button UIButton;
    private Vector3 posDown;
    private readonly float acceptedScrollDistance = Screen.width * 0.1f;
    private float reClickableAfterSec = 0.5f;

    // used for locked level
    public bool Disallowed
    {
        get;
        set;
    }

    void Awake()
    {
        // Get UI Button Component
        this.UIButton = this.GetComponent<UnityEngine.UI.Button>();

        // Add OnClick Event
        if (!UseScrollRect)
        {
            UIButton.onClick.AddListener(ButtonListener);
        }

    }

    void OnDestroy()
    {
        Inputs.Instance.OnLeftMouseUp -= OnClickUp;
    }

    // Button -> Start / Retry Game
    void ButtonListener()
    {
        if (Disallowed == true)
            return;

        // Play Button Sound if available
        if (this.Sound.Trim() != "")
            SoundManager.Instance.Play(this.Sound, SoundManager.SoundType.Soundeffect);


        if (MultipleClickable == false)
        {
            this.GetComponent<Button>().interactable = false;
            Timer.Instance.Add(reClickableAfterSec, reclickableCallback);
        }

        // AAAAAAND... ACTION!
        this.ButtonAction();

    }

    protected abstract void ButtonAction();

    /* Button inside of ScrollRect */

    // Click Down
    public void OnPointerDown(PointerEventData data)
    {
        // cancel if not using ScrollRect
        if (!UseScrollRect)
            return;

        if (Disallowed)
            return;

        // save MouseDownPos
        posDown = Input.mousePosition;

        // SignIn Click Up Event
        Inputs.Instance.OnLeftMouseUp -= OnClickUp;
        Inputs.Instance.OnLeftMouseUp += OnClickUp;
    }

    // If MouseLeftUp or TouchUp
    void OnClickUp()
    {
        // if mouse not moved too much -> click is accepted
        if (Vector3.Distance(posDown, Input.mousePosition) < this.acceptedScrollDistance)
        {
            //this.ButtonAction();
            ButtonListener();
        }

        // SignOut Click Up Event
        Inputs.Instance.OnLeftMouseUp -= OnClickUp;
    }

    void reclickableCallback()
    {
        if (this == null)
            return;
        if (this.GetComponent<Button>() == null)
            return;

        this.GetComponent<Button>().interactable = true;
    }

}
