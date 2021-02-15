using System;
using System.Collections.Generic;
using System.Linq;

namespace Rush_Hour.Models
{
    public class Game
    {
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public Dictionary<int, MapObject> Map { get; set; } = new Dictionary<int, MapObject>();

        public void ExcecuteCommand(string cmd)
        {
            var command = cmd.Split(" ");
            var dir = command[1][0];

            List<Vehicle> vehicles = Map.Values.Where(v => v.Code == command[0][0])
                                                .Cast<Vehicle>()
                                                .ToList();

            var minPos = vehicles[0].Positions.Min();
            var maxPos = vehicles[0].Positions.Max();
            var currentPositions = vehicles[0].Positions;

            if (IsNeighbourFree(dir, minPos, maxPos))
            {
                foreach (var v in vehicles)
                {
                    v.MoveToDirection(dir);
                }
            }
        }

        private bool IsNeighbourFree(char dir, int minPos, int maxPos)
        {
            switch (dir)
            {
                case 'f':
                    return Map[minPos - 10].Code == ' ';

                case 'b':
                    return Map[minPos - 1].Code == ' ';

                case 'l':
                    return Map[maxPos + 10].Code == ' ';

                case 'j':
                    return Map[maxPos + 1].Code == ' ';

                default:
                    throw new Exception("Valami nem jó!");
            }
        }
    }
}
