using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SceneManage : MonoBehaviour
{
    public GameObject OP;
    public GameObject Panel;
    public GameObject IN;
    public GameObject GM;
    public GameObject GB;
    public GameObject Tile;
    public GameManager gameManager;
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
    public void MLA()
    {
        SceneManager.LoadScene(1);
    }
    public void QTB()
    {
        SceneManager.LoadScene(2);
    }
    public void MMLoad()
    {
        SceneManager.LoadScene(0);
    }
    public void Hooman()
    {
        SceneManager.LoadScene(3);
    }
    public void pause()
    {
        Panel.SetActive(true);
        GB.SetActive(false);
        Tile.SetActive(false);
    }
    public void resume()
    {
        Panel.SetActive(false);
        GB.SetActive(true);
    }
    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
    public void rest()
    {
        Panel.SetActive(false);
        GB.SetActive(true);
        gameManager.Restart();
        Tile.SetActive(true);
    }
}
