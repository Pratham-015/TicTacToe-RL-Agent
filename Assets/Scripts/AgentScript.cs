using UnityEngine;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentScript : Agent
{
    public float Valid=0.05f;
    public float InvalidPenalty=-3.0f;
    public int player;
    public GameManager gameManager;
    public void OnEpisodeBegan()
    {
        // player : 1 ; enemy : 0
        player=gameManager.Restart()+1;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        for (int i = 0; i < 9; i++)
        {
            sensor.AddObservation(gameManager.board[i]);    // 9 int
        }
        // Player = 1 : Red X
        // Player = 0 : Blue O
        sensor.AddObservation(player);    // 1 int
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        // One discrete action received : tile index (0 to 8)
        int a = actions.DiscreteActions[0];
        if (gameManager.board[a] != -1)
        {
            gameManager.Invalid++;
            AddReward(InvalidPenalty);
        }
        else
        {
            AddReward(Valid);
            player=1-player;
            gameManager.PlayerTurn(a/3,a%3);
        }
    }
}
