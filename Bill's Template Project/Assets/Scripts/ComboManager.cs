using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;
    public int combo = 0;
    public TextMeshProUGUI ComboText;
    public TextMeshProUGUI hundredsText;
    public TextMeshProUGUI tensText;
    public TextMeshProUGUI onesText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddCombo()
    {
        combo++;
        UpdateComboDisplay();
        // Debug.Log("Combo: " + combo);
    }

    public void ResetCombo()
    {
        combo = 0;
        UpdateComboDisplay();
        // Debug.Log("Combo: " + combo);
    }

    void UpdateComboDisplay()
    {
        //如果combo在5个以下，就不显示数字
        if (combo < 5)
        {
            hundredsText.text = "";
            tensText.text = "";
            onesText.text = "";
            ComboText.gameObject.SetActive(false);
            return;
        }

        ComboText.gameObject.SetActive(true);
        int hundreds = combo / 100;
        int tens = (combo / 10) % 10;
        int ones = combo % 10;

        hundredsText.text = hundreds > 0 ? hundreds.ToString() : "";
        tensText.text = (hundreds > 0 || tens > 0) ? tens.ToString() : "";
        onesText.text = ones.ToString();
    }
}
