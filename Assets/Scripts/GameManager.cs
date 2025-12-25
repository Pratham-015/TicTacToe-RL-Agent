using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    int turn;
    int currentTurn=0;
    public int[] board=new int[9];

    [Header("Reward")]
    public float Win=1.0f;
    public float Lose=0f;
    public float Draw=0.75f;

    [Header("Scores")]
    public int Games = 0;
    public int PlayerWins = 0;
    public int EnemyWins = 0;
    public int Draws = 0;
    public int Invalid=0;

    [Header("UI")]
    public TMP_Text GamesText;
    public TMP_Text PlayerScoreText;
    public TMP_Text EnemyScoreText;
    public TMP_Text DrawsText;
    public TMP_Text TurnText;
    public TMP_Text InvalidText;

    [Header("Agents")]
    public AgentScript player;

    void Start()
    {
        Restart();
    }
    void Update()
    {
        // For manual restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    public int TurnRandomizer()
    {
        if (Random.value < 0.5f)
        {
            TurnText.text="Player's Turn";
            return 0;
        }
        TurnText.text="Enemy's Turn";           
        return -1;
    }
    public int PlayerTurn(int r, int c)
    {
        if (currentTurn>=0) {
            // Player's Turn : Starts with turn = 1
            currentTurn++;
            turn =currentTurn%2;
        }
        else {
            // Enemy's Turn : Starts with turn = 0
            currentTurn--;
            turn=(-currentTurn)%2;
        }

        board[r*3+c]=turn;

        if (WinCheck(r, c, turn) == 1) return -1;
        if (DrawCheck()==1) return -1;
        return turn;
    }
    public int WinCheck(int i,int j,int turn)
    {
        // Row check
        if ((board[i*3] == turn) & (board[i*3 + 1] == turn) & (board[i*3 + 2] == turn))
        {
            return AddScore(turn);
        }
        // Column check
        if ((board[ j] == turn) & (board[3 + j] == turn) & (board[6 + j] == turn))
        {
            return AddScore(turn);
        }
        // Diagonals check
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
    public int DrawCheck()
    {
        if (currentTurn == 9 || currentTurn == -10)
        {
            return AddScore(-1);
        }
        return 0;
    }
    public int AddScore(int turn)
    {
        Games++;
        if (turn == 1)
        {
            PlayerWins++;
            player.AddReward(Win);

        }
        else if (turn == 0)
        {
            EnemyWins++;
            player.AddReward(Lose);
        }
        else if (turn==-1)
        {
            Draws++;
            player.AddReward(Draw);
        }
        Restart();
        player.EndEpisode();
        return 1;
    }
    public int Restart()
    {
        for (int i = 0; i < 9; i++)
        {
            board[i]=-1;
        }
        UpdateUI();
        currentTurn=TurnRandomizer();
        return currentTurn;
    }
    void UpdateUI()
    {
        GamesText.text="Games "+Games;
        PlayerScoreText.text="Player "+PlayerWins;
        EnemyScoreText.text="Enemy "+EnemyWins;
        DrawsText.text="Draws "+Draws;
        InvalidText.text="Invalid "+Invalid;
    }
}
