using Rush_Hour.Helpers;
using Rush_Hour.Models;
using Rush_Hour.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using Rush_Hour.Database;
using System.IO;

namespace Rush_Hour
{
    class Program
    {
        public static Game game = new Game();
        static void Main(string[] args)
        {
            Console.WriteLine("Milyen módban szeretné elindítani az alkalmazást? Solver (S) vagy Generator (G) vagy kézi játék (M)?");
            var gameType = Console.ReadLine();

            DrawingHelper dh = new DrawingHelper();

            switch (gameType)
            {
                case "M":

                    dh.Draw(game);

                    while (game.GameStillOn)
                    {
                        var command = Console.ReadLine();
                        //game.ExcecuteCommand(game.Map, command);
                        dh.Draw(game);
                    }

                    Console.WriteLine("You Won!");
                    break;

                case "G":
                    // Létrehozunk véletlenül egy játékot, aztán meg "megoldjuk visszafelé"
                    var mapGenerated = false;
                    var mapArray = new string[1];

                    while (!mapGenerated)
                    {
                        var mg = new RandomMapGenerator();
                        var map = File.ReadAllLines("map.txt"); //mg.GenerateMap(8);

                        //var lines2 = File.ReadAllLines("map.txt");
                        game.MapWidth = map[0].Length;
                        game.MapHeight = map.Length;

                        InitializeGame(map);

                        Console.WriteLine("Hány lépésből álljon a puzzle?");
                        var requiredSteps = Int32.Parse(Console.ReadLine());

                        var gm = new GameManager(game);
                        mapArray = gm.MakeGame(requiredSteps);

                        if (mapArray != null)
                            mapGenerated = true;

                        game.Map = new Dictionary<int, MapObject>();
                    }

                    Console.WriteLine("A generált pálya:");

                    foreach(var mapLine in mapArray)
                    {
                        Console.WriteLine(mapLine);
                    }

                    break;

                case "S":
                default:
                    var lines = File.ReadAllLines("map.txt");

                    //Console.WriteLine("Válasszon egy puzzle-t 0 és 2'577'411 között!");
                    //string numOfGame = Console.ReadLine();

                    //List<string[]> mapWithData = MapConverter.ConvertMapFromDatabase(Int32.Parse(numOfGame));

                    //var lines = mapWithData[0];

                    game.MapWidth = lines[0].Length;
                    game.MapHeight = lines.Length;

                    InitializeGame(lines);

                    var rh = new GameManager(game);
                    rh.SolveGame();
                    Console.WriteLine("Successfully solved!");
                    break;
            }
            //TODO: lépésszám, idő kiírása
        }

        private static void InitializeGame(string[] lines)
        {
            #region Map init
            //Init Map
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
            #endregion

            //Init Vehicles
            foreach (var item in game.Map)
            {
                if(item.Value.Code >= 'A' && item.Value.Code <= 'z')
                {
                    ((Vehicle)item.Value).CalcOrientation();
                }
            }
        }
    }
}
