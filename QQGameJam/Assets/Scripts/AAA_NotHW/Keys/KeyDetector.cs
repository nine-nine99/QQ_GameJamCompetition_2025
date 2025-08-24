using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDetector : MonoBehaviour
{
    public KeyCode key;
    public float perfectKeyRange = 0.3f;
    public float goodKeyRange = 0.5f;
    public float badKeyRange = 0.7f;
    // public float missRange = 1.0f;
    public float rayLength = 1.0f;
    public int combo = 0;

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            bool hitSomething = false;
            //向上检测
            RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, rayLength, LayerMask.GetMask("Note"));
            if (hitUp.collider != null)
            {
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
                    // Debug.Log("Bad!");
                    ComboManager.Instance.ResetCombo();
                }
                else
                {
                    // Debug.Log("Miss!");
                    ComboManager.Instance.ResetCombo();
                }   
                ObjectPool.Instance.Recycle(hitUp.collider.gameObject);
                hitSomething = true;
            }

            //向下检测
            if (!hitSomething)
            {
                RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, rayLength, LayerMask.GetMask("Note"));
                if (hitDown.collider != null)
                {
                    float distance = Mathf.Abs(hitDown.collider.transform.position.y - transform.position.y);

                    if (hitDown.collider.transform.position.y < transform.position.y)
                    {
                        if (distance <= badKeyRange)
                        {
                            // Debug.Log("Late Bad!");
                            ComboManager.Instance.ResetCombo();
                        }
                        else
                        {
                            // Debug.Log("Miss!");
                            ComboManager.Instance.ResetCombo();
                        }
                        ObjectPool.Instance.Recycle(hitDown.collider.gameObject);
                    }
                }
            }
        }
    }
}
