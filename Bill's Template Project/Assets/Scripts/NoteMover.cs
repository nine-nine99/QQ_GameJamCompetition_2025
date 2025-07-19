using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMover : MonoBehaviour
{
    public float speed = 2.0f;
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }
}
