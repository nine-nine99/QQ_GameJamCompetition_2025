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
    private BaseNote mBaseNote;
    private LongNote mLongNote; // 当前按住的长音符
    void Update()
    {
        // if (Input.GetKeyDown(key))
        if (SetMenuControl.Instance.IsKeyPressed(index))
        {
            hitSomething = false;
            mLongNote = null;
            //向上检测
            RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, rayLength, LayerMask.GetMask("Note"));
            if (hitUp.collider == null) return;

            // 获取得到的音符
            if (hitUp.collider.GetComponent<LongNote_PartMove>() != null)
            {
                mBaseNote = hitUp.collider.GetComponent<LongNote_PartMove>().mLongNote;
                mLongNote = hitUp.collider.GetComponent<LongNote_PartMove>().mLongNote;
            }
            else if (hitUp.collider.GetComponent<NormalNote>() != null)
            {
                mBaseNote = hitUp.collider.GetComponent<NormalNote>();
            }
            else
            {
                Debug.LogError("判定线上出问题了");
            }

            UpDetect(hitUp, mBaseNote);

            // 若向上检测没有碰到东西，则向下检测
            if (!hitSomething)
            {
                RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, rayLength, LayerMask.GetMask("Note"));
                DownDetect(hitDown, mBaseNote);
            }
        }

        if (mLongNote == null) return;
        // 按住了
        if (SetMenuControl.Instance.IsKeyHold(index))
        {
            mLongNote.OnHold();
        }

        // 没按住
        if (!SetMenuControl.Instance.IsKeyHold(index))
        {
            mLongNote.OnLose(mLongNote.headCurSpeed);
            mLongNote = null;
        }
    }

    //设置当前按键
    public void SetKey(KeyCode newKey)
    {
        key = newKey;
    }

    // 向上检测
    private void UpDetect(RaycastHit2D hitUp, BaseNote baseNote)
    {
        if (hitUp.collider == null) return;

        float distance = Mathf.Abs(hitUp.collider.transform.position.y - transform.position.y);

        if (distance <= perfectKeyRange)
        {
            Debug.Log("Perfect!");
            ComboManager.Instance.AddCombo();
            // 触发击中
            baseNote.OnHit();
        }
        else if (distance <= goodKeyRange)
        {
            Debug.Log("Good!");
            ComboManager.Instance.AddCombo();
            baseNote.OnHit();
        }
        else if (distance <= badKeyRange)
        {
            ComboManager.Instance.ResetCombo();
            baseNote.OnHit();
        }
        else    // Miss
        {
            ComboManager.Instance.ResetCombo();
            baseNote.OnHit();
        }

        // TODO:整合到BaseNote中去
        // ObjectPool.Instance.Recycle(hitUp.collider.gameObject);
        hitSomething = true;

    }
    // 向下检测
    private void DownDetect(RaycastHit2D hitDown, BaseNote baseNote)
    {
        if (hitDown.collider == null) return;

        float distance = Mathf.Abs(hitDown.collider.transform.position.y - transform.position.y);

        if (hitDown.collider.transform.position.y < transform.position.y)
        {
            if (distance <= badKeyRange)
            {
                ComboManager.Instance.ResetCombo();
                baseNote.OnHit();

            }
            else
            {
                ComboManager.Instance.ResetCombo();
                baseNote.OnHit();

            }
            // ObjectPool.Instance.Recycle(hitDown.collider.gameObject);
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
