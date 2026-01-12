using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameStates gameState;
    static public LevelManager inst;
    public Level[] levels;
    public Level curLevel;
    [HideInInspector] public float startLevelTime, endLevelTime, pauseTime;
    [HideInInspector] public int amountOfCoins;
    private void Awake()
    {
        inst = this;
    }
    private void Start()
    {
        levels[0].UnlockLevel();
        foreach (Level level in levels)
        {
            level.SetTriggerObjectsData();
            level.CheckUnlockedLevel();
            level.SetDefTurnedOn();
        }
    }
    private void Update()
    {
        if (gameState == GameStates.Paused) pauseTime += Time.deltaTime;
    }
    public void LoadLevel(int index)
    {
        gameState = GameStates.GameStart;
        UIManager.inst.OnLoadLevelUI();
        startLevelTime = Time.time;
        foreach (Level level in levels) level.container.SetActive(false);
        foreach (Level level in levels) if (level.index == index)
        {
            curLevel = level;
            level.SetTriggerObjectsData();
            level.ReactivateTriggerObjects();
            level.container.SetActive(true);
            Camera.main.orthographicSize = level.cameraSize;
            Camera.main.transform.position = new Vector3(-0.5f, -0.5f, -10);
            DOTween.Kill(Movement.inst.rb);
            Movement.inst.transform.position = level.playerPosition;
            Movement.inst.sprite.DORotate(Vector3.zero, 0);
            ResetCamera();
        }
        amountOfCoins = 0;
        pauseTime = 0;
        Invoke("GameGoing", 0.25f);
    }
    void GameGoing() => gameState = GameStates.GameGoing;
    void ResetCamera()
    {
        Movement.inst.cameraZoomed = false;
        Camera.main.transform.SetParent(null);
        Camera.main.transform.position = new Vector3(-0.5f, -0.5f, -10);
    }
    public void RetryLevel() => LoadLevel(curLevel.index);
    public void LoadNextLevel() => LoadLevel(curLevel.index + 1);
    public void EndTrigger()
    {
        gameState = GameStates.Menu;
        endLevelTime = Time.time - startLevelTime - pauseTime;
        if (endLevelTime < PlayerPrefs.GetFloat($"Level time {curLevel.index}") || PlayerPrefs.GetFloat($"Level time {curLevel.index}") == 0) PlayerPrefs.SetFloat($"Level time {curLevel.index}", endLevelTime);
        UIManager.inst.nextLevelButton.interactable = curLevel.index < levels.Length;
        UIManager.inst.EndTriggerUI();
        curLevel.levelUIInfo.SetStuff(endLevelTime);
        curLevel.SetLevelAsPassed();
        if (curLevel.index < levels.Length) levels[curLevel.index].UnlockLevel();
        foreach (Level level in levels) level.CheckUnlockedLevel();
    }
}
[System.Serializable]
public class Level
{
    public int index, cameraSize;
    public GameObject container;
    public IsTriggerObjects[] coins;
    public SwitchableObjects[] swObjects;
    public IsTriggerObjects end;
    public Vector3 playerPosition;
    public MovementType movementType;
    public LevelUIInfo levelUIInfo;
    public bool unlocked;
    public void SetTriggerObjectsData()
    {
        foreach (IsTriggerObjects coin in coins) coin.level = this;
        foreach (SwitchableObjects swObj in swObjects)
        {
            foreach (IsTriggerObjects sw in swObj.switches) sw.level = this;
            swObj.SetAllStuff();
        }
        end.level  = this;
    }
    public void ReactivateTriggerObjects()
    {
        foreach (IsTriggerObjects coin in coins) coin.Reactivate();
        foreach (SwitchableObjects swObj in swObjects)
        {
            foreach (IsTriggerObjects sw in swObj.switches) sw.Reactivate();
            swObj.SetAllStuff();
            swObj.RecheckTriggerObjects();
        }
        end.Reactivate();
    }
    public void CheckUnlockedLevel()
    {
        levelUIInfo.index = index;
        levelUIInfo.levelButton.interactable = PlayerPrefs.GetInt($"Level {index}") == 1 ? true : false;
        if (PlayerPrefs.GetInt($"Level passed {index}") == 1)  levelUIInfo.SetStuff(PlayerPrefs.GetFloat($"Level time {index}"));
        else levelUIInfo.LockLevel();
    }
    public void UnlockLevel()
    {
        PlayerPrefs.SetInt($"Level {index}", 1);
        levelUIInfo.SetStuff(PlayerPrefs.GetFloat($"Level time {index}"));
    }
    public void SetLevelAsPassed()
    {
        PlayerPrefs.SetInt($"Level passed {index}", 1);
        levelUIInfo.SetStuff(PlayerPrefs.GetFloat($"Level time {index}"));
    }
    public void LockLevel()
    {
        PlayerPrefs.SetInt($"Level {index}", 0);
        PlayerPrefs.SetInt($"Level passed {index}", 0);
        PlayerPrefs.SetFloat($"Level time {index}", 0);
        levelUIInfo.LockLevel();
    }
    public void SetDefTurnedOn()
    {
        foreach (SwitchableObjects swObj in swObjects) swObj.SetDefTurnedOn();
    }
}
[System.Serializable]
public class SwitchableObjects
{
    public Color color, obstacleDisabled;
    public IsTriggerObjects[] switches;
    public Obstacle[] obstacles;
    public void SetAllStuff()
    {
        foreach (IsTriggerObjects sw in switches) sw.spriteRenderer.color = color;
        foreach (Obstacle obs in obstacles)
        {
            obs.sprite.color = obs.isTurnedOn ? color : obstacleDisabled;
            obs.box.enabled = obs.isTurnedOn ? true : false;
        }
    }
    public void SwitchActiveObstacles()
    {
        foreach (Obstacle obs in obstacles) obs.isTurnedOn = !obs.isTurnedOn;
        RecheckActiveObjects();
    }
    public void SetDefTurnedOn()
    {
        foreach (Obstacle obs in obstacles) obs.defTurnedOn = obs.isTurnedOn;
    }
    public void RecheckActiveObjects()
    {
        foreach (Obstacle obs in obstacles)
        {
            obs.sprite.color = obs.isTurnedOn ? color : obstacleDisabled;
            obs.box.enabled = obs.isTurnedOn ? true : false;
        }
    }
    public void RecheckTriggerObjects()
    {
        foreach (Obstacle obs in obstacles) obs.isTurnedOn = obs.defTurnedOn;
        RecheckActiveObjects();
    }
}
[System.Serializable]
public class Obstacle
{
    public GameObject obstacle;
    public bool isTurnedOn;
    [HideInInspector] public bool defTurnedOn;
    public BoxCollider2D box { get { return obstacle.GetComponent<BoxCollider2D>(); } }
    public SpriteRenderer sprite { get { return obstacle.GetComponent<SpriteRenderer>(); } }
}
public enum GameStates
{
    Menu,
    Settings,
    LevelSelect,
    GameStart,
    GameGoing,
    Paused
}
public enum MovementType
{
    Default, 
    Ice
}