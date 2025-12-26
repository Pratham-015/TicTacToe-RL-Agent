using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public enum GameMode
    {
        SelfPlay,
        HumanVsBot,
        BotVsBot
    }
    public GameMode gameMode = GameMode.HumanVsBot;
    public int humanPlayer=0; // X=1, O=0
    int turn;
    public int currentPlayer;
    public float waitTime=1.0f;
    bool botThinking=false;
    public int[] board=new int[9];

    [Header("Reward")]
    public float WinReward=1.0f;
    public float LosePenalty=-1.00f;
    public float DrawReward=0.2f;

    [Header("Scores")]
    public int Games = 0;
    public int X_Wins = 0;
    public int O_Wins = 0;
    public int Draws = 0;

    [Header("UI")]
    public TMP_Text GamesText;
    public TMP_Text X_ScoreText;
    public TMP_Text O_ScoreText;
    public TMP_Text DrawsText;
    public TMP_Text TurnText;

    [Header("Agents")]
    public AgentScript agentX;
    public AgentScript agentO;

    void Start()
    {
        Restart();
        if (gameMode != GameMode.HumanVsBot)
        {
            humanPlayer=-1;
        }
    }
    void Update()
    {
        if (currentPlayer != humanPlayer && !botThinking)
        {
            StartCoroutine(StartBotMove());
        }
        if (gameMode == GameMode.HumanVsBot)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }
    }

    public void AgentTurn(int a,AgentScript agent)
    {
        // Not this agent's turn
        if (agent.player!=currentPlayer) return;
        // Invalid move
        if (board[a]!=-1)   return;

        board[a]=currentPlayer;
        turn++;

        if (WinCheck(a/3, a%3, currentPlayer)) 
        {
            Games++;
            if (currentPlayer==1) X_Wins++;
            else O_Wins++;
            if (gameMode == GameMode.SelfPlay)
            {
                agent.AddReward(WinReward);
                (agent.opponent).AddReward(LosePenalty);
            }
            EndGame();
            return ;
        }
        if (turn==9) 
        {
            Games++;
            Draws++;
            if (gameMode == GameMode.SelfPlay)
            {
                agentX.AddReward(DrawReward);
                agentO.AddReward(DrawReward);
            }
            EndGame();
            return;
        }
        currentPlayer=1-currentPlayer;
        UpdateUI();
    }
    public bool WinCheck(int i,int j,int turn)
    {
        // Row check
        if ((board[i*3] == turn) & (board[i*3 + 1] == turn) & (board[i*3 + 2] == turn))
        {
            return true;
        }
        // Column check
        if ((board[ j] == turn) & (board[3 + j] == turn) & (board[6 + j] == turn))
        {
            return true;
        }
        // Diagonals check
        if (i == j)
        {
            if ((board[0] == turn) & (board[4] == turn) & (board[8] == turn))
            {
                return true;
            }           
        }
        if (i + j == 2)
        {
            if ((board[2] == turn) & (board[4] == turn) & (board[6] == turn))
            {
                return true;
            }           
        }
        return false;
    }
    public void EndGame()
    {
        Restart();
        agentX.EndEpisode();
        agentO.EndEpisode();
    }
    public void Restart()
    {
        StopAllCoroutines();
        botThinking = true;

        for (int i = 0; i < 9; i++)
        {
            board[i]=-1;
        }
        
        turn=0;
        currentPlayer=(Random.value>0.5f)?1:0;

        UpdateUI();
        botThinking=false;
    }
    void UpdateUI()
    {
        GamesText.text="Games "+Games;
        X_ScoreText.text="X "+X_Wins;
        O_ScoreText.text="O "+O_Wins;
        DrawsText.text="Draws "+Draws;
        TurnText.text=(currentPlayer==1)?"X Turn":"O Turn";
    }
    IEnumerator StartBotMove()
    {
        botThinking=true;

        AgentScript bot = (currentPlayer==1)? agentX:agentO;
        yield return new WaitForSeconds(waitTime);
        bot.RequestDecision();

        botThinking=false;
    }
}
