using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class FoodPoints
{

}
public class FoodMeter : MonoBehaviour
{
    [SerializeField]
    public int E_T0 = 10;
    [SerializeField]
    public int a = 1; //Linear
    [SerializeField]
    public int E_TMAX = 100;

    public int level = 0;
    // Number of small food needs to collect to reach the next level
    // Expands the ability slot
    // E = E_T0 + level(E_TMAX - ET0)^a
    public int calculate_E()
    {
        return Mathf.RoundToInt(Mathf.Pow((float)(E_TMAX - E_T0),a)*level+E_T0);
    }
    public int current_E
    {
        get
        {
            return _E_cache;
        }
    }
    private int _E_cache;
    private int points = 0;
    public void on_collect_food(int points_yield)
    {
        points += points_yield;
        if(points>=current_E)
        {
            on_level_up();
        }
    }
    public void on_level_up()
    {
        level++;
        _E_cache = calculate_E();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
