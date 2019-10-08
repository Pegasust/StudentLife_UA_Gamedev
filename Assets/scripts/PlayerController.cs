using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ActionPiece
{
    protected FoodMeter food_mtr;

    public new const float normal_velocity = 1.0f;
    public const float DEADZONE = 0.2f;
    public override EntityType e_type
    {
        get
        {
            return EntityType.PLAYER;
        }
    }

    protected new void Awake()
    {
        base.Awake();
        food_mtr = GetComponent<FoodMeter>();
    }
    protected override bool do_action()
    {
        if(handle_movement())
        {
            return true;
        }
        //Food interactions

        return false;
    }
    //Also update worldxy, posxy
    public void update_target_pos(
        int delta_x,int delta_y)
    {
        float a = (float)WorldManager.RESOLUTION_WIDTH / 100f;
        target_position.x += a * (float)delta_x;
        target_position.y += a * (float)delta_y;
        target_world_x += delta_x;
        target_world_y += delta_y;
    }
    bool handle_movement()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(vertical) < DEADZONE && Mathf.Abs(horizontal) < DEADZONE)
        {
            return false;
        }
        if (Mathf.Abs(horizontal) >= Mathf.Abs(vertical))
        {
            update_target_pos(Mathf.RoundToInt(Mathf.Sign(horizontal)), 0);
        }
        else
        {
            update_target_pos(0,Mathf.RoundToInt(Mathf.Sign(vertical)));

        }
        return true;
    }
    /// <summary>
    /// Handles interaction with <paramref name="other"/>.
    /// Returns true if this action_piece should have
    /// decrement in action count.
    /// </summary>
    /// <param name="other"></param>
    /// <returns>true if current_action_count should be reduced</returns>
    protected override bool handle_interaction(ActionPiece other)
    {
        Debug.Log(other);
        switch(other.e_type)
        {
            case EntityType.FREE_SPACE:
                // Preserves the target positions
                return true;
            case EntityType.STATIC_OBJECT:
            case EntityType.BARRIER:
                //Can't access that tile. Fall back.
                set_target_position(this.world_x, this.world_y);
                return false;
            case EntityType.WALL:
                // Might be able to break a wall
                // Handle it here
                //Otherwise
                set_target_position(this.world_x, this.world_y);
                return false;
            case EntityType.DYNAMO:
                // Game over for now
                Destroy(this);
                world.destroy();
                return true;
            case EntityType.SMALL_FOOD:
                food_mtr.on_collect_food(1);
                other.destroy();
                return true;
            case EntityType.HOUSE:
                // Gratz, you won the scene
                world.destroy();
                return true;
                               
        }
        return false;
    }
    //bool handle_food_interaction()
    //{

    //}
}
