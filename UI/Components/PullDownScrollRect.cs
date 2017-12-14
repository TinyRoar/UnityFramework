using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PullDownScrollRect : MonoBehaviour, IEndDragHandler
{

    public UnityEvent OnPullDown;
    public float TriggerValue = -200;

    private RectTransform contentRectTransform;

    public void Start()
    {
        contentRectTransform = GetComponent<ScrollRect>().content.GetComponent<RectTransform>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (TriggerValue < contentRectTransform.anchoredPosition.y)
            return;

        OnPullDown.Invoke();
        print("Fire update event");
    }
}
