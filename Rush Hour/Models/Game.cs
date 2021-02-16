using Rush_Hour.Enums;
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

        private Dictionary<char, DirectionEnum> enums = 
            new Dictionary<char, DirectionEnum> { { 'f', DirectionEnum.Up },
                                                  { 'l', DirectionEnum.Down },
                                                  { 'j', DirectionEnum.Right },
                                                  { 'b', DirectionEnum.Left }};

        public void ExcecuteCommand(string cmd)
        {
            var command = cmd.Split(" ");
            var dir = enums[Char.Parse(command[1])];
            var m = Int32.Parse(command[2]);

            var vehicle = Map.Values.Where(v => v.Code == Char.Parse(command[0]))
                                                .Cast<Vehicle>()
                                                .FirstOrDefault();

            //if (IsOrientationEquivalent(dir, vehicle))
            for (int i = 0; i < m; i++)
            {
                var minPos = vehicle.Positions.Min();
                var maxPos = vehicle.Positions.Max();

                if (IsNeighbourFree(dir, minPos, maxPos))
                {
                    vehicle.MoveToDirection(dir);
                    UpdateMap(dir, minPos, maxPos, vehicle);
                }
            }
        }

        private void UpdateMap(DirectionEnum dir, int minPos, int maxPos, Vehicle vehicle)
        {
            var newPos = maxPos;
            var oldPos = minPos;
            if (dir == DirectionEnum.Up || dir == DirectionEnum.Left)
            {
                newPos = minPos;
                oldPos = maxPos;
            }

            Map[newPos + (int)dir] = vehicle;   //map[minp-10] = map[maxpos]; - lehet ez is jó lenne és akkor nem kéne átvenni a vehicle-t
            Map[oldPos] = new EmptyPlace(' ');
        }

        private bool IsNeighbourFree(DirectionEnum dir, int minPos, int maxPos)
        {
            var position = (dir == DirectionEnum.Up || dir == DirectionEnum.Left) ? minPos : maxPos;

            return Map[position + (int)dir].Code == ' ';
        }

        private bool IsOrientationEquivalent(DirectionEnum dir, Vehicle vehicle)
        {
            var orientation = (dir == DirectionEnum.Up || dir == DirectionEnum.Down) ? 1 : 0;

            return orientation == vehicle.Orientation;
        }
    }
}
