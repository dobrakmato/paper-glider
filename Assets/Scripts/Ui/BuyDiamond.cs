using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyDiamond : MonoBehaviour
{
    public Text DiamondCount;
    public Text CoinCount;
    public int Price;

    public AudioClip TransactionSound;
    public AudioClip BadSound;

    public void Buy()
    {
        var diamonds = PlayerPrefs.GetFloat("Diamonds", 0);
        var coins = PlayerPrefs.GetFloat("Coins", 0);
        if (coins < Price)
        {
            PlayBadSound();
            return;
        }

        PlayerPrefs.SetFloat("Coins", coins - Price);
        PlayerPrefs.SetFloat("Diamonds", diamonds + 1);
        UpdateCoinsAndDiamondsText();
    }

    private void UpdateCoinsAndDiamondsText()
    {
        DiamondCount.text = "" + (int)PlayerPrefs.GetFloat("Diamonds");
        CoinCount.text = "" + (int)PlayerPrefs.GetFloat("Coins");
        DiamondCount.gameObject.GetComponent<Animation>().Play(PlayMode.StopAll);
        PlayTransactionSound();
        FindObjectOfType<Skins>().UpdateGui();
    }

    private void PlayTransactionSound()
    {
        AudioSource.PlayClipAtPoint(TransactionSound, Vector3.zero);
    }

    private void PlayBadSound()
    {
        AudioSource.PlayClipAtPoint(BadSound, Vector3.zero);
    }
}