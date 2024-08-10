using System;
using System.Collections.Generic;

namespace Coordinate
{
    // Small custom type for storing integer room coordinates
    [Serializable]
    public struct Coord
    {
        public int x, y;

        // Four cardinal direction in a list
        // Useful for dungeon generation, static so it only gets made once
        public static Queue<Coord> cardinals = new Queue<Coord>();

        static Coord()
        {
            cardinals.Enqueue(new Coord(0, 1));
            cardinals.Enqueue(new Coord(0, -1));
            cardinals.Enqueue(new Coord(-1, 0));
            cardinals.Enqueue(new Coord(1, 0));
        }

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static Coord operator +(Coord a, Coord b)
        {
            return new Coord(
            a.x + b.x,
            a.y + b.y);
        }
        public static Coord operator -(Coord a, Coord b)
        {
            return new Coord(
                a.x - b.x,
                a.y - b.y);
        }
        public static bool operator ==(Coord a, Coord b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Coord a, Coord b)
        {
            return !(a == b);
        }


        // Visual studio said these were proper to have
        public override bool Equals(object obj)
        {
            return obj is Coord coord &&
                   x == coord.x &&
                   y == coord.y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}

