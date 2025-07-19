using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDetector : MonoBehaviour
{
    public KeyCode key;
    public float perfectKeyRange = 0.1f;
    public float goodKeyRange = 0.7f;
    public float badKeyRange = 1.0f;
    public float missRange = 2f;

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1.0f, LayerMask.GetMask("Note"));

            if (hit.collider != null)
            {
                float distance = Mathf.Abs(hit.collider.transform.position.y - transform.position.y);

                if (distance <= perfectKeyRange)
                    Debug.Log("Perfect!");
                else if (distance <= goodKeyRange)
                    Debug.Log("Good!");
                else if (distance <= badKeyRange)
                    Debug.Log("Bad!");
                else
                    Debug.Log("Miss!");

                Destroy(hit.collider.gameObject);
            }
            else
            {
                Debug.Log("Miss! No note hit.");
            }
        }
    }

}
