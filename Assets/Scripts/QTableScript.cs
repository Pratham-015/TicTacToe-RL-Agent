using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTableScript : MonoBehaviour
{
    public GameManager gameManager;
    Dictionary<int,float[]> QX=new Dictionary<int,float[]>();
    Dictionary<int, float[]> QO = new Dictionary<int, float[]>();
    [Header("Training Parameters")]
    public float alpha = 0.1f;   // learning rate
    public float gamma = 0.95f;  // discount
    public float epsilon = 1.0f; // exploration
    public float epsilonMin = 0.05f;
    public float epsilonDecay = 0.9995f;
    [Header("Episode Parameters")]
    public int maxEpisode=25000;
    public int summaryFreq=1000;
    [Header("Last Info")]
    int lastStateX=-1;
    int lastActionX=-1;
    int lastStateO=-1;
    int lastActionO=-1;

    public void Train()
    {
        for (int episode=0; episode < maxEpisode; episode++)
        {
            gameManager.ResetBoardForTraining();
            bool done=false;
            lastStateX = lastStateO = -1;
            lastActionX = lastActionO = -1;

            while (!done)
            {
                int player=gameManager.currentPlayer;
                int state=EncodeState(gameManager.board);
                
                int status;
                int action;
                if (player == 1) 
                {
                    action = ChooseAction(QX, state, gameManager.board);
                    lastStateX=state;
                    lastActionX=action;
                }
                else
                {
                    action = ChooseAction(QO, state, gameManager.board);
                    lastStateO=state;
                    lastActionO=action;
                }

                status=gameManager.QTurn(action);
                int nextState=EncodeState(gameManager.board);

                float reward1=0,reward2=0;
                if (status == 1)
                {
                    done = true;
                    reward1=gameManager.WinReward;
                    reward2=gameManager.LosePenalty;
                }
                else if (status == 2)
                {
                    done=true;
                    reward1=gameManager.DrawReward;
                    reward2=gameManager.DrawReward;
                }
                
                if (player == 1)
                {
                    UpdateQ(QX, state, action, reward1, nextState, done);
                }
                else
                {
                    UpdateQ(QO, state, action, reward1, nextState, done);
                }
                if (done)
                {
                    if (player == 1 && lastActionO != -1)
                        UpdateQ(QO, lastStateO, lastActionO, reward2, nextState, true);
                    else if (player == 0 && lastActionX != -1)
                        UpdateQ(QX, lastStateX, lastActionX, reward2, nextState, true);
                }
            }
            if (episode % summaryFreq == 0)
            {
                Debug.Log($"Episode {episode} X Wins {gameManager.X_Wins} O Wins {gameManager.O_Wins} Draws {gameManager.Draws}");
            }
            epsilon = Mathf.Max(epsilonMin, epsilon * epsilonDecay);
        }
        Debug.Log("Two-agent Q-learning complete");
    }
    int EncodeState(int[] board)
    {
        int state=0;
        int base3=1;
        int v;
        for (int i = 0; i < 9; i++)
        {
            v=board[i]+1;
            state+=v*base3;
            base3*=3;
        }
        // Returns a number between 0 and 19682
        return state;
    }
    float[] GetQValues(Dictionary<int,float[]> Q,int state)
    {
        // If state encountered for first time
        // Initialize an array of float
        if (!Q.ContainsKey(state))
        {
            Q[state]=new float[9];
        }
        return Q[state];
    }
    int ChooseAction(Dictionary<int,float[]> Q,int state, int[] board)
    {
        // Exploration
        if (Random.value < epsilon)
        {
            List<int> valid = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == -1)
                {
                    valid.Add(i);
                }
            }
            return valid[Random.Range(0,valid.Count)];
        }

        // Exploitation
        float[] q=GetQValues(Q,state);
        float best = float.NegativeInfinity;
        int bestAction=-1;
        for (int i = 0; i < 9; i++)
        {
            if (board[i]!=-1)   continue;
            if (q[i] > best)
            {
                best=q[i];
                bestAction=i;
            }
        }
        if (bestAction == -1)
        {
            for (int i = 0; i < 9; i++)
                if (board[i] == -1)
                    return i;
        }
        return bestAction;
    }
    void UpdateQ(
        Dictionary<int,float[]> Q,
        int state,
        int action,
        float reward,
        int nextState,
        bool done)
    {
        float[] q=GetQValues(Q,state);
        float[] qNext = GetQValues(Q,nextState);
        float maxNext = 0f;
        if (!done)
        {
            maxNext = float.NegativeInfinity;
            for (int i = 0; i < 9; i++)
                if (qNext[i] > maxNext)
                    maxNext = qNext[i];
        }
        q[action]+= alpha*(reward+gamma*maxNext-q[action]);
    }

[System.Serializable]
public class QEntry
{
    public int state;
    public float[] q;
}
[System.Serializable]
public class QTableData
{
    public List<QEntry> QX = new List<QEntry>();
    public List<QEntry> QO = new List<QEntry>();
}
public void SaveQTables()
{
    QTableData data = new QTableData();

    foreach (var kv in QX)
        data.QX.Add(new QEntry { state = kv.Key, q = kv.Value });

    foreach (var kv in QO)
        data.QO.Add(new QEntry { state = kv.Key, q = kv.Value });

    string json = JsonUtility.ToJson(data, true);
    string path = Application.persistentDataPath + "/qtable.json";

    System.IO.File.WriteAllText(path, json);

    Debug.Log("Q-table saved to: " + path);
}
public void LoadQTables()
{
    string path = Application.persistentDataPath + "/qtable.json";

    if (!System.IO.File.Exists(path))
    {
        Debug.LogWarning("No Q-table found");
        return;
    }

    string json = System.IO.File.ReadAllText(path);
    QTableData data = JsonUtility.FromJson<QTableData>(json);

    QX.Clear();
    QO.Clear();

    foreach (var e in data.QX)
        QX[e.state] = e.q;

    foreach (var e in data.QO)
        QO[e.state] = e.q;

    Debug.Log("Q-table loaded");
}
public int GetBestMove(int player, int[] board)
{
    int state = EncodeState(board);
    var Q = (player == 1) ? QX : QO;

    float[] q = GetQValues(Q, state);

    float best = float.NegativeInfinity;
    int bestAction = -1;

    for (int i = 0; i < 9; i++)
    {
        if (board[i] != -1) continue;
        if (q[i] > best)
        {
            best = q[i];
            bestAction = i;
        }
    }
    return bestAction;
}
}
