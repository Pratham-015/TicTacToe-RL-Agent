using Unity.VisualScripting;
using UnityEngine;

public class TurnScript : MonoBehaviour
{
    public Sprite[] Images;
    SpriteRenderer spriteRenderer;
    public int turn=0;
    public GameManager gameManager;
    public int index,row,col;
    void Start()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();
        spriteRenderer.sprite=null;

        index=gameObject.name[1]-'1';
        row=index/3;
        col=index%3;
    }
    void Update()
    {
        if (gameManager.board[row*3+col] == -1)
        {
            spriteRenderer.sprite=null;
        }
        else
        {
            spriteRenderer.sprite=Images[gameManager.board[row*3+col]];
        }
    }
    // For manual playing
    void OnMouseDown()
    {
        if (gameManager.gameMode != GameManager.GameMode.HumanVsBot && gameManager.gameMode != GameManager.GameMode.HumanVsHuman) return;
        if (gameManager.currentPlayer!=gameManager.humanPlayer) return;
        if (gameManager.board[index]!=-1)  return;

        gameManager.AgentTurn(index,
            gameManager.humanPlayer==1 ?
            gameManager.agentX :
            gameManager.agentO);
        if (turn==-1) return;

        // Player : 1 (Red X)
        // Enemy : 0 (Blue O)
    }
}