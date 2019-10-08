using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Only stores PLAYER or DYNAMO
    public EntityType current_turn;

    public WorldManager world;
    void Awake()
    {
        world = GetComponent<WorldManager>();

    }
    // Start is called before the first frame update
    void Start()
    {
        // instantialize turn of the player first.
        // Assigning current_turn to dynamo
        // then end its turn will effectively call
        // player's on_turn()
        current_turn = EntityType.DYNAMO;
        end_turn();
    }
    public void end_turn()
    {
        current_turn =  current_turn == EntityType.DYNAMO?
            EntityType.PLAYER:EntityType.DYNAMO;
        for(int i = 0; i < world.dynamic_objs.Count; i++)
        {
            if(world.dynamic_objs[i].e_type == current_turn)
            {
                // Handles the on-turn event
                world.dynamic_objs[i].on_turn();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
