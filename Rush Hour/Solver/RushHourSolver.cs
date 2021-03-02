using Rush_Hour.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
            // TODO: Implement

            while (GameContinues() && IsSolvable())
            {

            }
        }

        private Dictionary<int, MapObject> GetNextMap(MapTree currentMap)
        {
            // TODO: Implement
            // Kikeresi a következő mapet a listából

            return mapTree.First(maps => maps.Key == (currentMap.Key + 1)).Map;
        }

        private void GetVehicles(Dictionary<int, MapObject> currentMap)
        {
            // Nem akarjuk, hogy a kocsik régi helyzete benn legyen
            vehicleList.Clear();

            var runUntilFalse = true;
            var increment = 97;

            while (runUntilFalse)
            {
                var vehicle = currentMap.Values.Where(v => v.Code == (char)increment)
                                                         .Cast<Vehicle>()
                                                         .FirstOrDefault();

                // Ha üres lesz a lista, akkor nagyon nagy gond van
                vehicleList.Add(vehicle);
                
                increment++;
                if (vehicle == null) runUntilFalse = false;
            }
        }

        private List<string> GetCommands(Dictionary<int, MapObject> currentMap)
        {
            // TODO: Implement
            // Megadja az összes parancsot, amiből elérhetjük a következő állapotokat
            // Ha üres listát dob vissza, akkor egyértelműen nem megoldható
            // Ez elég nehéz...

            var tempMap = currentMap;

            return null;
        }

        private int ContainsMap(Dictionary<int, MapObject> currentMap)
        {
            // TODO: Implement
            // Megmondja, hogy az új map létezik-e
            // Azért int, hogy állapotgéppel meg lehessen oldani
            // De jó a bool is
            // 1 - már létezik // 0 - nem létezik még
            return mapTree.Exists(maps => maps.Map.Equals(currentMap)) ? 1 : 0;
        }

        private void SetDeadEnd(MapTree currentMap)
        {
            // TODO: Implement
            // Kitörli a haszontalan mapet és a szüleit a listából
            // A szüleit azért, mert ha zsákutca, akkor zsákutca

            var tempMap = currentMap;

            while (!HasChildren(tempMap))
            {
                // Nem jó, mivel az eredetit nem módosítom...
                tempMap.DeadEnd = true;
                tempMap = mapTree.Find(maps => maps.Key == tempMap.ParentKey);
            }
        }

        private void StoreMap(Dictionary<int, MapObject> currentMap, Dictionary<int, MapObject> newMap, string cmd)
        {
            // TODO: Implement
            // Berakja a lista végére az új mapet

            var uniqueKey = mapTree.Last().Key + 1;
            var parentKey = mapTree.First(maps => maps.Map.Equals(currentMap)).Key;

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
            return !vehicleList.Any();
        }

        private bool HasChildren(MapTree map)
        {
            // Megnézi, hogy egy map-nek vannak-e gyerekei
            // és hogy azok zsákutcák-e
            var hasChildren = false;

            if(mapTree.Exists(maps => maps.ParentKey == map.Key))
            {
                var tempMaps = mapTree.FindAll(maps => maps.ParentKey == map.Key);
                tempMaps.ForEach(maps => { hasChildren = hasChildren & maps.DeadEnd; });
            }

            return hasChildren;
        }
    }
}
