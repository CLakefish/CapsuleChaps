using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopingBackground : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private Vector2 movement;

    // Update is called once per frame
    void Update()
    {
        image.uvRect = new Rect(image.uvRect.position + new Vector2(movement.x, movement.y) * Time.deltaTime, image.uvRect.size);
    }
}
