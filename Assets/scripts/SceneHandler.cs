using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler
{
    //private void Awake()
    //{
    //    DontDestroyOnLoad(this.gameObject);
    //}
    public static void display_start_scene()
    {
        SceneManager.LoadScene("start_scene", LoadSceneMode.Single);
    }
    public static void display_intro_scene()
    {
        // Not implemented yet
    }
    public static void display_game_scene()
    {
        SceneManager.LoadScene("game_scene", LoadSceneMode.Single);
    }
    public static void display_lose_scene()
    {
        SceneManager.LoadScene("lose_scene");
    }
}
