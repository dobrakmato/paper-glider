using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skins : MonoBehaviour
{
    public Material[] AllSkins;
    public int[] AllSkinPrices;
    public string[] AllSkinNames;

    public Text SkinNameGui;
    public Text DiamondCountGui;
    public Button BuySkinBtn;
    public ParticleSystem PuffParticles;
    public ParticleSystem BuyParticles;
    public AudioClip PuffSound;
    public AudioClip TransactionSound;
    public AudioClip BadSound;
    public MeshRenderer PlayerObject;

    private void Start()
    {
        SetSkinBought(0); // always unlock first skin
        SetSelectedSkin(PlayerPrefs.GetInt("SelectedSkin", 0));
    }

    // -----------------------

    public void NextSkin()
    {
        var next = PlayerPrefs.GetInt("SelectedSkin", 0) + 1;

        if (next >= AllSkins.Length) next = 0;

        SetSelectedSkin(next);
        PuffParticles.Play();
        AudioSource.PlayClipAtPoint(PuffSound, Vector3.zero);
    }

    public void PrevSkin()
    {
        var prev = PlayerPrefs.GetInt("SelectedSkin", 0) - 1;

        if (prev < 0) prev = AllSkins.Length - 1;

        SetSelectedSkin(prev);
        PuffParticles.Play();
        AudioSource.PlayClipAtPoint(PuffSound, Vector3.zero);
    }

    public void UpdateGui()
    {
        SetSelectedSkin(PlayerPrefs.GetInt("SelectedSkin", 0));
    }

    public void UnlockSkin()
    {
        var selected = PlayerPrefs.GetInt("SelectedSkin", 0);

        var money = PlayerPrefs.GetFloat("Diamonds", 0f);

        if (AllSkinPrices[selected] > money)
        {
            return;
        }

        PlayerPrefs.SetFloat("Diamonds", money - AllSkinPrices[selected]);
        DiamondCountGui.text = "" + (int) PlayerPrefs.GetFloat("Diamonds");
        SetSkinBought(selected);
        SetSelectedSkin(selected);
        AudioSource.PlayClipAtPoint(TransactionSound, Vector3.zero);
        BuyParticles.Play();
    }

    public void SetLastBoughtSkin() // called before game starts
    {
        SetSelectedSkin(PlayerPrefs.GetInt("UsedSkin", 0));
    }

    // -------------------

    private static bool IsSkinBought(int skinId)
    {
        return PlayerPrefs.GetInt("Skins.Bought." + skinId, 0) == 1;
    }

    private void SetSkinBought(int skinId)
    {
        PlayerPrefs.SetInt("Skins.Bought." + skinId, 1);
    }

    private void SetSelectedSkin(int skinId)
    {
        var money = PlayerPrefs.GetFloat("Diamonds", 0f);

        PlayerPrefs.SetInt("SelectedSkin", skinId);
        if (IsSkinBought(skinId)) PlayerPrefs.SetInt("UsedSkin", skinId);

        BuySkinBtn.gameObject.SetActive(!IsSkinBought(skinId));
        BuySkinBtn.interactable = money >= AllSkinPrices[skinId];
        BuySkinBtn.GetComponentInChildren<Text>().text = "" + AllSkinPrices[skinId];

        SkinNameGui.text = AllSkinNames[skinId];

        PlayerObject.material = AllSkins[skinId];
        var color = PlayerObject.material.color;
        color.a = IsSkinBought(skinId) ? 1 : 0.5f;
        PlayerObject.material.color = color;
    }
}