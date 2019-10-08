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
    public Dictionary<char, GameObject> char_2_go = new Dictionary<char, GameObject>();
    /// <summary>
    /// Exception-free implementation of char_2_go[char].
    /// </summary>
    /// <param name="char_symbol"></param>
    /// <param name="go">Returns free_space if char_symbol not found in key.</param>
    /// <returns></returns>
    public bool try_get_prefab(in char char_symbol, out GameObject go)
    {
        bool r = char_2_go.TryGetValue(char_symbol, out go);
        if(!r)
        {
            go = free_space;
        }
        return r;
    }

    public const int RESOLUTION_WIDTH = 64;
    public const int OFFSET = 1;
    
    public GameObject[,] objs { get; private set; }
    public List<ActionPiece> dynamic_objs { get; private set; }
    public bool game_over;
    void Awake()
    {
        char_2_go.Add(' ', free_space);
        char_2_go.Add('#', barrier);
        char_2_go.Add('P', player_prefab);
        char_2_go.Add('D', hostile_prefabs[0]);
        char_2_go.Add('S', barrier);
        //DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        extract_objects();
        game_over = false;
    }

    private void extract_objects()
    {
        string level_str = level_txt.text;
        string[] level_rows = level_str.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
                );
        int height = level_rows.Length;
        int width = level_rows[0].Length;
        objs = new GameObject[height, width];
        dynamic_objs = new List<ActionPiece>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                char key = level_rows[y][x];
                if(key != ' ')
                {
                    // Spawn a free space for the stylistic look
                    // And do not assign this object to reference anywhere.
                    spawn_action_piece_prefab(y, x, free_space);
                }
                // C# initializes a declaration to null.
                GameObject prefab;
                try_get_prefab(key, out prefab);
                //Debug.Log(debug += " is referenced to " + prefab);
                ActionPiece a = spawn_action_piece_prefab(y, x, prefab);
                if (TypeHelper.has_actions(a.e_type))
                {
                    dynamic_objs.Add(a);
                }
            }
        }
    }

    private ActionPiece spawn_action_piece_prefab(int y, int x, GameObject prefab)
    {
        objs[y, x] = Instantiate(prefab,
            position_from_world(x, y),
            Quaternion.identity, this.transform);
        ActionPiece a = objs[y, x].GetComponent<ActionPiece>();
        a.assign_world_xy(x, y);
        return a;
    }
    /// <summary>
    /// A function to return Unity's relative position from world game obj
    /// </summary>
    /// <param name="worldx">x component of world position</param>
    /// <param name="worldy">y component of world position</param>
    /// <returns></returns>
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
        if(game_over)
        {
            destroy();
        }
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
            SceneHandler.display_lose_scene();
        }
        else;
        {
            // Prolly got to the house.
            //Should proceed to next stage but we don't have any.
            SceneHandler.display_intro_scene();
        }

    }
}
