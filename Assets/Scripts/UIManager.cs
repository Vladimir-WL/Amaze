using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static public UIManager inst;
    public GameObject winUIPanel, selectLevelPanel, pausePanel, settingsPanel, mainPanel, backgroundPanel;
    public RectTransform selectLevelContent;
    public TMP_Text[] goldTexts, volumeSettingsText;
    public TMP_Text timeText, coinsText, settingsText;
    public TMP_Text upKeyText, downKeyText, leftKeyText, rightKeyText, zoomKeyText;
    public Button zoomButton, zoomChange, upChange, downChange, leftChange, rightChange, nextLevelButton;
    public AudioSource source;
    public Slider[] volumeSliders;
    public Image mainMenuImage;
    public Sprite[] mainMenuSprites;
    private void Awake()
    {
        inst = this;
    }
    private void Start()
    {
        StartCoroutine(MainMenuSpriteChange());
        OnFirstLoad();
        if (Movement.inst.zoomKey == KeyCode.None) ResetControls();
        LoadKeys();
        SetControlsTexts();
        source.volume = PlayerPrefs.GetFloat("Volume");
        for (int i = 0; i < volumeSliders.Length; i++)
        {
            volumeSliders[i].value = PlayerPrefs.GetFloat("Volume");
            volumeSettingsText[i].text = $"{(source.volume * 100).ToString("F0")}%";
        }
        LoadGoldTexts();
    }
    public void OnFirstLoad()
    {
        if (PlayerPrefs.GetInt("FirstLoad") == 0)
        {
            PlayerPrefs.SetFloat("Volume", 0.5f);
            ResetControls();
            PlayerPrefs.SetInt("FirstLoad", 1);
        }
    }
    public void LoadGoldTexts()
    {
        string coins = PlayerPrefs.GetInt("Coins").ToString();
        foreach (TMP_Text text in goldTexts) text.text = coins;
    }
    public void OnLoadLevelUI()
    {
        ToMenu(0, 0, 0, 0, 0);
        backgroundPanel.SetActive(false);
        OnLevelLoadVolume();
    }
    public void ToLevelSelect()
    {
        ToMenu(0, 1, 0, 0, 0);
        pausePanel.transform.DOScale(new Vector3(0, 1, 1), 0f);
        selectLevelContent.position.Set(selectLevelContent.position.y, 0, selectLevelContent.position.z);
    }
    public void ToSettings()
    {
        if (settingsPanel.transform.localScale.x == 0)
        {
            settingsText.text = "Close";
            ToMenu(0, 0, 1, 1, 0);
        }
        else if (settingsPanel.transform.localScale.x == 1)
        {
            settingsText.text = "Settings";
            ToMenu(0, 0, 0, 1, 0);
        }
    }
    public void ToMain() => ToMenu(0, 0, 0, 1, 0);
    public void ToMenu(int winUI, int select, int settings, int main, int pause)
    {
        winUIPanel.transform.DOScale(new Vector3(winUI, 1, 1), 0.5f);
        selectLevelPanel.transform.DOScale(new Vector3(select, 1, 1), 0.5f);
        settingsPanel.transform.DOScale(new Vector3(settings, 1, 1), 0.5f);
        mainPanel.transform.DOScale(new Vector3(main, 1, 1), 0.5f);
        pausePanel.transform.DOScale(new Vector3(pause, 1, 1), 0.5f);
        backgroundPanel.SetActive(true);
        if (settings != 1) settingsText.text = "Settings";
    }
    public void ChangeVolume(int index)
    {
        source.volume = volumeSliders[index].value;
        volumeSettingsText[index].text = $"{(source.volume * 100).ToString("F0")}%";
        PlayerPrefs.SetFloat("Volume", source.volume);
    }
    public void OnLevelLoadVolume()
    {
        for (int i = 0; i < volumeSliders.Length; i++)
        {
            volumeSliders[i].value = PlayerPrefs.GetFloat("Volume");
            volumeSettingsText[i].text = $"{(source.volume * 100).ToString("F0")}%";
        }
    }
    public void PauseGame()
    {
        LevelManager lvl = LevelManager.inst;
        if (lvl.gameState == GameStates.GameGoing)
        {
            pausePanel.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
            lvl.gameState = GameStates.Paused;

        }
        else if (lvl.gameState == GameStates.Paused)
        {
            pausePanel.transform.DOScale(new Vector3(0, 1, 1), 0.5f);
            lvl.gameState = GameStates.GameGoing;
        }
    }
    public void EndTriggerUI()
    {
        LevelManager lvl = LevelManager.inst;
        winUIPanel.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        timeText.text = $"Time: {lvl.endLevelTime.ToString("F2")}";
        coinsText.text = $"Collected: {lvl.amountOfCoins}/{lvl.curLevel.coins.Length}";
    }
    public void UpdateAllTexts()
    {
        LoadGoldTexts();
    }
    public void ChangeControl(string keyName) => StartCoroutine(ChangeKey(keyName));
    public IEnumerator ChangeKey(string keyName)
    {
        switch (keyName)
        {
            case "up": upKeyText.text = "<color=red>---"; break;
            case "down": downKeyText.text = "<color=red>---"; break;
            case "left": leftKeyText.text = "<color=red>---"; break;
            case "right": rightKeyText.text = "<color=red>---"; break;
            case "zoom": zoomKeyText.text = "<color=red>---"; break;
        }
        yield return new WaitUntil(() => Input.anyKeyDown);
        KeyCode key = KeyCode.None;
        Movement movement = Movement.inst;
        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                key = keyCode;
                break;
            }
        }
        switch (keyName)
        {
            case "up": movement.upKey = key; break;
            case "down": movement.downKey = key; break;
            case "left": movement.leftKey = key; break;
            case "right": movement.rightKey = key; break;
            case "zoom": movement.zoomKey = key; break;
        }
        SaveKeys();
        SetControlsTexts();
    }
    public void SetControlsTexts()
    {
        Movement movement = Movement.inst;
        upKeyText.text = movement.upKey.ToString();
        downKeyText.text = movement.downKey.ToString();
        leftKeyText.text = movement.leftKey.ToString();
        rightKeyText.text = movement.rightKey.ToString();
        zoomKeyText.text = movement.zoomKey.ToString();
    }
    public void ResetControls()
    {
        Movement movement = Movement.inst;
        movement.upKey = KeyCode.W;
        movement.downKey = KeyCode.S;
        movement.leftKey = KeyCode.A;
        movement.rightKey = KeyCode.D;
        movement.zoomKey = KeyCode.E;
        SaveKeys();
        SetControlsTexts();
    }
    public void SaveKeys()
    {
        Movement movement = Movement.inst;
        PlayerPrefs.SetString("Up Key", movement.upKey.ToString());
        PlayerPrefs.SetString("Down Key", movement.downKey.ToString());
        PlayerPrefs.SetString("Left Key", movement.leftKey.ToString());
        PlayerPrefs.SetString("Right Key", movement.rightKey.ToString());
        PlayerPrefs.SetString("Zoom Key", movement.zoomKey.ToString());
        PlayerPrefs.Save();
    }

    public void LoadKeys()
    {
        Movement movement = Movement.inst;
        movement.upKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up Key", "W"));
        movement.downKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down Key", "S"));
        movement.leftKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left Key", "A"));
        movement.rightKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right Key", "D"));
        movement.zoomKey = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Zoom Key", "Z"));
    }
    public void CloseGame() => Application.Quit();
    IEnumerator MainMenuSpriteChange()
    {
        for (int i = 0; i < mainMenuSprites.Length; i++)
        {
            mainMenuImage.sprite = mainMenuSprites[i];
            yield return new WaitForSeconds(1f);
            mainMenuImage.DOColor(Color.white, 2f);
            yield return new WaitForSeconds(6f);
            mainMenuImage.DOColor(Color.black, 1f);
            yield return new WaitForSeconds(2f);
            if (i == mainMenuSprites.Length - 1) i = -1;
        }
    }
}
