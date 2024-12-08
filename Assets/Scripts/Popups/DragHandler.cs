using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ScrollRect scrollRect;

    public UnityEvent OnBeginDragEvent;
    public UnityEvent OnDragEvent;
    public UnityEvent OnEndDragEvent;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    // 드래그 시작 시 호출
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Drag Started");
        // 여기에 드래그 시작시 처리할 코드를 넣을 수 있습니다.

        OnBeginDragEvent?.Invoke();
    }

    // 드래그 중에 계속 호출
    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중일 때 처리할 코드
        Debug.Log("Dragging");

        OnDragEvent?.Invoke();
    }

    // 드래그 끝날 때 호출
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Drag Ended");
        // 여기에 드래그 끝날 때 처리할 코드를 넣을 수 있습니다.

        OnEndDragEvent?.Invoke();
    }
}