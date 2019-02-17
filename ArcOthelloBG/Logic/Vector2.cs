using System;

namespace ArcOthelloBG.Logic
{
    /// <summary>
    /// class for a vector 2 of short
    /// </summary>
    [Serializable]
    class Vector2 : ICloneable
    {
        private int x;
        private int y;

        /// <summary>
        /// getter for X value
        /// </summary>
        public int X
        {
            get
            {
                return this.x;
            }
        }
        /// <summary>
        /// getter for y value
        /// </summary>
        public int Y
        {
            get
            {
                return this.y;
            }
        }

        /// <summary>
        /// constructor by value
        /// </summary>
        /// <param name="x">x value</param>
        /// <param name="y">y value</param>
        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// constructor by copy
        /// </summary>
        /// <param name="toCopy">vector to Copy</param>
        public Vector2(Vector2 toCopy) : this(toCopy.x, toCopy.y)
        { }

        /// <summary>
        /// constructor with a tuple of int
        /// </summary>
        /// <param name="toCopy">tuple of int</param>
        public Vector2(Tuple<int,int> toCopy) : this(toCopy.Item1, toCopy.Item2)
        { }

        /// <summary>
        /// add a vector to another
        /// </summary>
        /// <param name="vToAdd">vector to add to the vector</param>
        /// <returns>result of the sum</returns>
        public Vector2 add(Vector2 vToAdd)
        {
            return new Vector2(this.x + vToAdd.x, this.y + vToAdd.y);
        }

        /// <summary>
        /// clone of a vector
        /// </summary>
        /// <returns>cloned vector</returns>
        public object Clone()
        {
            return new Vector2(this.x, this.y);
        }

        /// <summary>
        /// a vector is equals when x and y are the same
        /// </summary>
        /// <param name="obj">obj to compare</param>
        /// <returns>equals or not</returns>
        public override bool Equals(object obj)
        {
            Vector2 toCompare = null;
            try
            {
                toCompare = obj as Vector2;
            }
            catch(Exception)
            {
                return false;
            }
           

            return toCompare.x == this.x && toCompare.y == this.y;
        }

        /// <summary>
        /// hashcode of a vector, add the x ans y in a string and get the hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return $"{this.x}{this.y}".GetHashCode();
        }

        /// <summary>
        /// Transform a vector2 to a Tuples of int int
        /// </summary>
        /// <returns></returns>
        public Tuple<int, int> toTuplesintint()
        {
            return new Tuple<int, int>(this.x, this.y);
        }

        public override string ToString()
        {
            return $"x : {this.x} y : {this.y}";
        }
    }
}
