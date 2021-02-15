using Rush_Hour.Helpers;
using Rush_Hour.Models;
using System;
using System.IO;
using System.Linq;

namespace Rush_Hour
{
    class Program
    {
        public static Game game = new Game();
        static void Main(string[] args)
        {
            DrawingHelper dh = new DrawingHelper();

            var lines = File.ReadAllLines("map.txt");
            game.MapWidth = lines[0].Length;
            game.MapHeight = lines.Length;

            InitializeMap(lines);
            dh.Draw(game);

            while (true)
            {
                var command = Console.ReadLine();
                game.ExcecuteCommand(command);
                dh.Draw(game);
            }
        }

        private static void InitializeMap(string[] lines)
        {
            for (int j = 0; j < game.MapHeight; j++)
            {
                var line = lines[j];
                for (int i = 0; i < game.MapWidth; i++)
                {
                    var pos = i + j * 10;
                    switch (line[i])
                    {
                        case '#':
                            game.Map.Add(pos, new Wall('#'));
                            break;

                        case ' ':
                            game.Map.Add(pos, new EmptyPlace(' '));
                            break;

                        case '0':
                            game.Map.Add(pos, new Exit('0'));
                            break;

                        default:
                            var vehicle = (Vehicle)game.Map.Values
                                          .Where(v => v.Code == line[i])
                                          .FirstOrDefault();

                            if (vehicle != null)
                            {
                                game.Map.Add(pos, vehicle);
                                vehicle.Length++;
                                vehicle.Positions.Add(pos);
                                break;
                            }

                            vehicle = new Vehicle(line[i])
                            {
                                Code = line[i],
                                Length = 1,
                                Positions = { pos }
                            };
                            game.Map.Add(pos, vehicle);
                            break;
                    }
                }
            }
        }
    }
}
