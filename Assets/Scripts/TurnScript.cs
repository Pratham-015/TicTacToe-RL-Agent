using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnScript : MonoBehaviour
{
    public Sprite[] Images;
    SpriteRenderer spriteRenderer;
    public int turn=0;
    public GameManager gameManager;
    private bool unplayed=true;
    public int index,row,col;
    void Start()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();
        spriteRenderer.sprite=null;
        gameManager=GameObject.Find("Game Manager").GetComponent<GameManager>();
        index=gameObject.name[1]-'1';
        row=index/3;
        col=index%3;
    }

    void OnMouseDown()
    {
        if (!unplayed)  return;
        //Debug.Log((row,col));
        turn = gameManager.PlayerTurn();

        // Player : 1 (Red X)
        // Enemy : 0 (Blue O)
        spriteRenderer.sprite=Images[turn];
        gameManager.board[row*3+col]=turn;

        unplayed=false;
        gameManager.WinCheck(row, col, turn);
    }
    void ResetTile()
    {
        spriteRenderer.sprite=null;
        unplayed=true;
    }
}