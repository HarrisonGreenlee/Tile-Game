using System;
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
                    board[i, j] = new Tile(TileID.EmptyTile, SimpleVector.Zero());
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
        public const TileID ID = TileID.InvalidTile;
        public SimpleVector direction;
        protected bool state;

        public Tile(SimpleVector direction, bool state)
        {
            this.direction = direction;
            this.state = state;
        }

        public virtual void ResetState()
        {
            this.state = false;
        }

        public virtual void TrySetHighState()
        {
            this.state = true;
        }

        public virtual void TrySetHighStateAndDirection(SimpleVector direction)
        {
            TrySetHighState();
        }

        public bool HasSignal()
        {
            return state;
        }

        public Tile Clone()
        {
            return new Tile(direction.Clone(), state);
        }
    }

    public class EmptyTile : Tile
    {
        public readonly TileID ID = TileID.EmptyTile;
        public bool stateHasBeenTrueThisUpdate;

        public EmptyTile(SimpleVector direction, bool state) : base(direction, state)
        {
            this.stateHasBeenTrueThisUpdate = false;
        }

        public override void TrySetHighState()
        {
            this.state = true && !stateHasBeenTrueThisUpdate;
            this.stateHasBeenTrueThisUpdate = true;
        }

        public override void TrySetHighStateAndDirection(SimpleVector direction)
        {
            base.TrySetHighStateAndDirection(direction);
            this.direction = direction;
        }

        public override void ResetState()
        {
            base.ResetState();
            this.stateHasBeenTrueThisUpdate = false;
        }
    }
    public class Splitter : Tile
    {
        public readonly TileID ID = TileID.Splitter;

        public Splitter(SimpleVector direction, bool state) : base(direction, state) { }
    }

    public class Redirector : EmptyTile // We inherit from empty tile because most of the behavior is identical
    {
        public readonly TileID ID = TileID.Redirector;
        
        public Redirector(SimpleVector direction, bool state) : base(direction, state) { }

        public override void TrySetHighStateAndDirection(SimpleVector direction)
        {
            base.TrySetHighStateAndDirection(direction);
            // Don't set direction.
        }
    }

    public class Wall : Tile
    {
        public readonly TileID ID = TileID.Wall;

        public Wall(SimpleVector direction, bool state) : base(direction, state) { }

        // The wall tile cannot have a high state.
        public override void TrySetHighState()
        {
            
        }

        // The wall tile cannot have a high state or a direction.
        public override void TrySetHighStateAndDirection(SimpleVector direction)
        {

        }

        // The wall tile cannot have a high state, so resetting is not neccesary.
        public override void ResetState()
        {

        }
    }

    public class Jumper : Tile
    {
        public readonly TileID ID = TileID.Jumper;
        public bool stateHasBeenTrueThisUpdate;
        public SimpleVector direction2;
        protected bool state2;
        public bool stateHasBeenTrueThisUpdate2;

        public Jumper(SimpleVector direction, bool state, SimpleVector direction2, bool state2) : base(direction, state)
        {
            this.direction2 = direction2;
            this.state2 = state2;
        }

        public override void TrySetHighState()
        {
            throw new InvalidOperationException("The Jumper tile can contain two signals, so TrySetHighState() cannot be used. Instead, use TrySetHighStateAndDirection().");
        }

        public override void TrySetHighStateAndDirection(SimpleVector direction)
        {
            //TODO
            base.TrySetHighStateAndDirection(direction);
            this.direction = direction;
        }

        public override void ResetState()
        {

            base.ResetState();
            this.stateHasBeenTrueThisUpdate = false;
            this.state2 = false;
            this.stateHasBeenTrueThisUpdate2 = false;
        }
    }

    public enum TileID
    {   
        InvalidTile,
        EmptyTile,
        Splitter,
        Redirector,
        Wall,
        Jumper
    }
}