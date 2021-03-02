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
     *      if GameStillOn == false     -> FINISH Nem, de talán easy :D
     *      if GameStillOn == true      -> 3                Nem
     *
     * We need to check the solvability somewhere           Nem
     *
     * Megvalósítás egy állapotgép(state machine) formájában működhet?
     * Enumokkal, meg saját függvényekkel...
     *
     * 4 & 5 lehetnek u.abban az állapotban
     *
     * Az Execute Command csak annyi lenne, hogy game.ExecuteCommand(string)
     * Ehhez viszont az kell, hogy game.Map = ...
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
        }

        private Dictionary<int, MapObject> GetNextMap(MapTree currentMap)
        {
            // TODO: Implement
            // Kikeresi a következő mapet a listából
            // Nem olyan egyszerű, mert folyamatosan változik a lista hossza

            var parentKey = currentMap.ParentKey;
            var nextMap = mapTree.Where(map => map.ParentKey == parentKey)
                                                        .FirstOrDefault();

            return nextMap.Map;
        }

        private void GetVehicles(Dictionary<int, MapObject> currentMap)
        {
            vehicleList.Clear();    // Nem akarjuk, hogy a kocsik régi helyzete benn legyen

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

        private string[] CommandList()
        {
            // TODO: Implement
            // Megadja az összes parancsot, amiből elérhetjük a következő állapotokat
            // Ha üres listát dob vissza, akkor egyértelműen nem megoldható
            // Ez elég nehéz...
            return null;
        }

        private int CompareMaps()
        {
            // TODO: Implement
            // Megmondja, hogy az új map létezik-e
            // Azért int, hogy állapotgéppel meg lehessen oldani
            // De jó a bool is
            return 0;
        }

        private void DeleteFromTree()
        {
            // TODO: Implement
            // Kitörli a haszontalan mapet és a szüleit a listából
            // A szüleit azért, mert ha zsákutca, akkor zsákutca
        }

        private void StoreMap()
        {
            // TODO: Implement
            // Berakja a lista végére az új mapet
        }

        private bool GameContinues()
        {
            // TODO: Implement
            // Megmondja, hogy vége van-e vagy sem
            return true;
        }

        private bool IsSolvable()
        {
            // TODO: Implement
            // Megomndja, hogy megoldható-e vagy sem
            // Kb úgy kellene, hogy ha a mapek listája üres lesz,
            // akkor nem megoldható a puzzle
            return true;
        }
    }
}
