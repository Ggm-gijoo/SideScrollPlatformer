using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundColorChange : MonoBehaviour
{
    [SerializeField]
    private Renderer groundRenderer;
    float angle = 0f;

    private void Update()
    {
        angle += (float)System.Math.Round(1 / 360f, 5) * Time.deltaTime * 40f;
        if (angle >= 1f)
        {
            angle -= 1f;
        }
        groundRenderer.material.SetColor("_BaseColor", Color.HSVToRGB(angle, 0.7f, 1f));
        groundRenderer.material.SetColor("_Color", Color.HSVToRGB(angle, 0.9f, 1f));
    }
}
