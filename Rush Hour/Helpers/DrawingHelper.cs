using Rush_Hour.Models;
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
    }
}
