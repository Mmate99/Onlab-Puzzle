using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rush_Hour.Models;

namespace Rush_Hour.Models
{
    public class MapNode
    {
        public int Key { get; set; }
        public int ParentKey { get; set; }
        public Dictionary<int, MapObject> Map { get; set; } = new Dictionary<int, MapObject>();
        public string Command { get; set; }
        public int DistanceFromLastSolved { get; set; }
        public int Ply { get; set; }
        public string MapString { get; set; }

        public MapNode(int key, int parentKey, string command, int ply, string mapString) 
        {
            //tuti nem ref???
            Key = key;
            ParentKey = parentKey;
            Command = command;
            Ply = ply;
            MapString = mapString;
        }

        public MapNode(int key, int parentKey, Dictionary<int, MapObject> map, string command, int ply, string mapString, int distFromLastSolved)
        {
            foreach (var m in map)
            {
                Map.Add(m.Key, m.Value);
            }

            // Ez csak növekvő és egyedi!
            Key = key;
            // Ez mindig a szülőé
            ParentKey = parentKey;
            // Ez a tényleges map
            //this.Map = map;
            // Ez az a command, amelyikkel az adott map-et létrehozzuk
            Command = command;
            Ply = ply;
            MapString = mapString;
            DistanceFromLastSolved = distFromLastSolved;
        }

        public MapNode(Dictionary<int, MapObject> map)
        {
            foreach (var m in map)
            {
                Map.Add(m.Key, m.Value);
            }

            this.Key = 0;
            // -1, mert akkor ez is unique
            this.ParentKey = -1;
            // this.Map = map;
            this.Command = "";
            Ply = 0;
        }

        public MapNode Clone()
        {
            var clone = new MapNode(Key, ParentKey, Command, Ply, MapString);
            foreach (var mapTile in Map)
            {
                var key = mapTile.Key;
                var value = mapTile.Value;
                

                switch (value.Code)
                {
                    case '#':
                        clone.Map.Add(key, new Wall(value.Code));
                        break;
                    case ' ':
                        clone.Map.Add(key, new EmptyPlace(value.Code));
                        break;
                    case '0':
                        clone.Map.Add(key, new Exit(value.Code));
                        break;
                    default:
                        var vehicle = (Vehicle)clone.Map.Values
                                          .FirstOrDefault(v => v.Code == value.Code);
                        if (vehicle != null)
                        {
                            clone.Map.Add(key, vehicle);
                            break;
                        }

                        var tempVehicle = (Vehicle)value;
                        clone.Map.Add(key, new Vehicle(tempVehicle.Code)
                        {
                            Length = tempVehicle.Length,
                            Orientation = tempVehicle.Orientation,
                            Positions = tempVehicle.Positions
                        });
                        break;
                }
            }

            return clone;
        }
    }
}
