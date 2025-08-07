using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverItem : MonoBehaviour
{
    private Vector3 originalScale;
    public float scaleMultiplier = 1.4f;
    // private SpriteRenderer getColor;
    // private Color originalColor;
    // public Color highlightColor = new Color(0.9f, 0.9f, 0.9f, 1f);

    void Start()
    {
        // getColor = GetComponent<SpriteRenderer>();
        // originalColor = getColor.color;
        originalScale = transform.localScale;

        // getColor.sortingOrder = 10;
    }

    void OnMouseEnter()
    {
        // getColor.color = highlightColor;
        transform.localScale = originalScale * scaleMultiplier;
    }

    void OnMouseExit()
    {
        // getColor.color = originalColor;
        transform.localScale = originalScale;
    }
}
