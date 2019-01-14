using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloBG.Logic
{
    /// <summary>
    /// class for a vector 2 of short
    /// </summary>
    class Vector2 : ICloneable
    {
        private short x;
        private short y;

        /// <summary>
        /// getter for X value
        /// </summary>
        public short X
        {
            get
            {
                return this.x;
            }
        }
        /// <summary>
        /// getter for y value
        /// </summary>
        public short Y
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
        public Vector2(short x, short y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// constructor by value
        /// </summary>
        /// <param name="x">x value</param>
        /// <param name="y">y value</param>
        public Vector2(int x, int y)
        {
            this.x = (short)x;
            this.y = (short)y;
        }

        /// <summary>
        /// constructor by copy
        /// </summary>
        /// <param name="toCopy">vector to Copy</param>
        public Vector2(Vector2 toCopy) : this(toCopy.x, toCopy.y)
        { }

        /// <summary>
        /// add a vector to another
        /// </summary>
        /// <param name="vToAdd">vector to add to the vector</param>
        /// <returns>result of the sum</returns>
        public Vector2 add(Vector2 vToAdd)
        {
            return new Vector2((short)(this.x + vToAdd.x), (short)(this.y + vToAdd.y));
        }

        /// <summary>
        /// clone of a vector
        /// </summary>
        /// <returns>cloned vector</returns>
        public object Clone()
        {
            return new Vector2(this.x, this.y);
        }

        public Tuple<short, short> toTupleshortshort()
        {
            return new Tuple<short, short>(this.x, this.y);
        }
    }
}
