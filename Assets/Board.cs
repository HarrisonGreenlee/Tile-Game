using System;
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
                                if (nextBoardState.IsValidPosition(i - 1, j))
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
                    boardClone.board[i, j] = currentTile.Clone();
                }
            }
            return boardClone;
        }

        public bool IsValidPosition(int xPos, int yPos)
        {
            if (xPos >= 0 && xPos < xLength && yPos >= 0 && yPos < yLength)
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

    public interface Tile
    {
        public TileID GetTileID();
        
        //TODO
        // public SimpleVector GetDirection();
        public void TrySetHighState();
        public void TrySetHighStateAndDirection(SimpleVector direction);
        public void ResetState();
        public bool HasSignal();
        public Tile Clone();
    }

    // This used to be the base tile class, but it was replaced by an interface.
    //public class Tile
    //{
    //    public const TileID ID = TileID.InvalidTile;
    //    public SimpleVector direction;
    //    protected bool state;

    //    public Tile(SimpleVector direction, bool state)
    //    {
    //        this.direction = direction;
    //        this.state = state;
    //    }

    //    public virtual void ResetState()
    //    {
    //        this.state = false;
    //    }

    //    public virtual void TrySetHighState()
    //    {
    //        this.state = true;
    //    }

    //    public virtual void TrySetHighStateAndDirection(SimpleVector direction)
    //    {
    //        TrySetHighState();
    //    }

    //    public bool HasSignal()
    //    {
    //        return state;
    //    }

    //    public Tile Clone()
    //    {
    //        return new Tile(direction.Clone(), state);
    //    }
    //}

    public class EmptyTile : Tile
    {
        public readonly TileID ID = TileID.EmptyTile;

        private SimpleVector direction;
        private bool state;
        private bool collisionFlag;

        public EmptyTile(SimpleVector direction, bool state, bool collisionFlag = false)
        {
            this.direction = direction;
            this.state = state;
            this.collisionFlag = false;
        }

        public TileID GetTileID()
        {
            return ID;
        }

        public void TrySetHighState()
        {
            this.state = !collisionFlag;
            this.collisionFlag = true;
        }

        public void TrySetHighStateAndDirection(SimpleVector direction)
        {
            this.TrySetHighState();
            this.direction = direction;
        }

        public void ResetState()
        {
            this.state = false;
            this.collisionFlag = false;
        }

        public bool HasSignal()
        {
            return this.state;
        }

        public Tile Clone()
        {
            return new EmptyTile(this.direction, this.state, this.collisionFlag); 
        }
    }

    public class Splitter : Tile
    {
        public readonly TileID ID = TileID.Splitter;
        private bool state;

        public Splitter(bool state)
        {
            this.state = state;
        }
        public TileID GetTileID()
        {
            return ID;
        }

        public void TrySetHighState()
        {
            this.state = true;
        }

        public void TrySetHighStateAndDirection(SimpleVector direction)
        {
            TrySetHighState();
            // The splitter tile does not have a direction.
        }

        public void ResetState()
        {
            this.state = false;
        }

        public bool HasSignal()
        {
            return this.state;
        }

        public Tile Clone()
        {
            return new Splitter(this.state);
        }
    }

    public class Redirector : Tile
    {
        public readonly TileID ID = TileID.Redirector;

        private SimpleVector direction;
        private bool state;
        private bool collisionFlag;

        public Redirector(SimpleVector direction, bool state, bool collisionFlag = false)
        {
            this.direction = direction;
            this.state = state;
            this.collisionFlag = false;
        }

        public TileID GetTileID()
        {
            return ID;
        }

        public void TrySetHighState()
        {
            this.state = !collisionFlag;
            this.collisionFlag = true;
        }

        public void TrySetHighStateAndDirection(SimpleVector direction)
        {
            this.TrySetHighState();
            // Don't set direction.
        }

        public void ResetState()
        {
            this.state = false;
            this.collisionFlag = false;
        }

        public bool HasSignal()
        {
            return this.state;
        }

        public Tile Clone()
        {
            return new Redirector(this.direction, this.state, this.collisionFlag);
        }
    }

    public class Wall : Tile
    {
        public readonly TileID ID = TileID.Wall;

        public TileID GetTileID()
        {
            return ID;
        }

        public void TrySetHighState()
        {
            // Wall tiles do not have a high state.
        }

        public void TrySetHighStateAndDirection(SimpleVector direction)
        {
            // Wall tiles do not have a high state or a direction.
        }

        public void ResetState()
        {
            // There is no high state or direction to reset.
        }

        public bool HasSignal()
        {
            // Wall tiles cannot have a signal.
            return false;
        }

        public Tile Clone()
        {
            return new Wall();
        }
    }

    public class Jumper : Tile
    {
        public readonly TileID ID = TileID.Jumper;

        // There can be two vectors in a Jumper tile, but only if they are perpendicular. 
        // Therefore we can represent the data within the tile as two vectors - one vertical and one horizontal.
        private SimpleVector xDirection;
        private bool xState;

        private SimpleVector yDirection;
        private bool yState;

        public Jumper(SimpleVector xDirection, bool xState, SimpleVector yDirection, bool yState)
        {
            if(this.xDirection.yComponent != 0)
            {
                throw new ArgumentException("The horizontal data in the jumper tile cannot contain a vector with a non-zero vertical direction component.");
            }

            if (this.yDirection.xComponent != 0)
            {
                throw new ArgumentException("The vertical data in the jumper tile cannot contain a vector with a non-zero horizontal direction component.");
            }

            this.xDirection = xDirection;
            this.xState = xState;
            this.yDirection = yDirection;
            this.yState = yState;
        }

        public TileID GetTileID()
        {
            return ID;
        }

        public void TrySetHighState()
        {
            throw new InvalidOperationException("The Jumper tile can contain two signals, so TrySetHighState() cannot be used. Instead, use TrySetHighStateAndDirection().");
        }

        public void TrySetHighStateAndDirection(SimpleVector direction)
        {
            bool hasXComponent = direction.xComponent != 0;
            bool hasYComponent = direction.yComponent != 0;
            if ((hasXComponent && hasYComponent) || (direction.xComponent == 0 && direction.yComponent == 0))
            {
                throw new ArgumentException("Jumper tiles can only store non-zero horizontal and vertical vectors.");
            }
            else if (hasXComponent)
            {
                this.xState = !this.xState; // handles horizontal collisions
                this.xDirection = direction; // This will set direction even if there is a collision, which isn't an issue.
            }
            else // hasYComponent
            {
                this.yState = !this.yState; // handles horizontal collisions
                this.yDirection = direction; // This will set direction even if there is a collision, which isn't an issue.
            }
        }

        public void ResetState()
        {
            xState = false;
            yState = false;
        }

        public bool HasSignal()
        {
            return this.xState || this.yState;
        }

        public Tile Clone()
        {
            return new Jumper(this.xDirection, this.xState, this.yDirection, this.yState);
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