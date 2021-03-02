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

        public MapTree(int key, int parentKey, Dictionary<int, MapObject> map, string command)
        {
            this.Key = key;
            this.ParentKey = parentKey;
            this.Map = map;
            this.Command = command;
        }

        public MapTree(Dictionary<int, MapObject> map)
        {
            // Ez csak növekvő és egyedi!
            this.Key = 0;
            // Ez mindig a szülőé
            this.ParentKey = 0;
            this.Map = map;
            this.Command = "";
        }
    }
}
