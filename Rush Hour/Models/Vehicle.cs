using System;
using System.Collections.Generic;
using System.Text;

namespace Rush_Hour.Models
{
    class Vehicle : MapObject
    {
        public int Length { get; set; }
        public int Orientation { get; set; }
        public List<int> Positions { get; set; } = new List<int>();

        public Vehicle(char c) : base(c) { }

        public void MoveToDirection(char dir)
        {
            var newList = new List<int>();
            switch (dir)
            {
                case 'f':
                    Positions.ForEach(p => newList.Add(p - 10));
                    Positions = newList;
                    break;

                case 'b':
                    Positions.ForEach(p => newList.Add(p - 1));
                    Positions = newList;
                    break;

                case 'l':
                    Positions.ForEach(p => newList.Add(p + 10));
                    Positions = newList;
                    break;

                case 'j':
                    Positions.ForEach(p => newList.Add(p + 1));
                    Positions = newList;
                    break;

                default:
                    throw new Exception("Valami nem jó!");
            }
        }
    }
}
