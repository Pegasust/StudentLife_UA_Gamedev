using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TypeHelper
{
    public static bool has_actions(in EntityType inp)
    {
        switch(inp)
        {
            //case EntityType.FREE_SPACE:
            //case EntityType.STATIC_OBJECT:
            //case EntityType.BARRIER:
            //case EntityType.WALL:
            //case EntityType.SMALL_FOOD:
            //case EntityType.FOOD_SPEED_UP:
            //case EntityType.FOOD_VOMIT:
            //case
            case EntityType.DYNAMO:
            case EntityType.PLAYER:
                return true;
            default:
                return false;
        }
    }
    /// <summary>
    /// Returns movement cost that dynamic_obj loses when hitting target
    /// </summary>
    /// <param name="dynamic_obj"></param>
    /// <param name="target"></param>
    /// <returns>min = 1, max = int.MaxValue</returns>
    public static int get_movement_cost(in EntityType dynamic_obj, in EntityType target)
    {
        switch(target)
        {
            case EntityType.FREE_SPACE:
                return 1;
            case EntityType.STATIC_OBJECT:
            case EntityType.BARRIER:
                return int.MaxValue;
            case EntityType.WALL:
                // Player might be able to break walls
                return int.MaxValue;
            case EntityType.DYNAMO:
                return int.MaxValue;
            case EntityType.PLAYER:
                return 1;
            case EntityType.SMALL_FOOD:
            case EntityType.FOOD_SPEED_UP:
            case EntityType.FOOD_VOMIT:
                // Players see them as food,
                // monsters see them as free_space
                return 1;
            case EntityType.HOUSE:
                if(dynamic_obj == EntityType.PLAYER)
                {
                    return 1;
                }
                else //Dynamo
                {
                    // See it as a barrier.
                    return int.MaxValue;
                }
        }
        return int.MaxValue;
    }
}
public enum EntityType
{
    FREE_SPACE, //Objects are free to move in and out
    STATIC_OBJECT, //Objects should not be able to move into
    DYNAMO, //Hostile entities that can move and interact. Intelligence is questionable.
    PLAYER, //Object that the player has control over
    BARRIER, //Static_Object that can certainly cannot be broken.
    WALL, //Static_Object that may be broken.
    
    SMALL_FOOD, //Meant to fill up exp
    FOOD_SPEED_UP, //Eat to act more per turn
    FOOD_VOMIT, //Eat to gain ability to vomit


    HOUSE, //End point of a scene

}

