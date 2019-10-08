using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void display_start_scene()
    {
        SceneManager.LoadScene("start_scene", LoadSceneMode.Single);
    }
    public void display_intro_scene()
    {
        // Not implemented yet
    }
    public void display_game_scene()
    {
        SceneManager.LoadScene("game_scene", LoadSceneMode.Single);
    }
    public void display_lose_scene()
    {
        SceneManager.LoadScene("lose_scene");
    }
}
