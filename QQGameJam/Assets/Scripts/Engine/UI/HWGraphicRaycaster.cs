using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HWGraphicRaycaster : GraphicRaycaster
{
    public Camera TargetCamera;

    public override Camera eventCamera {
        get {
            if (TargetCamera==null) {
                TargetCamera = base.eventCamera;
            }
            return TargetCamera;
        }
    }

    #region 事件点击部分；

    private Canvas canvas;

    protected override void Awake() {
        canvas = GetComponent<Canvas>();
    }

    [NonSerialized] 
    private List<Graphic> m_RaycastResults = new List<Graphic>();
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList) {
        if (canvas == null)
            return;

        var canvasGraphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
        if (canvasGraphics == null || canvasGraphics.Count == 0)
            return;

        int displayIndex;
        var currentEventCamera = eventCamera; // Propery can call Camera.main, so cache the reference

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || currentEventCamera == null)
            displayIndex = canvas.targetDisplay;
        else
            displayIndex = currentEventCamera.targetDisplay;

        var eventPosition = Display.RelativeMouseAt(eventData.position);
        if (eventPosition != Vector3.zero) {
            int eventDisplayIndex = (int)eventPosition.z;
            if (eventDisplayIndex != displayIndex)
                return;
        }
        else {
            eventPosition = eventData.position;
        }

        // Convert to view space
        Vector2 pos;
        if (currentEventCamera == null) {
            float w = Screen.width;
            float h = Screen.height;
            if (displayIndex > 0 && displayIndex < Display.displays.Length) {
                w = Display.displays[displayIndex].systemWidth;
                h = Display.displays[displayIndex].systemHeight;
            }
            pos = new Vector2(eventPosition.x / w, eventPosition.y / h);
        }
        else
            pos = currentEventCamera.ScreenToViewportPoint(eventPosition);
        if (pos.x < 0f || pos.x > 1f || pos.y < 0f || pos.y > 1f)
            return;

        float hitDistance = float.MaxValue;

        Ray ray = new Ray();

        if (currentEventCamera != null)
            ray = currentEventCamera.ScreenPointToRay(eventPosition);

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && blockingObjects != BlockingObjects.None) {
            float distanceToClipPlane = 100.0f;

            if (currentEventCamera != null) {
                float projectionDirection = ray.direction.z;
                distanceToClipPlane = Mathf.Approximately(0.0f, projectionDirection)
                    ? Mathf.Infinity
                    : Mathf.Abs((currentEventCamera.farClipPlane - currentEventCamera.nearClipPlane) / projectionDirection);
            }
        }
        m_RaycastResults.Clear();
        Raycast(canvas, currentEventCamera, eventPosition, canvasGraphics, m_RaycastResults);
        int totalCount = m_RaycastResults.Count;
        for (var index = 0; index < totalCount; index++) {
            var go = m_RaycastResults[index].gameObject;
            bool appendGraphic = true;

            if (ignoreReversedGraphics) {
                if (currentEventCamera == null) {
                    var dir = go.transform.rotation * Vector3.forward;
                    appendGraphic = Vector3.Dot(Vector3.forward, dir) > 0;
                }
                else {
                    var cameraFoward = currentEventCamera.transform.rotation * Vector3.forward;
                    var dir = go.transform.rotation * Vector3.forward;
                    appendGraphic = Vector3.Dot(cameraFoward, dir) > 0;
                }
            }

            if (appendGraphic) {
                float distance = 0;

                if (currentEventCamera == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    distance = 0;
                else {
                    Transform trans = go.transform;
                    Vector3 transForward = trans.forward;
                    distance = (Vector3.Dot(transForward, trans.position - currentEventCamera.transform.position) / Vector3.Dot(transForward, ray.direction));
                    if (distance < 0)
                        continue;
                }
                if (distance >= hitDistance)
                    continue;
                var castResult = new RaycastResult {
                    gameObject = go,
                    module = this,
                    distance = distance,
                    screenPosition = eventPosition,
                    index = resultAppendList.Count,
                    depth = m_RaycastResults[index].depth,
                    sortingLayer = canvas.sortingLayerID,
                    sortingOrder = canvas.sortingOrder
                };
                resultAppendList.Add(castResult);
            }
        }
    }


    /// <summary>
    /// Perform a raycast into the screen and collect all graphics underneath it.
    /// </summary>
    [NonSerialized] 
    static readonly List<Graphic> s_SortedGraphics = new List<Graphic>();
    private static void Raycast(Canvas canvas, Camera eventCamera, Vector2 pointerPosition, IList<Graphic> foundGraphics, List<Graphic> results) {
        int totalCount = foundGraphics.Count;
        Graphic upGraphic = null;
        int upIndex = -1;
        for (int i = 0; i < totalCount; ++i) {
            Graphic graphic = foundGraphics[i];
            int depth = graphic.depth;
            if (depth == -1 || !graphic.raycastTarget || graphic.canvasRenderer.cull)
                continue;

            if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera))
                continue;

            if (eventCamera != null && eventCamera.WorldToScreenPoint(graphic.rectTransform.position).z > eventCamera.farClipPlane)
                continue;

            if (graphic.Raycast(pointerPosition, eventCamera)) {
                s_SortedGraphics.Add(graphic);
                if (depth > upIndex) {
                    upIndex = depth;
                    upGraphic = graphic;
                }
            }
        }
        if (upGraphic != null)
            results.Add(upGraphic);
    }
 #endregion
}
