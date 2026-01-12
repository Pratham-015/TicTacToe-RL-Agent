using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public enum GameMode
    {
        SelfPlay,
        HumanVsBot,
        BotVsBot,
        QTraining,
        HumanVsHuman
    }
    public enum AI
    {
        MLAgent,
        QTable
    }
    public GameMode gameMode = GameMode.HumanVsBot;
    public AI ai=AI.QTable;
    public int humanPlayer=0; // X=1, O=0
    public int turn;
    public int currentPlayer;
    public float waitTime=1.0f;
    bool botThinking=false;
    public int[] board=new int[9];
    public AudioSource audioSource;
    public Sprite[] Images;
    public SpriteRenderer spriteRenderer;

    [Header("Rewards")]
    public float WinReward=1.0f;
    public float LosePenalty=-1.00f;
    public float DrawReward=0.05f;
    public float CenterReward=0.15f;
    public float CornerReward=0.05f;
    public float MissesBlockedMove=-0.5f;

    [Header("Scores")]
    public int Games = 0;
    public int X_Wins = 0;
    public int O_Wins = 0;
    public int Draws = 0;

    [Header("Q Learning")]
    public QTableScript qTable;

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
        audioSource = GetComponent<AudioSource>();
        Restart();
        if (gameMode != GameMode.HumanVsBot && gameMode!=GameMode.HumanVsHuman)
        {
            humanPlayer=-1;
        }
        if (gameMode == GameMode.QTraining)
        {
            StartCoroutine(StartQTraining());
        }
        if (ai == AI.QTable && gameMode != GameMode.QTraining)
        {
            qTable.LoadQTables();
        }
    }
    void Update()
    {
        if (gameMode==GameMode.SelfPlay) return;
        if (currentPlayer != humanPlayer && !botThinking)
        {
            if (ai==AI.MLAgent)
            {
                StartCoroutine(StartMLAgentBotMove());
            }
            else if (ai == AI.QTable)
            {
                StartCoroutine(StartQTableBotMove());
            }
            
        }
        if (gameMode == GameMode.HumanVsBot || gameMode==GameMode.HumanVsHuman)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }
    }

    public void AgentTurn(int a,AgentScript agent)
    {
        // if (ai!=AI.MLAgent) return;
        // Not this agent's turn
        if (agent.player!=currentPlayer) return;
        // Invalid move
        if (board[a]!=-1)   return;

        board[a]=currentPlayer;
        turn++;
        audioSource.Play();

        if (WinCheck(a/3, a%3, currentPlayer)) 
        {
            Games++;
            if (currentPlayer==1) X_Wins++;
            else O_Wins++;
            if (gameMode == GameMode.SelfPlay)
            {
                agent.AddReward(WinReward);
                agent.opponent.AddReward(LosePenalty);
            }
            EndGame();
            return ;
        }
        if (gameMode==GameMode.SelfPlay && turn==0)
            {
                if (a==4){
                    agent.AddReward(CenterReward);
                    agent.opponent.AddReward(CenterReward);
                }
                else if (a==0||a==2||a==6||a==8){
                    agent.AddReward(WinReward);
                    agent.opponent.AddReward(LosePenalty);
                }
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
        if (gameMode == GameMode.HumanVsHuman) humanPlayer = currentPlayer;
        UpdateUI();
    }
    public int QTurn(int a)
    {
        if (ai!=AI.QTable) return -1;
        // Invalid move
        if (a<0 || a>8) return -1;
        if (board[a]!=-1)   return -1;

        board[a]=currentPlayer;
        turn++;
        audioSource.Play();

        if (WinCheck(a/3, a%3, currentPlayer)) 
        {
            Games++;
            if (currentPlayer==1) X_Wins++;
            else O_Wins++;
            if (gameMode==GameMode.HumanVsBot) Restart();
            return 1;
        }
        if (turn==9) 
        {
            Games++;
            Draws++;
            if (gameMode==GameMode.HumanVsBot) Restart();
            return 2;
        }
        currentPlayer=1-currentPlayer;
        UpdateUI();
        return 0;
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
    public bool BlocksImmediateWin(int action, int player)
    {
        int opp=1-player;
        for (int i=0;i<9;i++)
        {
            if (board[i]==-1)
            {
                board[i]=opp;
                if (WinCheck(i/3,i%3,opp))
                {
                    board[i]=-1;
                    return false;
                }
                board[i]=-1;
            }
        }
        return true;
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
        if (gameMode == GameMode.HumanVsHuman) humanPlayer = currentPlayer;

        UpdateUI();
        botThinking=false;
    }
    public void ResetBoardForTraining()
    {
        for (int i = 0; i < 9; i++)
        {
            board[i] = -1;
        }
        turn = 0;
        currentPlayer = (Random.value > 0.5f) ? 1 : 0;
    }
    void UpdateUI()
    {
        if (gameMode == GameMode.QTraining) return;
        GamesText.text="Games "+Games;
        X_ScoreText.text=""+X_Wins;
        O_ScoreText.text=""+O_Wins;
        DrawsText.text="" + Draws;
        spriteRenderer.sprite = Images[currentPlayer];
        TurnText.text="Turn";
    }
    IEnumerator StartMLAgentBotMove()
    {
        botThinking=true;

        AgentScript bot = (currentPlayer==1)? agentX:agentO;
        yield return new WaitForSeconds(waitTime);
        bot.RequestDecision();

        botThinking=false;
    }
    IEnumerator StartQTableBotMove()
    {
        botThinking = true;
        yield return new WaitForSeconds(waitTime);

        int action = qTable.GetBestMove(currentPlayer, board);
        QTurn(action);

        botThinking = false;
    }
    IEnumerator StartQTraining()
    {
        Debug.Log("Starting Q-table training...");

        // Disable visuals for speed
        Time.timeScale = 100f;
        qTable.Train();
        Time.timeScale = 1f;
        qTable.SaveQTables();
        Debug.Log("Training finished & Q-table saved");
        
        // Switch to play mode automatically
        gameMode=GameMode.HumanVsBot;
        humanPlayer=0;
        Games=0;
        X_Wins=0;
        O_Wins=0;
        Draws=0;
        Restart();
        yield return null;
    }
}
