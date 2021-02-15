using System;
using System.Collections.Generic;
using System.Text;

namespace Rush_Hour.Models
{
    public class MapObject
    {
        public char Code { get; set; }

        public MapObject(char c)
        {
            Code = c;
        }
    }
}
