﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace TileGame
{
    public class Board
    {
        private readonly int xLength;
        private readonly int yLength;
        public Tile[,] board;

        public Board(int xLength, int yLength)
        {
            if (xLength < 1)
            {
                throw new ArgumentOutOfRangeException("When initializing a board, the x length cannot be less than 1.");
            }

            if (yLength < 1)
            {
                throw new ArgumentOutOfRangeException("When initializing a board, the y length cannot be less than 1.");
            }

            this.xLength = xLength;
            this.yLength = yLength;

            board = new Tile[xLength, yLength];

            // Initialize the Tile class in each entry of board with the default values
            for (int j = 0; j < yLength; j++)
            {
                for (int i = 0; i < xLength; i++)
                {
                    board[i, j] = new Tile(TileID.EmptyTile, SimpleVector.Zero(), false);
                }
            }
        }

        public void Update()
        {
            Board nextBoardState = this.Clone();

            // The clone may have some states set to true, which will interfere with the update process.
            // We need to turn off all of the state states on this board before we update it.
            // We also need to tell it that no states have been true this update.
            for (int j = 0; j < yLength; j++)
            {
                for (int i = 0; i < xLength; i++)
                {
                    nextBoardState.board[i, j].ResetState();
                }
            }

            for (int j = 0; j < yLength; j++)
            {
                for (int i = 0; i < xLength; i++)
                {
                    Tile originTile = board[i, j];

                    //This is where tile behaviors are defined.
                    switch (originTile.ID)
                    {
                        case TileID.EmptyTile:
                            if (originTile.HasSignal())
                            {
                                Tuple<int, int> direction = originTile.direction.GetComponents();
                                int xToEdit = i + direction.Item1;
                                int yToEdit = j + direction.Item2;
                                if (nextBoardState.IsValidPosition(xToEdit, yToEdit))
                                {
                                    Tile newBoardTile = nextBoardState.board[xToEdit, yToEdit];
                                    newBoardTile.TrySetHighStateAndDirection(originTile.direction.Clone());
                                }
                            }
                            break;

                        case TileID.Splitter:
                            if (originTile.HasSignal())
                            {
                                if (nextBoardState.IsValidPosition(i, j - 1))
                                {
                                    nextBoardState.board[i, j - 1].TrySetHighStateAndDirection(SimpleVector.Up());
                                }
                                if (nextBoardState.IsValidPosition(i, j + 1))
                                {
                                    nextBoardState.board[i, j + 1].TrySetHighStateAndDirection(SimpleVector.Down());
                                }
                                if (nextBoardState.IsValidPosition(i-1, j))
                                {
                                    nextBoardState.board[i - 1, j].TrySetHighStateAndDirection(SimpleVector.Left());
                                }
                                if (nextBoardState.IsValidPosition(i + 1, j))
                                {
                                    nextBoardState.board[i + 1, j].TrySetHighStateAndDirection(SimpleVector.Right());
                                }
                            }
                            break;

                        case TileID.Redirector:
                            if (originTile.HasSignal())
                            {
                                Tuple<int, int> direction = originTile.direction.GetComponents();
                                if (nextBoardState.IsValidPosition(i + direction.Item1, j + direction.Item2))
                                {
                                    nextBoardState.board[i + direction.Item1, j + direction.Item2].TrySetHighStateAndDirection(originTile.direction.Clone());
                                }
                            }
                            break;

                        case TileID.Wall:

                            break;

                        case TileID.Jumper:
                            // This is by far the most complicated behavior.
                            // We need to access TWO vectors from the tile, 
                            // This behavior probably requires a Jumper class inheriting from Tile
                            break;

                        default:
                            throw new KeyNotFoundException("Tile behavior is not defined for this ID.");

                    }
                }
            }
            board = nextBoardState.board;
        }

        public Board Clone()
        {
            Board boardClone = new Board(xLength, yLength);
            for (int j = 0; j < yLength; j++)
            {
                for (int i = 0; i < xLength; i++)
                {
                    Tile currentTile = board[i, j];
                    boardClone.board[i,j] = currentTile.Clone();
                }
            }
            return boardClone;
        }

        public bool IsValidPosition(int xPos, int yPos)
        {
            if(xPos >= 0 && xPos < xLength && yPos >= 0 && yPos < yLength)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int j = 0; j < yLength; j++)
            {
                for (int i = 0; i < xLength; i++)
                {
                    sb.Append(board[i, j].HasSignal());
                    sb.Append(", ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public String DisplaySignals()
        {
            // This is just for testing
            // if actually used then the function needs to be changed to use stringbuilder
            String builtString = "";
            for (int j = 0; j < yLength; j++)
            {
                for (int i = 0; i < xLength; i++)
                {
                    if (board[i, j].HasSignal())
                    {
                        builtString = builtString + "▓";
                    }
                    else
                    {
                        builtString = builtString + "░";
                    }
                }
                builtString = builtString + "\n";
            }
            return builtString;
        }
    }

    public class Tile
    {
        public TileID ID;
        public SimpleVector direction;
        private bool state;
        public bool stateHasBeenTrueThisUpdate;

        public Tile(TileID ID, SimpleVector direction, bool state)
        {
            this.ID = ID;
            this.direction = direction;
            this.state = state;
            this.stateHasBeenTrueThisUpdate = false;
        }

        public void ResetState()
        {
            this.state = false;
            this.stateHasBeenTrueThisUpdate = false;
        }

        public void TrySetHighState()
        {
            // Handle signal collisions on empty tiles.
            if (ID == TileID.EmptyTile)
            {
                this.state = true && !stateHasBeenTrueThisUpdate;
            }
            else
            {
                this.state = true;
            }
            this.stateHasBeenTrueThisUpdate = true;
        }

        public void TrySetHighStateAndDirection(SimpleVector direction)
        {
            TrySetHighState();

            // The direction is only changed for empty tiles.
            if (ID == TileID.EmptyTile)
            {
                this.direction = direction;
            }
        }

        public bool HasSignal()
        {
            return state;
        }

        public Tile(TileID ID, SimpleVector direction, bool state, bool stateHasBeenTrueThisUpdate)
        {
            this.ID = ID;
            this.direction = direction;
            this.state = state;
            this.stateHasBeenTrueThisUpdate = false;
        }

        public Tile Clone()
        {
            return new Tile(ID, direction.Clone(), state, stateHasBeenTrueThisUpdate);
        }
    }

    public enum TileID
    {   
        EmptyTile,
        Splitter,
        Redirector,
        Wall,
        Jumper
    }
}