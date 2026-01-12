using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public float delay;
    int i;
    private void Start()
    {
        Invoke("ChangeSprite", delay);
    }
    public void ChangeSprite()
    {
        i++;
        if (i == sprites.Length) i = 0;
        spriteRenderer.sprite = sprites[i];
        Invoke("ChangeSprite", delay);
    }
}
