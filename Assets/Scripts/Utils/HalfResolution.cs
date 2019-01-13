﻿using UnityEngine;

public class HalfResolution : MonoBehaviour
{
    private static readonly int OriginalWidth = Screen.width;
    private static readonly int OriginalHeight = Screen.height;

    void Start()
    {
        if (Application.isMobilePlatform)
        {
            Application.targetFrameRate = 60;
            Screen.SetResolution(720, 1080, false, 60);
        }
    }
}