using System;
using System.Collections.Generic;
using System.Text;

namespace Rush_Hour.Helpers
{
    class RandomMapGenerator
    {
        public string[] GenerateMap(int CarNumber)
        {
            var r = new Random();

            bool isCarHorizontal = (r.NextDouble() - 0.5) <= 0;
            bool isCarSizeTwo = (r.NextDouble() - 0.7) <= 0;
            int tileNumber = r.Next()

            return null;
        }
    }
}
