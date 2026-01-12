using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    static public Movement inst;
    public LayerMask tileLayer;
    public Rigidbody2D rb;
    public Transform sprite;
    public bool cameraZoomed;
    public KeyCode zoomKey, upKey, downKey, leftKey, rightKey;
    bool isMoving, zoomDelay;
    UnityEngine.EventSystems.EventSystem eventSystem;
    int horizontal;
    int vertical;
    Vector3 endPosition;
    Vector3Int movement;
    private void Awake()
    {
        inst = this;
    }
    private void Start()
    {
        eventSystem = UnityEngine.EventSystems.EventSystem.current;
    }
    void Update()
    {
        int horizontal = Input.GetKey(leftKey) ? -1 : (Input.GetKey(rightKey) ? 1 : 0);
        int vertical = Input.GetKey(downKey) ? -1 : (Input.GetKey(upKey) ? 1 : 0);
        movement = new Vector3Int(horizontal, vertical, 0);
        endPosition = Vector2.zero;
        if (((horizontal != 0 && vertical == 0) || (horizontal == 0 && vertical != 0))
        && Input.anyKey && !isMoving && LevelManager.inst.gameState == GameStates.GameGoing)
        {
            if (LevelManager.inst.curLevel.movementType == MovementType.Ice) ChangePositionIce();
            else ChangePosition();
        }
        if (Input.GetKeyDown(zoomKey)) ZoomCamera();
        else if (Input.GetKeyDown(KeyCode.Escape)) UIManager.inst.PauseGame();
        if (eventSystem.currentSelectedGameObject && eventSystem.currentSelectedGameObject.GetComponent<Button>()) 
            eventSystem.SetSelectedGameObject(null);
    }
    public void ChangeHorizontalValue(int value)
    {
        horizontal = value;
        movement = new Vector3Int(horizontal, vertical, 0);
        ChangePosition();
    }
    public void ChangeVerticalValue(int value)
    {
        vertical = value;
        movement = new Vector3Int(horizontal, vertical, 0);
        ChangePosition();
    }
    void ChangePosition()
    {
        isMoving = true;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, (Vector2Int)movement, 1, tileLayer);
        if (!hit.collider)
        {
            endPosition = transform.position + movement;
            rb.DOMove(transform.position + movement, 0.15f);
            Invoke("ResetIsMoving", 0.16f);
            if (Input.GetKey(upKey)) sprite.DORotate(new Vector3(0, 0, 180), 0.17f, RotateMode.Fast);
            else if (Input.GetKey(downKey)) sprite.DORotate(new Vector3(0, 0, 0), 0.17f, RotateMode.Fast);
            else if (Input.GetKey(rightKey)) sprite.DORotate(new Vector3(0, 0, 90), 0.17f, RotateMode.Fast);
            else if (Input.GetKey(leftKey)) sprite.DORotate(new Vector3(0, 0, 270), 0.17f, RotateMode.Fast);
        }
        else ResetIsMoving();
    }
    void ChangePositionIce()
    {
        isMoving = true;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, (Vector2Int)movement, 1, tileLayer);
        if (!hit.collider)
        {
            hit = Physics2D.Raycast(transform.position, (Vector2Int)movement, Mathf.Infinity, tileLayer);
            int amount = Mathf.RoundToInt(Mathf.Abs(hit.distance) -0.5f);
            Debug.Log(amount);
            endPosition = transform.position + movement * amount;
            rb.DOMove(endPosition, 0.1f * amount);
            Invoke("ResetIsMoving", 0.1f * amount + 0.1f);
            if (Input.GetKey(upKey)) sprite.DORotate(new Vector3(0, 0, 180), 0.17f, RotateMode.Fast);
            else if (Input.GetKey(downKey)) sprite.DORotate(new Vector3(0, 0, 0), 0.17f, RotateMode.Fast);
            else if (Input.GetKey(rightKey)) sprite.DORotate(new Vector3(0, 0, 90), 0.17f, RotateMode.Fast);
            else if (Input.GetKey(leftKey)) sprite.DORotate(new Vector3(0, 0, 270), 0.17f, RotateMode.Fast);
        }
        else ResetIsMoving();
    }
    public void ZoomCamera()
    {
        if (!zoomDelay)
        {
            Camera cam = Camera.main;
            if (cameraZoomed)
            {
                cam.transform.SetParent(null);
                DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, LevelManager.inst.curLevel.cameraSize, 1);
                cam.transform.DOMove(new Vector3(-0.5f, -0.5f, -10), 1f);
                cameraZoomed = false;
            }
            else
            {
                cam.transform.SetParent(transform);
                DOTween.To(() => cam.orthographicSize, x => cam.orthographicSize = x, 8, 1);
                cam.transform.DOLocalMove(new Vector3(0, 0, -10), 1f);
                cameraZoomed = true;
            }
            zoomDelay = true;
            UIManager.inst.zoomButton.interactable = false;
            Invoke("ZoomDelay", 1);
        }
    }
    void ZoomDelay()
    {
        zoomDelay = false;
        UIManager.inst.zoomButton.interactable = true;
    }
    private void ResetIsMoving() => isMoving = false;
}
