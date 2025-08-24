using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMgr : SingletonMonoBehavior<CameraMgr>
{
    public Transform target; // 目标物体
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

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 5f);
    }
}
