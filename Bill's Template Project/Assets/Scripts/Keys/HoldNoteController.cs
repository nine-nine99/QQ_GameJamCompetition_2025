using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldNoteController : MonoBehaviour
{
    public KeyCode key;
    private bool isHolding = false;
    private float holdTime = 0f;
    public float requiredHoldDuration = 1f;
    private bool hasCompleted = false;

    public void StartHolding()
    {
        isHolding = true;
        holdTime = 0f;
        hasCompleted = false;
    }

    void Update()
    {
        if (!isHolding || hasCompleted) return;

        if (Input.GetKey(key))
        {
            holdTime += Time.deltaTime;

            if (holdTime >= requiredHoldDuration)
            {
                Debug.Log("Hold Perfect!");
                ComboManager.Instance.AddCombo();
                hasCompleted = true;
                // Destroy(this.transform.root.gameObject);
            }
        }

        if (Input.GetKeyUp(key))
        {
            if (holdTime < requiredHoldDuration)
            {
                Debug.Log("Hold Missed!");
                ComboManager.Instance.ResetCombo();
                // Destroy(this.transform.root.gameObject);
            }

            isHolding = false;
        }
    }
}
