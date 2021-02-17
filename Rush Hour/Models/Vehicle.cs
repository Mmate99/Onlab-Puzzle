using Rush_Hour.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rush_Hour.Models
{
    class Vehicle : MapObject
    {
        public int Length { get; set; }
        public int Orientation { get; set; }    // vertikális: 1, horizontális: 0
        public List<int> Positions { get; set; } = new List<int>();

        public Vehicle(char c) : base(c) { }

        public void MoveToDirection(DirectionEnum dir)
        {
            var newList = new List<int>();

            Positions.ForEach(p => newList.Add(p + (int)dir));
            Positions = newList;
        }

        public void CalcOrientation()
        {
            var orientation = (Math.Abs(Positions[0] - Positions[1]) == 1) ? 0 : 1;

            Orientation = orientation;
        }
    }
}
