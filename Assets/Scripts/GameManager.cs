using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int currentTurn=0;
    public int[] board=new int[9];
    void Start()
    {
        Restart();
    }
    public int PlayerTurn()
    {
        currentTurn++;
        return currentTurn%2;
    }
    /*public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }*/
    public int WinCheck(int i,int j,int turn)
    {
        if ((board[i*3] == turn) & (board[i*3 + 1] == turn) & (board[i*3 + 2] == turn))
        {
            AddScore(turn);
            return 1;
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
        if (turn == 1)
        {
            Debug.Log("Player won");
        }
        else if (turn == 0)
        {
            Debug.Log("Enemy won");
        }
        Restart();
        return 1;
    }
    public void Restart()
    {
        for (int i = 0; i < 9; i++)
        {
            board[i]=-1;
        }
        currentTurn=0;
        Debug.Log("Restart");
    }
}
