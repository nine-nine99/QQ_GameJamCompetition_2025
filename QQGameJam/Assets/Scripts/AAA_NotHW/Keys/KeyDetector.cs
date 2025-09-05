using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDetector : MonoBehaviour
{
    private KeyCode key;
    public float perfectKeyRange = 0.3f;
    public float goodKeyRange = 0.5f;
    public float badKeyRange = 0.7f;
    // public float missRange = 1.0f;
    public float rayLength = 1.0f;
    public int combo = 0;
    bool hitSomething = false;

    public int index;
    void Update()
    {
        // if (Input.GetKeyDown(key))
        if (SetMenuControl.Instance.IsKeyPressed(index))
        {
            hitSomething = false;
            //向上检测
            RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, rayLength, LayerMask.GetMask("Note"));

            UpDetect(hitUp);

            //向下检测
            if (!hitSomething)
            {
                RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, rayLength, LayerMask.GetMask("Note"));
                DownDetect(hitDown);
            }
        }
    }

    //设置当前按键
    public void SetKey(KeyCode newKey)
    {
        key = newKey;
    }

    // 向上检测
    private void UpDetect(RaycastHit2D hitUp)
    {
        if (hitUp.collider == null) return;

        float distance = Mathf.Abs(hitUp.collider.transform.position.y - transform.position.y);

        if (distance <= perfectKeyRange)
        {
            Debug.Log("Perfect!");
            ComboManager.Instance.AddCombo();
        }
        else if (distance <= goodKeyRange)
        {
            Debug.Log("Good!");
            ComboManager.Instance.AddCombo();
        }
        else if (distance <= badKeyRange)
        {
            ComboManager.Instance.ResetCombo();
        }
        else
        {
            ComboManager.Instance.ResetCombo();
        }
        ObjectPool.Instance.Recycle(hitUp.collider.gameObject);
        hitSomething = true;

    }
    // 向下检测
    private void DownDetect(RaycastHit2D hitDown)
    {
        if (hitDown.collider == null) return;

        float distance = Mathf.Abs(hitDown.collider.transform.position.y - transform.position.y);

        if (hitDown.collider.transform.position.y < transform.position.y)
        {
            if (distance <= badKeyRange)
            {
                ComboManager.Instance.ResetCombo();
            }
            else
            {
                ComboManager.Instance.ResetCombo();
            }
            ObjectPool.Instance.Recycle(hitDown.collider.gameObject);
            hitSomething = true;

        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * rayLength);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayLength);
    }
}
