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
            var dir = Char.Parse(command[1]);
            var m = Int32.Parse(command[2]);

            var vehicle = Map.Values.Where(v => v.Code == Char.Parse(command[0]))
                                                .Cast<Vehicle>()
                                                .FirstOrDefault();

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

        private void UpdateMap(char dir, int minPos, int maxPos, Vehicle vehicle)
        {
            switch (dir)
            {
                case 'f':
                    Map[minPos - 10] = vehicle; //map[minp-10] = map[maxpos]; - lehet ez is jó lenne és akkor nem kéne átvenni a vehicle-t
                    Map[maxPos] = new EmptyPlace(' ');
                    break;

                case 'b':
                    Map[minPos - 1] = vehicle;
                    Map[maxPos] = new EmptyPlace(' ');
                    break;

                case 'l':
                    Map[maxPos + 10] = vehicle;
                    Map[minPos] = new EmptyPlace(' ');
                    break;

                case 'j':
                    Map[maxPos + 1] = vehicle;
                    Map[minPos] = new EmptyPlace(' ');
                    break;

                default:
                    throw new Exception("Valami nem jó!");
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
