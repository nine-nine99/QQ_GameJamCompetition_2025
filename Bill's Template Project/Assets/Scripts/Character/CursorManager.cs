using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
