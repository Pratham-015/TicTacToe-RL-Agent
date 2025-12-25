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

        ResetTile();

        index=gameObject.name[1]-'1';
        row=index/3;
        col=index%3;
    }
    void Update()
    {
        if (gameManager.board[row * 3 + col] == -1)
        {
            ResetTile();
        }
    }
    void OnMouseDown()
    {
        // For manual playing
        if (!unplayed)  return;

        turn = gameManager.PlayerTurn(row,col);
        if (turn==-1) return;

        // Player : 1 (Red X)
        // Enemy : 0 (Blue O)
        spriteRenderer.sprite=Images[turn];

        unplayed=false;
    }
    void ResetTile()
    {
        spriteRenderer.sprite=null;
        unplayed=true;
    }
}