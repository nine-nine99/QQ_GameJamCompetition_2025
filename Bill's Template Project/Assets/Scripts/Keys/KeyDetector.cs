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

            // 向上检测
            RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, rayLength, LayerMask.GetMask("Note"));
            if (hitUp.collider != null)
            {
                GameObject hitObj = hitUp.collider.gameObject;

                if (hitObj.CompareTag("LongNoteHead"))
                {
                    float distance = Mathf.Abs(hitObj.transform.position.y - transform.position.y);

                    if (distance <= perfectKeyRange)
                    {
                        Debug.Log("Perfect Hold Start!");
                        ComboManager.Instance.AddCombo();
                    }
                    else if (distance <= goodKeyRange)
                    {
                        Debug.Log("Good Hold Start!");
                        ComboManager.Instance.AddCombo();
                    }
                    else if (distance <= badKeyRange)
                    {
                        Debug.Log("Bad Hold Start!");
                        ComboManager.Instance.ResetCombo();
                    }
                    else
                    {
                        Debug.Log("Miss Hold Start!");
                        ComboManager.Instance.ResetCombo();
                        return;
                    }

                    HoldNoteController hold = hitObj.GetComponentInParent<HoldNoteController>();
                    if (hold != null) hold.StartHolding();
                    // Destroy(hitObj.transform.parent.gameObject);
                    hitSomething = true;
                }
                else
                {
                    float distance = Mathf.Abs(hitObj.transform.position.y - transform.position.y);

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
                        Debug.Log("Bad!");
                        ComboManager.Instance.ResetCombo();
                    }
                    else
                    {
                        Debug.Log("Miss!");
                        ComboManager.Instance.ResetCombo();
                    }

                    Destroy(hitObj);
                    hitSomething = true;
                }
            }

            // 向下检测 Miss
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
                            Debug.Log("Late Bad!");
                            ComboManager.Instance.ResetCombo();
                        }
                        else
                        {
                            Debug.Log("Miss!");
                            ComboManager.Instance.ResetCombo();
                        }
                        Destroy(hitDown.collider.gameObject);
                    }
                }
            }
        }

        // 松开检测长按尾部
        if (Input.GetKeyUp(key))
        {
            RaycastHit2D hitUpRelease = Physics2D.Raycast(transform.position, Vector2.up, rayLength, LayerMask.GetMask("Note"));

            if (hitUpRelease.collider != null && hitUpRelease.collider.CompareTag("LongNoteTail"))
            {
                GameObject hitObj = hitUpRelease.collider.gameObject;
                float distance = Mathf.Abs(hitObj.transform.position.y - transform.position.y);

                if (distance <= perfectKeyRange)
                {
                    Debug.Log("Perfect Hold End!");
                    ComboManager.Instance.AddCombo();
                }
                else if (distance <= goodKeyRange)
                {
                    Debug.Log("Good Hold End!");
                    ComboManager.Instance.AddCombo();
                }
                else if (distance <= badKeyRange)
                {
                    Debug.Log("Bad Hold End!");
                    ComboManager.Instance.ResetCombo();
                }
                else
                {
                    Debug.Log("Miss Hold End!");
                    ComboManager.Instance.ResetCombo();
                }

                Destroy(hitObj.transform.parent.gameObject);
            }
        }
    }
}
