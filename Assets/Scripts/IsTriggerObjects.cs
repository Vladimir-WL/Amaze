using DG.Tweening;
using UnityEngine;

public class IsTriggerObjects : MonoBehaviour
{
    public TriggerType triggerType;
    public Level level;
    public BoxCollider2D box;
    public SpriteRenderer spriteRenderer;
    public AudioClip sound;
    public bool turnOff = true;
    public int index;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (turnOff)
            {
                box.enabled = false;
                transform.DOScale(Vector3.zero, 0.25f);
            }
            if (triggerType == TriggerType.Coin)
            {
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 1);
                Debug.Log(PlayerPrefs.GetInt("Coins"));
                UIManager.inst.LoadGoldTexts();
                LevelManager.inst.amountOfCoins++;
            }
            else if (triggerType == TriggerType.End) LevelManager.inst.EndTrigger();
            else if (triggerType == TriggerType.Switch) LevelManager.inst.curLevel.swObjects[index].SwitchActiveObstacles();
            UIManager.inst.source.clip = sound;
            UIManager.inst.source.Play();
        }
    }
    public void Reactivate()
    {
        transform.localScale = Vector3.one;
        box.enabled = true;
    }
}
public enum TriggerType
{
    Coin,
    End,
    Switch
}
