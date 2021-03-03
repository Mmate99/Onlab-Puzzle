﻿using Rush_Hour.Enums;
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

        public Dictionary<int, MapObject> ExcecuteCommand(string cmd)
        {
            var command = cmd.Split(" ");
            var dir = enums[Char.Parse(command[1])];
            var m = Int32.Parse(command[2]);

            var vehicle = Map.Values.Where(v => v.Code == Char.Parse(command[0]))
                                                .Cast<Vehicle>()
                                                .FirstOrDefault();

            if (IsOrientationEquivalent(dir, vehicle))
            {
                for (int i = 0; i < m; i++)
                {
                    var minPos = vehicle.Positions.Min();
                    var maxPos = vehicle.Positions.Max();

                    if (IsNeighbourFree(dir, vehicle)) // minPos, maxPos))
                    {
                        vehicle.MoveToDirection(dir);
                        UpdateMap(dir, minPos, maxPos);
                    }
                }

                IsGameWon();
            }

            return Map;
        }

        private void IsGameWon()
        {
            var exitCode = Map.First(go => go.Value.Code == '0').Key;

            if (Map[exitCode - 1].Code == 'a')
                GameStillOn = false;
        }

        private void UpdateMap(DirectionEnum dir, int minPos, int maxPos)
        {
            var newPos = maxPos;
            var oldPos = minPos;
            if (dir == DirectionEnum.Up || dir == DirectionEnum.Left)
            {
                newPos = minPos;
                oldPos = maxPos;
            }

            Map[newPos + (int)dir] = Map[oldPos];
            Map[oldPos] = new EmptyPlace(' ');
        }

        private bool IsNeighbourFree(DirectionEnum dir, Vehicle vehicle) // int minPos, int maxPos)
        {
            var minPos = vehicle.Positions.Min();
            var maxPos = vehicle.Positions.Max();

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
