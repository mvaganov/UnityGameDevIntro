using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MrV;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    void Start()
    {
        string m = MazeGen.CreateMaze(new Coord(15, 9), Coord.One, 123);
        Debug.Log(m);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
