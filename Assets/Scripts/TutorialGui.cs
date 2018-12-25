using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGui : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.GetInt("SeenTutorial") > 0)
        {
            gameObject.SetActive(false);
        }
    }
}