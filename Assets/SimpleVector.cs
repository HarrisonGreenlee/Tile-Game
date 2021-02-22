using System;

namespace TileGame
{
    /// <summary>
    /// An extremely simplified 2D vector class. Can only rotate left and right by 90 degree increments.
    /// </summary>
    public class SimpleVector
    {
        public int xComponent;
        public int yComponent;
        
        public SimpleVector(int xComponent, int yComponent)
        {
            SetComponents(xComponent, yComponent);
        }

        public SimpleVector()
        {
            SetComponents(0, 0);
        }

        public Tuple<int, int> GetComponents()
        {
            return Tuple.Create(xComponent, yComponent);
        }

        public void SetComponents(int xComponent, int yComponent)
        {
            this.xComponent = xComponent;
            this.yComponent = yComponent;
        }

        public void RotateRight()
        {
            int tempComponent = -1 * xComponent;
            xComponent = yComponent;
            yComponent = tempComponent;
        }

        public void RotateRight(int numberOfRotations)
        {
            if (numberOfRotations < 0)
            {
                RotateLeft(-numberOfRotations);
            }
            else
            {
                for (int i = 0; i < numberOfRotations; i++)
                {
                    RotateRight();
                }
            }
        }

        public void RotateLeft()
        {
            int tempComponent = -1 * yComponent;
            yComponent = xComponent;
            xComponent = tempComponent;
        }

        public void RotateLeft(int numberOfRotations)
        {
            if (numberOfRotations < 0)
            {
                RotateRight(-numberOfRotations);
            }
            else
            {
                for (int i = 0; i < numberOfRotations; i++)
                {
                    RotateLeft();
                }
            }
        }

        public static SimpleVector Up()
        {
            return new SimpleVector(0, -1);
        }

        public static SimpleVector Down()
        {
            return new SimpleVector(0, 1);
        }

        public static SimpleVector Left()
        {
            return new SimpleVector(-1, 0);
        }

        public static SimpleVector Right()
        {
            return new SimpleVector(1, 0);
        }

        public static SimpleVector Zero()
        {
            return new SimpleVector(0, 0);
        }

        public SimpleVector Clone()
        {
            return new SimpleVector(xComponent, yComponent);
        }
    }
}