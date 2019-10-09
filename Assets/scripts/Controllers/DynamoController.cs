using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class NavNode
{
    /// <summary>
    /// current position
    /// </summary>
    public int x, y;
    /// <summary>
    /// previous position
    /// </summary>
    public NavNode previous;
    /// <summary>
    /// movement cost from the starting point
    /// </summary>
    public int g;
    /// <summary>
    /// estimated distance from destination (city block distance)
    /// or duong` chim bay in vnmese
    /// </summary>
    public int h;
    /// <summary>
    /// score upon moving to this point
    /// </summary>
    public int f;
    /// <summary>
    /// Premature implementation of city block dist
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="target_x"></param>
    /// <param name="target_y"></param>
    /// <returns></returns>
    public static int calculate_city_block_dist(int x, int y,
        int target_x, int target_y)
    {
        return Math.Abs(target_x - x) + Math.Abs(target_y - y);
    }
    public override bool Equals(object obj)
    {
        NavNode o = obj as NavNode;
        if(o is null)
        {
            return false;
        }
        return (this.x == o.x) && (this.y == o.y);
    }
    public static bool operator ==(NavNode self, NavNode other)
    {
        if(self is null)
        {
            return other is null;
        }
        else if(other is null)
        {
            return false;
        }
        return self.x == other.x && self.y == other.y;
    }
    public static bool operator !=(NavNode self, NavNode other)
    {
        return !(self == other);
    }
    public override int GetHashCode()
    {
        return string.Format("{0}_{1}", x, y).GetHashCode();
    }
    /// <summary>
    /// This algorithm returns the target node and
    /// expect us to do previous checking.
    /// Highly recommend swapping world and target positions
    /// </summary>
    /// <param name="worldx"></param>
    /// <param name="worldy"></param>
    /// <param name="targetx"></param>
    /// <param name="targety"></param>
    /// <param name="world"></param>
    /// <returns></returns>
    public static NavNode astar(in int worldx, in int worldy,
        in int targetx, in int targety, in WorldManager world)
    {
        NavNode start = new NavNode
        {
            x = worldx,
            y = worldy
        };
        NavNode target = new NavNode
        {
            x = targetx,
            y = targety
        };


        // list that consists of nav_nodes worth checking
        List<NavNode> open_list = new List<NavNode>();
        // list that consists of nav_nodes not worth checking
        List<NavNode> black_list = new List<NavNode>();
        int g = 0;
        open_list.Add(start);
        NavNode current = null;

        while(open_list.Count > 0)
        {
            //Alg logic:
            // get minimum f of open_list
            // use var just in case we change data type of f
            var lowest_f = open_list.Min(n => n.f);
            current = open_list.First(n => (n.f == lowest_f));
            black_list.Add(current);
            open_list.Remove(current);
            // if black_list contains target and we found the path
            if(black_list.FirstOrDefault(node => (node == target))
                != null)
            {
                break;
            }
            List<NavNode> adjacents = 
                get_plausible_adjacents(current.x, current.y,
                world);
            g++;
            for(int i = 0; i < adjacents.Count; i++)
            {
                // if already exist in black_list
                if (black_list.FirstOrDefault(
                    node=>node==adjacents[i])!= null)
                {
                    continue;
                }
                // if not exist in open_list
                if(open_list.FirstOrDefault(
                    node => node == adjacents[i]) == null)
                {
                    adjacents[i].g = g;
                    adjacents[i].h = calculate_city_block_dist(
                        adjacents[i].x, adjacents[i].y,
                        target.x, target.y);
                    adjacents[i].f = adjacents[i].g + adjacents[i].h;
                    adjacents[i].previous = current;
                    //add to open_list
                    open_list.Insert(0, adjacents[i]);
                }
                else
                {
                    //if there is a better path
                    if(g+adjacents[i].h < adjacents[i].f)
                    {
                        adjacents[i].g = g;
                        adjacents[i].f = adjacents[i].g + adjacents[i].h;
                        adjacents[i].previous = current;
                    }
                }
            }

        }
        return current;
    }
    static List<NavNode> get_plausible_adjacents(in int x, in int y,
        in WorldManager world)
    {
        List<NavNode> nodes = new List<NavNode>()
        {
            new NavNode{ x=x,y=y-1 },
            new NavNode{ x=x,y=y+1 },
            new NavNode{ x=x-1,y=y },
            new NavNode{ x=x+1, y=y },
        };
        List<NavNode> ret = new List<NavNode>();
        int min_cost = int.MaxValue;
        for(int i = 0; i < nodes.Count; i++)
        {
            if(nodes[i].x < 0 || nodes[i].y < 0 || 
                nodes[i].y>= world.objs.GetLength(0) ||
                nodes[i].x>= world.objs.GetLength(1))
            {
                //out of bounds
                continue;
            }
            //for each node in nodes, see if it has the min cost.
            int cost = TypeHelper.get_movement_cost(
                world.action_piece_at(x, y).e_type,
                world.action_piece_at(nodes[i].x, nodes[i].y).e_type);
            if (min_cost == cost)
            {
                ret.Add(nodes[i]);
            }
            else if(min_cost>cost)
            {
                ret = new List<NavNode>();
                ret.Add(nodes[i]);
                min_cost = cost;
            }
        }
        return ret;
    }
}
/// <summary>
/// Dum dum Dynamo doesn't know how to trace the player
/// </summary>
public class DynamoController : ActionPiece
{
    //Awake()
    //Start()
    //FixedUpdate()
    //Update()

    /// <summary>
    /// Only traces towards the player.
    /// TODO: This can be optimized by storing lots of nearby
    /// node, and in case the player is in one of the nodes,
    /// just return the new path instead of re-calculating everything.
    /// </summary>
    /// <returns></returns>
    protected override bool do_action()
    {
        NavNode path_traced_node = NavNode.astar(
            world_x, world_y,
            world.player.world_x, world.player.world_y, world);
        if (path_traced_node is null)
        {
            //No path to take.
            Debug.LogError("No path to take.");
            return true; //Skip turn
        }
        NavNode correct_node = path_traced_node;
        while (!(path_traced_node.previous is null))
        {
            correct_node = path_traced_node;
            path_traced_node = path_traced_node.previous;
        }
        // path_traced_node.previous is null
        set_target_position(correct_node.x,
            correct_node.y);
        return true;
    }
    protected override bool handle_interaction(ActionPiece other)
    {
        switch (other.e_type)
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
                // IT SHOULD NOT BE IN HERE.
                if(other == this)
                {
                    return true;
                }
                throw new Exception("Dynamoes aren't allowed to touch each other.");

            case EntityType.PLAYER:
                other.destroy();
                world.destroy();
                return true;
            case EntityType.SMALL_FOOD:
            case EntityType.FOOD_VOMIT:
            case EntityType.FOOD_SPEED_UP:
                //TODO: Implment this for food handling on monsters
                return true;
            case EntityType.HOUSE:
                // should not be here either
                throw new Exception("Dynamoes aren't allowed home.");
                return false;

        }
        return false;
    }
}
