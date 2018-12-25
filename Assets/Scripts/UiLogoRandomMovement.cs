using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiLogoRandomMovement : MonoBehaviour
{
    void Update()
    {
        var a = Mathf.Sin(Time.time * 0.05f) * 0.3f;
        var b = Mathf.Sin(13f + Time.time * 0.05f) * 0.3f;
        transform.rotation =
            Quaternion.Euler(Mathf.Sin(4 + Time.time * (0.5f + a)) * 9f, 0,
                Mathf.Cos(11 + Time.time * (0.5f + b)) * 4f);
    }
}