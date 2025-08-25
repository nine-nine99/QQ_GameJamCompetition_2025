using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMgr : SingletonMonoBehavior<CameraMgr>
{
    public Transform target; // 目标物体

    [Header("摄像机移动限制")]
    public bool useLimit = false;
    public Vector2 minPos;
    public Vector2 maxPos;

    public float smoothSpeed = 5f;

    private void Update()
    {
        if (target == null) return;

        // 调用跟随目标的方法
        FollowTarget(target);
    }
    public void FollowTarget(Transform target)
    {
        if (target == null) return;
        Vector3 newPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        if (target.tag == "Player")
        {
            newPosition.y = 0;
        }

        if (useLimit)
        {
            float clampX = Mathf.Clamp(newPosition.x, minPos.x, maxPos.x);
            float clampY = Mathf.Clamp(newPosition.y, minPos.y, maxPos.y);
            newPosition = new Vector3(clampX, clampY, newPosition.z);
        }

        // 平滑移动
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * smoothSpeed);
    }
}
