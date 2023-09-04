using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Image image;
    [SerializeField] private Outline outline;

    [Header("Selection Parameters")]
    [SerializeField] private float smoothTime;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color deselectColor;
    private Vector3 sizeVelocity;
    private Coroutine Scale;

    private void Start()
    {
        image = GetComponent<Image>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
        image.color = deselectColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Scale != null) StopCoroutine(Scale);
        Scale = StartCoroutine(SmoothScale(new Vector3(1.1f, 1.1f, 1)));
        outline.enabled = true;
        image.color = selectedColor;
        Debug.Log(2);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(Scale);
        Scale = StartCoroutine(SmoothScale(new Vector3(1f, 1f, 1)));
        outline.enabled = false;
        if (gameObject.GetComponent<UnityEngine.UI.Button>().enabled) image.color = deselectColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (Scale != null) StopCoroutine(Scale);
        transform.localScale = new Vector3(1, 1, 1);
        Scale = StartCoroutine(SmoothScale(new Vector3(1.05f, 1.05f, 1)));
    }

    private void OnDisable()
    {
        image.color = deselectColor;
        outline.enabled = false;
        if (Scale != null) StopCoroutine(Scale);
        transform.localScale = new Vector3(1, 1, 1);
    }

    private IEnumerator SmoothScale(Vector3 newSize) {
        while (transform.localScale != newSize) {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, newSize, ref sizeVelocity, smoothTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
