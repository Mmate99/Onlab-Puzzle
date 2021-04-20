using Rush_Hour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Rush_Hour.Enums;
using Rush_Hour.Helpers;

namespace Rush_Hour.Manager
{
    class GameManager
    {
        public Game RushGame { get; set; }
        private List<MapNode> mapTree = new List<MapNode>();
        private List<Vehicle> currentVehicleList = new List<Vehicle>();
        private DrawingHelper dh = new DrawingHelper();
        private bool isNotSolved = true;

        private Dictionary<char, DirectionEnum> enums =
            new Dictionary<char, DirectionEnum> { { 'f', DirectionEnum.Up },
                                                  { 'l', DirectionEnum.Down },
                                                  { 'j', DirectionEnum.Right },
                                                  { 'b', DirectionEnum.Left }};

        public GameManager(Game game)
        {
            RushGame = game;
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

            while (isNotSolved)
            {
                dh.Draw(currentMap);
                currentMap = GetNextMap(currentMap);

                if (currentMap == null)
                {
                    Console.WriteLine("All states found!");
                    break;
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

        public string[] MakeGame(int requiredSteps)
        {
            var currentMap = mapTree[0];
            string mapString;

            if (requiredSteps == 1)
            {
                Console.WriteLine("Number of steps cannot be 1!");
                // Konvertáljuk a mapTree[0]-t egy string tömbbé,
                // és azt adjuk vissza.
                mapString = MapToStringConverter(currentMap.Map);
                
                return MapStringToArray(mapString);
            }

            var commands = new List<string>();
            var currentPly = 0;

            while (true)
            {
                GetVehicles(currentMap.Map);
                commands = GetCommands(currentMap.Map, currentVehicleList);
                // Itt van az első lépés
                foreach (var cmd in commands)
                {
                    var newMap = RushGame.ExcecuteCommand(currentMap, cmd);
                    if (ContainsMap(newMap) == false)
                    {
                        StoreMap(currentMap, newMap, cmd);
                    }
                }

                if (GetNextMap(currentMap) == null)
                    return null;

                var nextply = GetNextMap(currentMap).Ply;
                if (currentPly != nextply)
                {
                    if (PlyUnsolved(currentPly))
                    {
                        currentPly++;
                        requiredSteps += currentPly;
                        break;
                    }
                    currentPly++;
                }

                currentMap = GetNextMap(currentMap);
            }
            
            while (currentMap.Ply <= requiredSteps)
            {
                dh.Draw(currentMap);
                currentMap = GetNextMap(currentMap);

                if (currentMap == null)
                {
                    //requiredSteps = currentPly;
                    Console.WriteLine($"Cannot reach the amount of required steps, only {currentPly - requiredSteps}");
                    break;
                }

                if (currentPly != currentMap.Ply)
                    currentPly = currentMap.Ply;

                GetVehicles(currentMap.Map);
                commands = GetCommands(currentMap.Map, currentVehicleList);

                foreach (var cmd in commands)
                {
                    var newMap = RushGame.ExcecuteCommand(currentMap, cmd);
                    if (ContainsMap(newMap) == false)
                    {
                        StoreMap(currentMap, newMap, cmd);
                    }
                }
            }

            //TODO: Ha nem tudunk eljutni a reqsteps-ig akkor megbaszódik

            // Ki kell választani egy map-et azok közül, ahol a requiredSteps == currentMap.Ply teljesül
            var chosenMap = getRandomMapWithRequiredSteps(requiredSteps);
            mapString = MapToStringConverter(chosenMap);

            return MapStringToArray(mapString);
        }

        private bool PlyUnsolved(int ply)
        {
            var mapsToCheck = mapTree.Where(mn => mn.Ply == ply)
                .ToList();

            foreach(var mapNode in mapsToCheck)
            {
                if (IsGameWon(mapNode.Map))
                    return false;
            }
            return true;
        }

        private void End()
        {
            Console.WriteLine("-------------------------------------------------------------------------------------------------");
            var currMap = mapTree.Last();
            MapNode parent = null;
            var moves = new List<MapNode>();
            var drawSolution = true;
            while (drawSolution)
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

                    drawSolution = false;
                    isNotSolved = false;
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

        private Dictionary<int, MapObject> getRandomMapWithRequiredSteps(int requiredSteps)
        {
            IEnumerable<MapNode> nodes = mapTree.Where(node => node.Ply == requiredSteps);
            Random random = new Random();
            var chosenMap = random.Next(0, nodes.Count() + 1);

            return nodes.ElementAt(chosenMap).Map;
        }

        private string[] MapStringToArray(string map)
        {
            string[] mapArray = new string[8];

            for (int i = 0; i < mapArray.Length; i++)
            {
                mapArray[i] = map.Substring(i * 8, 8);
            }

            return mapArray;
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
