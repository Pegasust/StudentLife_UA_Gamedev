using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public GameObject free_space;
    public GameObject barrier;
    public GameObject player_prefab;
    public GameObject[] hostile_prefabs;
    public GameObject[] food_prefabs;
    public TextAsset level_txt;
    public SceneHandler scene_handler;
    public Dictionary<char, GameObject> char_2_go = new Dictionary<char, GameObject>();

    public const int RESOLUTION_WIDTH = 64;
    public const int OFFSET = 1;
    
    public GameObject[,] objs { get; private set; }
    public List<ActionPiece> dynamic_objs { get; private set; }
    public bool game_over;
    void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        GameObject s_handler = GameObject.Find("SceneHandler");
        if (!(s_handler is null))
        {
            scene_handler = s_handler.GetComponent<SceneHandler>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        char_2_go.Add(' ', free_space);
        char_2_go.Add('#', barrier);
        char_2_go.Add('P', player_prefab);
        char_2_go.Add('D', hostile_prefabs[0]);
        char_2_go.Add('S', barrier);

        string level_str = level_txt.text;
        string[] level_rows = level_str.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
                );
        int height = level_rows.Length;
        int width = level_rows[0].Length;
        objs = new GameObject[height,width];
        dynamic_objs = new List<ActionPiece>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                objs[y,x] = Instantiate(char_2_go[level_rows[y][x]],
                    position_from_world(x,y),
                    Quaternion.identity,this.transform);
                ActionPiece a = objs[y, x].GetComponent<ActionPiece>();
                a.assign_world_xy(x, y);
                if(TypeHelper.has_actions(a.e_type))
                {
                    dynamic_objs.Add(a);
                }
            }
        }
        game_over = false;
    }
    public Vector3 position_from_world(int worldx, int worldy)
    {
        float a = (float)RESOLUTION_WIDTH / 100f;
        float o = (float)OFFSET / (float)RESOLUTION_WIDTH;
        return new Vector3(a * ((float)worldx + o), a * ((float)worldy + o), 0);
    }
    //void prompt_for_movement(int desired_pos_x, int desired_pos_y)
    //{

    //}
    // Update is called once per frame
    void Update()
    {
        
    }
    public ActionPiece action_piece_at(int world_x, int world_y)
    {
        return objs[world_x, world_y].GetComponent<ActionPiece>();
    }
    public void destroy(bool game_over = true)
    {
        //Handle serializations here
        if (game_over)
        {
            scene_handler.display_lose_scene();
        }
        else;
        {
            // Prolly got to the house.
            //Should proceed to next stage but we don't have any.
            scene_handler.display_intro_scene();
        }

    }
}
