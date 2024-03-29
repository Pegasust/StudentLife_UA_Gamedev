﻿using UnityEngine;
using System.Collections;

public class ActionPiece : MonoBehaviour
{
    [SerializeField]
    public int action_count;
    public int current_count;
    public const float normal_velocity = 1.0f;

    [System.NonSerialized]
    public int world_x, world_y;
    public virtual EntityType e_type { get
        {
            return type;
        } }
    [SerializeField]
    public EntityType type;
    protected Rigidbody2D rigidbody2d;
    protected GameManager game_manager;
    protected WorldManager world;

    protected Vector3 target_position;
    public int target_world_x, target_world_y;
    // This is for game manager to easily set the coords
    // on instantiation
    public void assign_world_xy(int x, int y)
    {
        world_x = x; world_y = y;
        target_world_x = x; target_world_y = y;
    }
    protected virtual void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        game_manager = transform.parent.GetComponent<GameManager>();
        world = transform.parent.GetComponent<WorldManager>();
        target_position = transform.position;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {

    }
    protected virtual void FixedUpdate()
    {
        // TODO: a fancy way to transform to target_position
        transform.position = target_position;
        world_x = target_world_x;
        world_y = target_world_y;
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        if (is_my_turn() && current_count>0)
        {
            if(do_action() &&
                handle_interaction(
                    world.action_piece_at(
                        target_world_x, target_world_y))
                )
            {
                world.switch_ground(this);
                if(--current_count <= 0)
                {
                    end_turn();
                }
            }
            
        }
    }
    protected virtual void end_turn()
    {
        game_manager.end_turn();
    }
    /// <summary>
    /// Handles interaction with <paramref name="other"/>.
    /// Returns true if this action_piece should have
    /// decrement in action count.
    /// </summary>
    /// <param name="other"></param>
    /// <returns>true if current_action_count should be reduced</returns>
    protected virtual bool handle_interaction(ActionPiece other)
    {
        //Static objs can't move.
        return true;
    }
    protected bool is_my_turn()
    {
        return game_manager.current_turn == this.e_type;
    }
    /// <summary>
    /// This function should set @target_position
    /// and @world_position
    /// 
    ///Returns whether current_count should be reduced
    /// </summary>
    protected virtual bool do_action()
    {
        return true;
    }
    public virtual void on_turn()
    {
        current_count = action_count;
    }
    // This will also sets spatial target position
    public void set_target_position(int world_x, int world_y)
    {
        target_world_x = world_x;
        target_world_y = world_y;
        target_position = world.position_from_world(world_x, world_y);
    }
    public void update_target_pos(
        int delta_x, int delta_y)
    {
        float a = (float)WorldManager.RESOLUTION_WIDTH / 100f;
        target_position.x += a * (float)delta_x;
        target_position.y += a * (float)delta_y;
        target_world_x += delta_x;
        target_world_y += delta_y;
    }
    public virtual void destroy()
    {
        Destroy(this);
        // We can also do some fancy stuff here.
    }
}