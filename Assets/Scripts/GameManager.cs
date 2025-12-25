using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    int turn;
    int currentTurn=0;
    public int[] board=new int[9];

    [Header("Scores")]
    public int Games = 0;
    public int PlayerWins = 0;
    public int EnemyWins = 0;
    public int Draws = 0;

    [Header("UI")]
    public TMP_Text GamesText;
    public TMP_Text PlayerScoreText;
    public TMP_Text EnemyScoreText;
    public TMP_Text DrawsText;
    public TMP_Text TurnText;
    void Start()
    {
        Restart();
    }
    public int PlayerTurn(int r, int c)
    {
        if (currentTurn>=0) {
            currentTurn++;
            turn =currentTurn%2;
        }
        else {
            currentTurn--;
            turn=(-currentTurn)%2;
        }
        board[r*3+c]=turn;
        if (WinCheck(r, c, turn) == 1) return -1;

        if (currentTurn == 9 || currentTurn == -10)
        {
            AddScore(-1);
            return -1;
        }
        return turn;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }
    public int WinCheck(int i,int j,int turn)
    {
        if ((board[i*3] == turn) & (board[i*3 + 1] == turn) & (board[i*3 + 2] == turn))
        {
            return AddScore(turn);
        }
        if ((board[ j] == turn) & (board[3 + j] == turn) & (board[6 + j] == turn))
        {
            return AddScore(turn);
        }
        if (i == j)
        {
            if ((board[0] == turn) & (board[4] == turn) & (board[8] == turn))
            {
                return AddScore(turn);
            }           
        }
        if (i + j == 2)
        {
            if ((board[2] == turn) & (board[4] == turn) & (board[6] == turn))
            {
                return AddScore(turn);
            }           
        }
        return 0;
    }
    public int AddScore(int turn)
    {
        Games++;
        if (turn == 1)
        {
            PlayerWins++;
            Restart();
        }
        else if (turn == 0)
        {
            EnemyWins++;
            Restart();
        }
        else if (turn==-1)
        {
            Draws++;
            Restart();
        }
        return 1;
    }
    public void Restart()
    {
        for (int i = 0; i < 9; i++)
        {
            board[i]=-1;
        }
        UpdateUI();
        
        if (Random.value < 0.5f)
        {
            currentTurn=0;
            TurnText.text="Player's Turn";
        }
        else
        {
            currentTurn=-1;
            TurnText.text="Enemy's Turn";           
        }
    }
    void UpdateUI()
    {
        GamesText.text="Games "+Games;
        PlayerScoreText.text="Player "+PlayerWins;
        EnemyScoreText.text="Enemy "+EnemyWins;
        DrawsText.text="Draws "+Draws;
    }
}
