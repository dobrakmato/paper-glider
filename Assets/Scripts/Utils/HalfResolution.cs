using UnityEngine;

public class HalfResolution : MonoBehaviour
{
    private static readonly int OriginalWidth = Screen.width;
    private static readonly int OriginalHeight = Screen.height;

    void Start()
    {
        if (Application.isMobilePlatform)
        {
            Screen.SetResolution(OriginalWidth / 2, OriginalHeight / 2, true);
        }
    }
}