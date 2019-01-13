using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCoins : MonoBehaviour
{
    public Text MainText;
    public Text PriceText;
    public Text CoinCount;
    public AudioClip TransactionSound;
    public AudioClip BadSound;

    private void Start()
    {
        UpdateTexts();
    }

    public void DoUpgrade()
    {
        var multiplier = PlayerPrefs.GetFloat("CoinMultiplier", 1f);
        var price = Mathf.Round(multiplier / 0.0007f / 500) * 500;

        var coins = PlayerPrefs.GetFloat("Coins", 0f);

        if (coins > price)
        {
            PlayerPrefs.SetFloat("Coins", coins - price);
            PlayerPrefs.SetFloat("CoinMultiplier", multiplier * 1.07f);
            AudioSource.PlayClipAtPoint(TransactionSound, Vector3.zero);
        }
        else
        {
            AudioSource.PlayClipAtPoint(BadSound, Vector3.zero);
        }

        UpdateTexts();
    }

    private void UpdateTexts()
    {
        var multiplier = PlayerPrefs.GetFloat("CoinMultiplier", 1f);
        var price = multiplier / 0.0007f;

        MainText.text = Math.Round(multiplier * 1.07f, 2) + "x coins";
        PriceText.text = "for " + Math.Round(Mathf.Round(price / 500) * 500) + " coins";
        CoinCount.text = "" + (int) PlayerPrefs.GetFloat("Coins", 0f);
    }
}