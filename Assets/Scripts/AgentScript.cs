using UnityEngine;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentScript : Agent
{
    public int player;
    public GameManager gameManager;
    public AgentScript opponent;
    
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
    public override void WriteDiscreteActionMask(IDiscreteActionMask mask)
    {
        for (int i = 0; i < 9; i++)
        {
            if (gameManager.board[i] != -1)
                mask.SetActionEnabled(0, i, false);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // One discrete action received : tile index (0 to 8)
        int a = actions.DiscreteActions[0];
        gameManager.AgentTurn(a,this);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discrete = actionsOut.DiscreteActions;
        for (int i = 0; i < 9; i++)
        {
            if (gameManager.board[i] == -1)
            {
                discrete[0] = i;
                return;
            }
        }
    }
}
