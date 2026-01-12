using TMPro;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    [SerializeField] private bool debug;
    public GameObject container, inGamePanel;
    public TMP_InputField deletePrefInput, deleteLevelInput, unlockLevelInput, gameSpeedInput;
    private void Update() { if (debug && Input.GetKeyDown(KeyCode.BackQuote)) container.SetActive(!container.activeSelf); }
    public void DeleteAllPrefs()
    {
        PlayerPrefs.DeleteAll();
    }         
    public void DeleteSelectedPref()
    {
        PlayerPrefs.DeleteKey(deletePrefInput.text);
    }
    public void DeleteSelectedLevels()
    {
        for (int i = int.Parse(deleteLevelInput.text) - 1; i < LevelManager.inst.levels.Length; i++)
        {
            LevelManager.inst.levels[i].LockLevel();
            LevelManager.inst.levels[i].CheckUnlockedLevel();
        }
    }
    public void UnlockSelectedLevels()
    {
        for (int i = int.Parse(unlockLevelInput.text) - 1; i >= 0; i--)
        {
            LevelManager.inst.levels[i].UnlockLevel();
            if (i - 1 >= 0) LevelManager.inst.levels[i - 1].SetLevelAsPassed();
            LevelManager.inst.levels[i].CheckUnlockedLevel();
        }
    }
    public void MazeOnlyMode()
    {
        Movement move = Movement.inst;
        Level lvl = LevelManager.inst.curLevel;
        move.sprite.gameObject.SetActive(!move.sprite.gameObject.activeSelf);
        lvl.end.gameObject.SetActive(move.sprite.gameObject.activeSelf);
        inGamePanel.SetActive(!inGamePanel.activeSelf);
        foreach (IsTriggerObjects obj in lvl.coins) obj.gameObject.SetActive(move.sprite.gameObject.activeSelf);
        foreach (SwitchableObjects obj in lvl.swObjects)
        {
            foreach (IsTriggerObjects swb in obj.switches) swb.gameObject.SetActive(move.sprite.gameObject.activeSelf);
            foreach (Obstacle obs in obj.obstacles) obs.obstacle.SetActive(move.sprite.gameObject.activeSelf);
        }
    }
    public void SetGameSpeed() => Time.timeScale = float.Parse(gameSpeedInput.text);
}
