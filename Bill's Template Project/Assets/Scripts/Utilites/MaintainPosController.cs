using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainPosController : MonoBehaviour
{
    // 让该物体的坐标时刻保持在父物体的坐标上
    private Transform parentTransform;
    private Vector3 offset;
    private void Start()
    {
        parentTransform = transform.parent;
        offset = transform.localPosition;
    }
    private void Update()
    {
        if (parentTransform != null)
        {
            transform.position = parentTransform.position + offset;
        }
    }
}
