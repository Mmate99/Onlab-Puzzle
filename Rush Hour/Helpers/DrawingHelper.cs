using Rush_Hour.Models;
using Rush_Hour.Solver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rush_Hour.Helpers
{
    public class DrawingHelper
    {
        public void Draw(Game game)
        {
            Console.Clear();

            int h = 0;
            while (h < game.MapHeight)
            {
                string line = "";
                for (int w = 0; w < game.MapWidth; w++)
                {
                    line += game.Map[h * 10 + w].Code;
                }
                Console.WriteLine(line);
                h += 1;
            }
        }

        public void Draw(MapNode mapTree)
        {

            var map = mapTree.Map;
            int h = 0;
            Console.WriteLine($"Azonosító: {mapTree.Key}");
            Console.WriteLine($"Szülő azonosító: {mapTree.ParentKey}");
            Console.WriteLine($"Parancs: {mapTree.Command}");
            Console.WriteLine($"Mélység: {mapTree.Ply}");
            while (h < Math.Sqrt(map.Count))
            {
                string line = "";
                for (int w = 0; w < 8; w++)
                {
                    line += map[h * 10 + w].Code;
                }
                Console.WriteLine(line);
                h += 1;
            }
        }
    }
}
