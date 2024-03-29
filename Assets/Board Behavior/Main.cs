﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TileGame
{
    public class Main : MonoBehaviour, IPointerClickHandler
    {
        Text displayText;
        Board testBoard = new Board(8, 8);

        void Start()
        {
            displayText = GetComponent<Text>();
            testBoard.board[3, 2] = new EmptyTile(SimpleVector.Down(), true);
            //testBoard.board[3, 3] = new Jumper(false, false);
            testBoard.board[2, 3] = new EmptyTile(SimpleVector.Right(), true);
            displayText.text = testBoard.DebugDisplay();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            testBoard.Update();
            displayText.text = testBoard.DebugDisplay();
        }
    }
}





















/*
namespace TileGame
{
    public class Main : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Board test = new Board(3, 3);
            Debug.Log(test.DisplaySignals());
            //test.board[3, 1] = new EmptyTile(SimpleVector.Left(), true);
            //test.board[0, 1] = new Redirector(SimpleVector.Right(), false);
            //test.board[0, 0] = new Redirector(SimpleVector.Right(), false);
            test.board[1, 0] = new EmptyTile(SimpleVector.Down(), true);
            test.board[0, 1] = new EmptyTile(SimpleVector.Right(), true);
            test.board[2, 1] = new EmptyTile(SimpleVector.Left(), true);
            //test.board[2, 1] = new EmptyTile(SimpleVector.Right(), true);
            //test.board[3, 0] = new Splitter(true);
            //test.board[3, 3] = new Redirector(SimpleVector.Up(), false);
            Debug.Log("---");
            Debug.Log(test.DisplaySignals());
            Debug.Log("UPDATING");
            test.Update();
            Debug.Log(test.DisplaySignals());
            Debug.Log("---");
            test.Update();
            Debug.Log(test.DisplaySignals());
            test.Update();
            Debug.Log(test.DisplaySignals());
            test.Update();
            Debug.Log(test.DisplaySignals());
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
*/