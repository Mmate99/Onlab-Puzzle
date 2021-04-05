using Rush_Hour.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rush_Hour.Models
{
    public class Game
    {
        public bool GameStillOn { get; private set; } = true;
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public Dictionary<int, MapObject> Map { get; set; } = new Dictionary<int, MapObject>();

        private Dictionary<char, DirectionEnum> enums = 
            new Dictionary<char, DirectionEnum> { { 'f', DirectionEnum.Up },
                                                  { 'l', DirectionEnum.Down },
                                                  { 'j', DirectionEnum.Right },
                                                  { 'b', DirectionEnum.Left }};

        public Dictionary<int, MapObject> ExcecuteCommand(MapNode mapTree, string cmd)
        {
            var mapTreeClone = mapTree.Clone();
            var ret = mapTreeClone.Map;

            int tempV = 0;

            var command = cmd.Split(" ");
            var dir = enums[Char.Parse(command[1])];
            var m = Int32.Parse(command[2]);

            var vehicle = ret.Values.Where(v => v.Code == Char.Parse(command[0]))
                                                .Cast<Vehicle>()
                                                .FirstOrDefault();

            if (IsOrientationEquivalent(dir, vehicle))
            {
                for (int i = 0; i < m; i++)
                {
                    var minPos = vehicle.Positions.Min();
                    var maxPos = vehicle.Positions.Max();

                    if (IsNeighbourFree(ret, dir, minPos, maxPos))
                    {
                        vehicle.MoveToDirection(dir);
                        UpdateMap(ret, dir, minPos, maxPos);
                        tempV++;
                    }
                }

                IsGameWon();
            }

            return ret;
        }

        private void IsGameWon()
        {
            var exitCode = Map.First(go => go.Value.Code == '0').Key;

            if (Map[exitCode - 1].Code == 'a')
                GameStillOn = false;
        }

        private void UpdateMap(Dictionary<int, MapObject> map,  DirectionEnum dir, int minPos, int maxPos)
        {
            var newPos = maxPos;
            var oldPos = minPos;
            if (dir == DirectionEnum.Up || dir == DirectionEnum.Left)
            {
                newPos = minPos;
                oldPos = maxPos;
            }

            var tempMap = new Dictionary<int, MapObject>();
            foreach (var m in map)
            {
                tempMap.Add(m.Key, m.Value);
            }

            map[newPos + (int)dir] = map[oldPos];
            map[oldPos] = new EmptyPlace(' ');
        }

        private bool IsNeighbourFree(Dictionary<int, MapObject> map, DirectionEnum dir, int minPos, int maxPos)
        {
            var position = (dir == DirectionEnum.Up || dir == DirectionEnum.Left) ? minPos : maxPos;

            return map[position + (int)dir].Code == ' ';
        }

        private bool IsOrientationEquivalent(DirectionEnum dir, Vehicle vehicle)
        {
            var orientation = (dir == DirectionEnum.Up || dir == DirectionEnum.Down) ? 1 : 0;

            return orientation == vehicle.Orientation;
        }
    }
}
