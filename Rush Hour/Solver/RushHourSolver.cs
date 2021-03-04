using Rush_Hour.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Rush_Hour.Enums;
using Rush_Hour.Helpers;

namespace Rush_Hour.Solver
{
    /**************************************************************************
     * TODO:
     *
     * State:                           Next state:         Megvalósítva:
     * 1: Get next map from list        -> 2                Sztem igen
     * 2: Get vehicles from map         -> 3                Sztem igen
     * 3: Get possible commands
     *      if exists next command      -> 4                Nem
     *      if not                      -> 1                Nem
     * 4: Execute command               -> 5
     * 5: Check new map
     *      if map already exists       -> 6                Nem
     *      if not                      -> 7                Nem
     * 6: Delete map and parents        -> 3                Nem
     * 7: Store map in list
     *      if GameStillOn == false     -> FINISH           Nem, de talán easy :D
     *      if GameStillOn == true      -> 3                Nem
     *
     * We need to check the solvability somewhere           Meghívni!
     *
     * Megvalósítás egy állapotgép(state machine) formájában működhet?
     * Enumokkal, meg saját függvényekkel...
     *
     * 4 & 5 lehetnek u.abban az állapotban
     *
     * Az Execute Command csak annyi lenne, hogy game.ExecuteCommand(string)
     * Ehhez viszont az kell, hogy game.Map = ...
     * 
     * Lehet jobb lenne MapTree objektumokkal dolgozni, majd meglátjuk.
     ***************************************************************************/

    class RushHourSolver
    {
        public Game RushGame { get; set; }
        private List<MapTree> mapTree = new List<MapTree>();
        private List<Vehicle> vehicleList = new List<Vehicle>();

        public RushHourSolver(Game game)
        {
            this.RushGame = game;
            mapTree.Add(new MapTree(game.Map));
        }

        public void SolveGame()
        {
            DrawingHelper dh = new DrawingHelper();

            var currentMap = mapTree[0];
            GetVehicles(currentMap.Map);
            var commands = GetCommands(vehicleList);
            foreach(var cmd in commands)
            {
                var newMap = RushGame.ExcecuteCommand(currentMap, cmd);
                if (ContainsMap(newMap) == false)
                {
                    StoreMap(currentMap, newMap, cmd);
                }

            }

            while (GameContinues() && IsSolvable())
            {
                dh.Draw(currentMap.Map);
                currentMap = GetNextMap(currentMap);

                if (currentMap == null)
                {
                    var asd  = "asd";
                }

                GetVehicles(currentMap.Map);
                commands = GetCommands(vehicleList);
                bool hasValidChildren = false;

                foreach (var cmd in commands)
                {
                    var newMap = RushGame.ExcecuteCommand(currentMap, cmd);
                    if (ContainsMap(newMap) == false)
                    {
                        StoreMap(currentMap, newMap, cmd);
                        hasValidChildren = true;
                    }
                }

                if (!hasValidChildren)
                    SetDeadEnd(currentMap);
            }
        }

        private MapTree GetNextMap(MapTree currentMap)
        {
            // TODO: Implement
            // Kikeresi a következő mapet a listából

            return mapTree.FirstOrDefault(maps => maps.Key == (currentMap.Key + 1));
        }

        private void GetVehicles(Dictionary<int, MapObject> currentMap)
        {
            // Lokális vált.
            // Nem akarjuk, hogy a kocsik régi helyzete benn legyen
            vehicleList.Clear();

            var runUntilFalse = true;
            var increment = 97;

            while (true) // runUntilFalse)
            {
                // teszt mert nem biztos, h kell a "?"
                var vehicle = currentMap.Values.Where(v => v.Code == (char)increment)?
                                                         .Cast<Vehicle>()
                                                         .FirstOrDefault();

                if (vehicle == null) break; // runUntilFalse = false;

                // Ha üres lesz a lista, akkor nagyon nagy gond van
                vehicleList.Add(vehicle);
                
                increment++;
            }
        }

        private List<string> GetCommands(List<Vehicle> vehicles)
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
            var maxMoves = Math.Sqrt(mapTree[0].Map.Count()) - 4;
            
            foreach (Vehicle vehicle in vehicles)
            {
                var directions = (vehicle.Orientation == 0) ? new[] { "b", "j" } : new[] { "f", "l" };

                foreach (string dir in directions)
                {
                    for (int i = 1; i <= maxMoves; i++)
                    {
                        string singleCmd = Char.ToString(vehicle.Code) + " " + dir + " " + $"{ i }"; // a j 1
                        cmds.Add(singleCmd);
                    }
                }
            }

            return cmds;
        }

        private bool ContainsMap(Dictionary<int, MapObject> currentMap)
        {
            // TODO: Implement
            // Megmondja, hogy az új map létezik-e
            // Azért int, hogy állapotgéppel meg lehessen oldani
            // De jó a bool is
            // 1 - már létezik // 0 - nem létezik még

            bool matchingExists = false;

            foreach (var node in mapTree)
            {
                bool changeMatchingValuable = true;
                foreach(var mapTile in currentMap)
                {
                    var key = mapTile.Key;
                    if (mapTile.Value.GetType() != node.Map.GetValueOrDefault(key).GetType())
                        changeMatchingValuable = false;
                }

                if (changeMatchingValuable)
                    matchingExists = true;
            }

            return matchingExists;
        }

        private void SetDeadEnd(MapTree currentMap)
        {
            // TODO: Implement
            // Kitörli a haszontalan mapet és a szüleit a listából
            // A szüleit azért, mert ha zsákutca, akkor zsákutca

            currentMap.DeadEnd = true;
            var parent = currentMap;

            while (!HasChildren(parent))
            {
                parent = mapTree.Find(maps => maps.Key == parent.ParentKey);
                if (parent != null)
                    parent.DeadEnd = true;
                if (parent is null)
                    break;
            }
        }

        private void StoreMap(MapTree currentMap, Dictionary<int, MapObject> newMap, string cmd)
        {
            // TODO: Implement
            // Berakja a lista végére az új mapet

            var uniqueKey = mapTree.Last().Key + 1;
            var parentKey = currentMap.Key;

            mapTree.Add(new MapTree(uniqueKey, parentKey, newMap, cmd));
        }

        private bool GameContinues()
        {
            // TODO: Implement
            // Megmondja, hogy vége van-e vagy sem
            return true;
        }

        private bool IsSolvable()
        {
            // Megomndja, hogy megoldható-e vagy sem
            // Kb úgy kellene, hogy ha a mapek listája üres lesz,
            // akkor nem megoldható a puzzle

            // NEM JÓ
            return mapTree.Any();
        }

        private bool HasChildren(MapTree currentMap)
        {
            // Megnézi, hogy egy map-nek vannak-e gyerekei
            // és hogy azok zsákutcák-e
            var hasChildren = false;

            var childrenasdgf = mapTree.Where(map => map.ParentKey == currentMap.Key);

            if(mapTree.Exists(map => map.ParentKey == currentMap.Key))
            {
                var children = mapTree.FindAll(map => map.ParentKey == currentMap.Key);
                children.ForEach(child => { hasChildren = hasChildren | !child.DeadEnd; });
            }

            return hasChildren;
        }
    }
}
