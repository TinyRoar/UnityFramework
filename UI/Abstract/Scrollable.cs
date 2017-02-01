using UnityEngine;
using UnityEngine.EventSystems;

namespace TinyRoar.Framework
{
    /*
     * this is an less-overheaded abstract class for building a button inside a scrollable
     * HOW TO USE: create a c# script that inherits Scrollable and use ButtonAction() method
     * BETTER way: in your c# script, inherit from BaseButton, this is same same, but different (with sound etc..)
     */
    public abstract class Scrollable : MonoBehaviour, IPointerDownHandler
    {

        // vars
        private Vector3 positionDown;
        private readonly float acceptedDistance = Screen.width * 0.10f;

        // Click Down
        public void OnPointerDown(PointerEventData data)
        {
            // save MouseDownPos
            positionDown = Input.mousePosition;

            // SignIn Click Up Event
            Inputs.Instance.OnLeftMouseUp -= OnClickUp;
            Inputs.Instance.OnLeftMouseUp += OnClickUp;
        }

        void OnDestroy()
        {
            // Sign Out Click Up Event
            Inputs.Instance.OnLeftMouseUp -= OnClickUp;
        }

        // If MouseLeftUp or TouchUp
        void OnClickUp()
        {
            // if mouse not moved too much -> click is accepted
            if (Vector3.Distance(positionDown, Input.mousePosition) < this.acceptedDistance)
            {
                this.ButtonAction();
            }

            // Sign Out Click Up Event
            Inputs.Instance.OnLeftMouseUp -= OnClickUp;
        }

        // Button Action
        protected abstract void ButtonAction();

    }
}
