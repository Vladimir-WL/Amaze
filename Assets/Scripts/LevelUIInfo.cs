using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIInfo : MonoBehaviour
{
    public Button levelButton;
    public TMP_Text timeText;
    public Image  levelImage;
    public Sprite completeSprite, defSprite;
    public int index;
    private void Start()
    {
        SetStuff(PlayerPrefs.GetFloat($"Level time {index}"));
    }
    public void SetStuff(float time)
    {
        if (PlayerPrefs.GetFloat($"Level time {index}") != 0) timeText.text = time.ToString("F2");
        else timeText.text = "??:??";
        if (PlayerPrefs.GetInt($"Level passed {index}") == 1) levelImage.sprite = completeSprite;
        else levelImage.sprite = defSprite;
    }
    public bool LockLevel()
    {
        timeText.text = "??:??";
        levelImage.sprite = defSprite;
        PlayerPrefs.SetFloat($"Level time {index}", 0);
        return false;
    }
}
