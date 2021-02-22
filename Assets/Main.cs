using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TileGame
{
    public class Main : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Board test = new Board(5, 5);
            Debug.Log(test.DisplaySignals());
            //test.board[1, 2].ID = TileID.Splitter;
            //test.board[1, 2].TrySetHighStateAndDirection(SimpleVector.Right());
            //test.board[2, 2].ID = TileID.Splitter;
            //test.board[2, 2].TrySetHighStateAndDirection(SimpleVector.Left());
            test.board[1, 0].ID = TileID.Redirector;
            test.board[1, 0].direction = SimpleVector.Right();
            test.board[3, 0].ID = TileID.Redirector;
            test.board[3, 0].direction = SimpleVector.Left();
            test.board[3, 0].TrySetHighState();
            test.board[2, 0].ID = TileID.Wall;
            Debug.Log("---");
            Debug.Log(test.DisplaySignals());
            Debug.Log("UPDATING");
            test.Update();
            Debug.Log(test.board[1, 2].direction.GetComponents());
            Debug.Log(test.DisplaySignals());
            Debug.Log("---");
            Debug.Log(test.board[1, 2].direction.GetComponents());
            test.Update();
            Debug.Log(test.DisplaySignals());
            Debug.Log(test.board[1, 2].direction.GetComponents());
            test.Update();
            Debug.Log(test.DisplaySignals());
            test.Update();
            Debug.Log(test.DisplaySignals());
            test.Update();
            Debug.Log(test.DisplaySignals());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}