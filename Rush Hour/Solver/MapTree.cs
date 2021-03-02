using System;
using System.Collections.Generic;
using System.Text;
using Rush_Hour.Models;

namespace Rush_Hour.Solver
{
    class MapTree
    {
        public int Key { get; set; }
        public int ParentKey { get; set; }
        public Dictionary<int, MapObject> Map { get; set; } = new Dictionary<int, MapObject>();
        public string Command { get; set; }
        public bool DeadEnd { get; set; }

        public MapTree(int key, int parentKey, Dictionary<int, MapObject> map, string command)
        {
            // Ez csak növekvő és egyedi!
            this.Key = key;
            // Ez mindig a szülőé
            this.ParentKey = parentKey;
            // Ez a tényleges map
            this.Map = map;
            // Ez az a command, amelyikkel az adott map-et létrehozzuk
            this.Command = command;
            // Amikor létrehozzuk, még nem tudjuk, hogy zsákutca-e
            this.DeadEnd = false;
        }

        public MapTree(Dictionary<int, MapObject> map)
        {
            this.Key = 0;
            // -1, mert akkor ez is unique
            this.ParentKey = -1;
            this.Map = map;
            this.Command = "";
            this.DeadEnd = false;
        }
    }
}
