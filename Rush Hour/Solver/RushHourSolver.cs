using Rush_Hour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Rush_Hour.Enums;
using Rush_Hour.Helpers;

namespace Rush_Hour.Solver
{
    class RushHourSolver
    {
        public Game RushGame { get; set; }
        private List<MapNode> mapTree = new List<MapNode>();
        private List<Vehicle> currentVehicleList = new List<Vehicle>();
        private DrawingHelper dh = new DrawingHelper();

        private Dictionary<char, DirectionEnum> enums =
            new Dictionary<char, DirectionEnum> { { 'f', DirectionEnum.Up },
                                                  { 'l', DirectionEnum.Down },
                                                  { 'j', DirectionEnum.Right },
                                                  { 'b', DirectionEnum.Left }};

        public RushHourSolver(Game game)
        {
            this.RushGame = game;
            mapTree.Add(new MapNode(game.Map));
        }

        public void SolveGame()
        {
            var currentMap = mapTree[0];
            GetVehicles(currentMap.Map);
            var commands = GetCommands(currentMap.Map, currentVehicleList);
            foreach(var cmd in commands)
            {
                var newMap = RushGame.ExcecuteCommand(currentMap, cmd);
                if (ContainsMap(newMap) == false)
                {
                    StoreMap(currentMap, newMap, cmd);
                }

            }

            while (true)
            {
                dh.Draw(currentMap);
                currentMap = GetNextMap(currentMap);

                if (currentMap == null)
                {
                    var asd  = "asd";
                }

                GetVehicles(currentMap.Map);
                commands = GetCommands(currentMap.Map, currentVehicleList);

                foreach (var cmd in commands)
                {
                    var newMap = RushGame.ExcecuteCommand(currentMap, cmd);
                    if (ContainsMap(newMap) == false)
                    {
                        StoreMap(currentMap, newMap, cmd);
                        if (IsGameWon(newMap))
                            End();
                    }
                }
            }
        }

        private void End()
        {
            Console.WriteLine("-------------------------------------------------------------------------------------------------");
            var currMap = mapTree.Last();
            MapNode parent = null;
            var moves = new List<MapNode>();
            while (true)
            {
                var parentKey = currMap.ParentKey;
                parent = mapTree.FirstOrDefault(mt => mt.Key == parentKey);
                moves.Add(currMap);
                currMap = parent;

                if (currMap.ParentKey == -1)
                {
                    moves.Add(currMap);
                    moves.Reverse();

                    foreach (var map in moves)
                    {
                        dh.Draw(map);
                    }
                }
            }
        }

        // Kikeresi a következő mapet a listából
        private MapNode GetNextMap(MapNode currentMap)
        {
            return mapTree.FirstOrDefault(maps => maps.Key == (currentMap.Key + 1));
        }

        private void GetVehicles(Dictionary<int, MapObject> currentMap)
        {
            // Lokális vált.
            // Nem akarjuk, hogy a kocsik régi helyzete benn legyen
            currentVehicleList.Clear();
            var increment = 97;

            while (true)
            {
                // teszt mert nem biztos, h kell a "?"
                var vehicle = currentMap.Values.Where(v => v.Code == (char)increment)?
                                                         .Cast<Vehicle>()
                                                         .FirstOrDefault();

                if (vehicle == null) break;

                // Ha üres lesz a lista, akkor nagyon nagy gond van
                currentVehicleList.Add(vehicle);
                
                increment++;
            }
        }

        private List<string> GetCommands(Dictionary<int, MapObject> currentMap, List<Vehicle> vehicles)
        {
            // TODO: Implement
            // Megadja az összes parancsot, amiből elérhetjük a következő állapotokat
            // Ha üres listát dob vissza, akkor egyértelműen nem megoldható
            // Ez elég nehéz...
            // Lehet azt is csinálni, hogy egy globális változóba dobjuk be a cmd-ket,
            // mint a kocsik esetében. Vagy azokat rakjuk át lokálisba...

            List<string> cmds = new List<string>();

            // A matekja: n*n-es pálya, egy sorban vagy ozslopban 2 fal van,
            // és a legkisebb kocsi az 2-es. Azaz a 6 szabad helyen max 4-et mozoghat
            var maxMoves = Math.Sqrt(mapTree[0].Map.Count()) - 4;   //ha a pálya 6*6-os akkor már ez egyből rossz
            
            foreach (Vehicle vehicle in vehicles)
            {
                var directions = (vehicle.Orientation == 0) ? new[] { "b", "j" } : new[] { "f", "l" };

                foreach (string dir in directions)
                {
                    for (int i = 1; i <= maxMoves; i++)
                    {
                        if (IsWayClear(currentMap, vehicle, dir, i))
                        {
                            string singleCmd = Char.ToString(vehicle.Code) + " " + dir + " " + $"{ i }";
                            cmds.Add(singleCmd);
                        }
                    }
                }
            }

            return cmds;
        }

        private bool IsWayClear(Dictionary<int, MapObject> currentMap, Vehicle vehicle, string direction, int numOfSteps)
        {
            var dir = enums[Char.Parse(direction)];
            var position = (dir == DirectionEnum.Up || dir == DirectionEnum.Left) ? vehicle.Positions.Min() : vehicle.Positions.Max();

            for (int i = 1; i <= numOfSteps; i++)
            {
                var nextPosIndex = position + i*(int)dir;
                if (currentMap[nextPosIndex].Code != ' ')
                    return false;
            }

            return true;
        }

        private bool ContainsMap(Dictionary<int, MapObject> currentMap)
        {
            // TODO: Implement
            // Megmondja, hogy az új map létezik-e
            // Azért int, hogy állapotgéppel meg lehessen oldani
            // De jó a bool is
            // 1 - már létezik // 0 - nem létezik még

            var currentMapString = MapToStringConverter(currentMap);
            bool matchingExists = false;

            foreach (var node in mapTree)
            {
                bool changeMatchingValuable = true;

                var nodeString = node.MapString;
                if (!currentMapString.Equals(nodeString))
                {
                    changeMatchingValuable = false;
                }

                if (changeMatchingValuable)
                    matchingExists = true;
            }

            return matchingExists;
        }

        private void StoreMap(MapNode currentMap, Dictionary<int, MapObject> newMap, string cmd)
        {
            // TODO: Implement
            // Berakja a lista végére az új mapet

            var uniqueKey = mapTree.Last().Key + 1;
            var parentKey = currentMap.Key;
            var ply = currentMap.Ply + 1;
            var mapString = MapToStringConverter(newMap);

            mapTree.Add(new MapNode(uniqueKey, parentKey, newMap, cmd, ply, mapString));
        }

        private bool IsGameWon(Dictionary<int, MapObject> map)
        {
            var pos = map.First(go => go.Value.Code == '0').Key;

            while (true)
            {
                if (map[pos - 1].Code == 'a')
                    return true;
                else if (map[pos - 1].Code != ' ')
                    return false;

                pos--;
            }
        }

        private string MapToStringConverter(Dictionary<int, MapObject> map)
        {
            var ret = "";

            foreach(var tile in map)
            {
                ret += tile.Value.Code;
            }

            return ret;
        }
    }
}
