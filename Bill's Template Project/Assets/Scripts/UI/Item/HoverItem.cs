using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoverItem : MonoBehaviour
{
    private Vector3 originalScale;
    public float scaleMultiplier = 1.4f;

    public GameObject blackOverlay;
    // public Sprite zoomSprite;
    public Image zoomedItem;
    public TextMeshProUGUI descriptionText;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void OnMouseEnter()
    {
        transform.localScale = originalScale * scaleMultiplier;
    }

    void OnMouseExit()
    {
        transform.localScale = originalScale;
    }

    void OnMouseDown()
    {
        blackOverlay.SetActive(true);
        zoomedItem.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);
    }

    public void CloseOverlay()
    {
        blackOverlay.SetActive(false);
        zoomedItem.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
    }
}
