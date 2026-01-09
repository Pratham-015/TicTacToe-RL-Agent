using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    public GameObject OP;
    public GameObject IN;
    public GameObject GM;
    public void Instructions()
    {
        OP.SetActive(false);
        IN.SetActive(true);
    }
    public void InstructionsOff()
    {
        OP.SetActive(true);
        IN.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void GameMode()
    {
        OP.SetActive(false);
        GM.SetActive(true);
    }
    public void GameModeOff()
    {
        OP.SetActive(true);
        GM.SetActive(false);
    }
}
