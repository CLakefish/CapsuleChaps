using System;
using System.Collections.Generic; 
using UnityEngine.UI; 
using UnityEngine.EventSystems;
using UnityEngine;

public class ColorPickUI : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    [Header("Reference")]
    [SerializeField] private Image image;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private MeshRenderer r;
    [SerializeField] private string spriteToAlter;

    [Header("Color Output")]
    [SerializeField] private Color color;

    public void OnDrag(PointerEventData eventData)
    {
        color = Pick(Camera.main.WorldToScreenPoint(eventData.position), GetComponent<Image>());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        color = Pick(Camera.main.WorldToScreenPoint(eventData.position), GetComponent<Image>());
    }

    private void Start()
    {
        if (PlayerPrefs.GetString(spriteToAlter + "R").Length > 0) 
            color = new Color(
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "R")), 
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "G")), 
            (float)Convert.ToDouble(PlayerPrefs.GetString(spriteToAlter + "B")), 1);
    }

    private void Update()
    {
        if (spriteToAlter == "bodyColor") r.sharedMaterial.color = new Color(color.r, color.g, color.b, 1);

        if (sprite != null) sprite.color = new Color(color.r, color.g, color.b, 1);
        image.color = new Color(color.r, color.g, color.b, 1);

        PlayerPrefs.SetString(spriteToAlter + "R", color.r.ToString());
        PlayerPrefs.SetString(spriteToAlter + "G", color.g.ToString());
        PlayerPrefs.SetString(spriteToAlter + "B", color.b.ToString());
    }

    private Color Pick(Vector2 screenPoint, Image image)
    {
        Texture2D texture = GetComponent<Image>().sprite.texture;
        Vector2 point = new();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, screenPoint, Camera.main, out point);
        point += image.rectTransform.sizeDelta / 2;

        Vector2Int p = new Vector2Int(Mathf.RoundToInt(texture.width * point.x / image.rectTransform.sizeDelta.x), Mathf.RoundToInt(texture.height * point.y / image.rectTransform.sizeDelta.y));

        return texture.GetPixel(p.x, p.y);
    }
}
