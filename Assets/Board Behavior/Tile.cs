using System;

namespace TileGame
{
    public abstract class Tile
    {
        public TileID ID { get; protected set; } = TileID.InvalidTile;

        protected SimpleVector direction;
        protected bool state;

        public virtual void TrySetHighState()
        {
            this.state = true;
        }

        public virtual void TrySetHighStateAndDirection(SimpleVector direction)
        {
            this.TrySetHighState();
            this.direction = direction;
        }

        public virtual void ResetState()
        {
            this.state = false;
        }

        public virtual bool HasSignal()
        {
            return this.state;
        }

        public virtual (bool, bool) GetSignals()
        {
            throw new InvalidOperationException("This tile has one signal, so GetSignals() cannot be used. Instead, use GetSignal().");
        }

        public virtual SimpleVector GetDirection()
        {
            return this.direction;
        }

        public virtual (SimpleVector, SimpleVector) GetDirections()
        {
            throw new InvalidOperationException("This tile has one direction, so GetDirections() cannot be used. Instead, use GetDirection().");
        }

        public abstract Tile Clone();

        public abstract Char DebugDisplay();
    }

    public class EmptyTile : Tile
    {
        private bool collisionFlag;

        public EmptyTile(SimpleVector direction, bool state, bool collisionFlag = false)
        {
            base.ID = TileID.EmptyTile;
            base.direction = direction;
            base.state = state;
            this.collisionFlag = false;
        }

        public override void TrySetHighState()
        {
            base.state = !collisionFlag;
            this.collisionFlag = true;
        }

        public override void TrySetHighStateAndDirection(SimpleVector direction)
        {
            this.TrySetHighState();
            base.direction = direction;
        }

        public override void ResetState()
        {
            base.ResetState();
            this.collisionFlag = false;
        }

        public override Tile Clone()
        {
            return new EmptyTile(this.direction, this.state, this.collisionFlag);
        }

        public override char DebugDisplay()
        {
            if (state)
            {
                return '▓';
            }

            return '░'; 
        }
    }

    public class Splitter : Tile
    {

        public Splitter(bool state)
        {
            base.ID = TileID.Splitter;
            base.state = state;
        }

        public override void TrySetHighStateAndDirection(SimpleVector direction)
        {
            base.TrySetHighState();
            // The splitter tile does not have a direction.
        }

        public override Tile Clone()
        {
            return new Splitter(this.state);
        }

        public override char DebugDisplay()
        {
            if (state)
            {
                return 'S';
            }

            return 's';
        }
    }

    public class Redirector : Tile
    {

        private bool collisionFlag;

        public Redirector(SimpleVector direction, bool state, bool collisionFlag = false)
        {
            ID = TileID.Redirector;
            base.direction = direction;
            base.state = state;
            this.collisionFlag = false;
        }

        public override void TrySetHighState()
        {
            base.state = !collisionFlag;
            this.collisionFlag = true;
        }

        public override void TrySetHighStateAndDirection(SimpleVector direction)
        {
            this.TrySetHighState();
            // The redirector tile's direction is not changed by signals.
        }

        public override void ResetState()
        {
            base.state = false;
            this.collisionFlag = false;
        }

        public override Tile Clone()
        {
            return new Redirector(this.direction, this.state, this.collisionFlag);
        }

        public override char DebugDisplay()
        {
            if (state)
            {
                if(direction == SimpleVector.Left())
                {
                    return '\u25C0';
                }
                else if (direction == SimpleVector.Right())
                {
                    return '\u25B6';
                }
                else if (direction == SimpleVector.Up())
                {
                    return '\u25B2';
                }
                else if (direction == SimpleVector.Down())
                {
                    return '\u25BC';
                }
            }
            else
            {
                if (direction == SimpleVector.Left())
                {
                    return '\u25C2';
                }
                else if (direction == SimpleVector.Right())
                {
                    return '\u25B8';
                }
                else if (direction == SimpleVector.Up())
                {
                    return '\u25B4';
                }
                else if (direction == SimpleVector.Down())
                {
                    return '\u25BE';
                }
            }
            return '?';
        }
    }

    public class Wall : Tile
    {

        public Wall()
        {
            base.ID = TileID.Wall;
        }

        public override void TrySetHighState()
        {
            // Wall tiles do not have a high state.
        }

        public override void TrySetHighStateAndDirection(SimpleVector direction)
        {
            // Wall tiles do not have a high state or a direction.
        }

        public override void ResetState()
        {
            // There is no high state or direction to reset.
        }

        public override bool HasSignal()
        {
            // Wall tiles cannot have a signal.
            return false;
        }

        public override Tile Clone()
        {
            return new Wall();
        }

        public override char DebugDisplay()
        {
            return 'W';
        }
    }

    // todo - get direction(s)
    public class Jumper : Tile
    {

        // There can be two vectors in a Jumper tile, but only if they are perpendicular. 
        // Therefore we can represent the data within the tile as two vectors - one vertical and one horizontal.
        private SimpleVector xDirection;
        private bool xState;

        private SimpleVector yDirection;
        private bool yState;

        public Jumper(SimpleVector xDirection, bool xState, SimpleVector yDirection, bool yState)
        {
            base.ID = TileID.Jumper;
            if (xDirection.yComponent != 0)
            {
                throw new ArgumentException("The horizontal data in the jumper tile cannot contain a vector with a non-zero vertical direction component.");
            }

            if (yDirection.xComponent != 0)
            {
                throw new ArgumentException("The vertical data in the jumper tile cannot contain a vector with a non-zero horizontal direction component.");
            }

            this.xDirection = xDirection;
            this.xState = xState;
            this.yDirection = yDirection;
            this.yState = yState;
        }

        public Jumper(bool xState, bool yState) : this(SimpleVector.Zero(), xState, SimpleVector.Zero(), yState) { }

        public override void TrySetHighState()
        {
            throw new InvalidOperationException("The Jumper tile can contain two signals, so TrySetHighState() cannot be used. Instead, use TrySetHighStateAndDirection().");
        }

        public override void TrySetHighStateAndDirection(SimpleVector direction)
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

        public override void ResetState()
        {
            xState = false;
            yState = false;
        }

        public override bool HasSignal()
        {
            return this.xState || this.yState;
        }

        public override (bool, bool) GetSignals()
        {
            return (this.xState, this.yState);
        }

        public override SimpleVector GetDirection()
        {
            throw new InvalidOperationException("This tile has two directions, so GetDirection() cannot be used. Instead, use GetDirections().");
        }

        public override (SimpleVector, SimpleVector) GetDirections()
        {
            return (this.xDirection, this.yDirection);
        }

        public override Tile Clone()
        {
            return new Jumper(this.xDirection, this.xState, this.yDirection, this.yState);
        }

        public override char DebugDisplay()
        {
            return '#';
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
