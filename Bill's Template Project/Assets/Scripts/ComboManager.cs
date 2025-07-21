using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;
    public int combo = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddCombo()
    {
        combo++;
        Debug.Log("Combo: " + combo);
    }

    public void ResetCombo()
    {
        combo = 0;
        // Debug.Log("Combo: " + combo);
    }

}
