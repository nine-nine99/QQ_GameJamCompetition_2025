using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonController : MonoBehaviour
{
    public void OnButtonPress()
    {
        EventHandler.CallStartBGMButtonPressedEvent();
    }
}
