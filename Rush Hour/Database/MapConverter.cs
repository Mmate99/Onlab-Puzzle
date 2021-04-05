using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rush_Hour.Database
{
    public static class MapConverter
    {
        public static List<string[]> ConvertMapFromDatabase(int line)
        {
            List<string[]> mapWithData = new List<string[]>();

            string lineFromFile = ReadLineFromDatabase(line);
            
            string[] splitLine = lineFromFile.Split(' ');
            string lineMap = splitLine[1];
            string numOfSteps = splitLine[0];
            string numOfStates = splitLine[2];
            string[] mapData = new string[] { numOfSteps, numOfStates };

            string firstRow = "########";
            string[] map = new string[8];
            map[0] = firstRow;

            lineMap = lineMap.Replace('x', '#').Replace('o', ' ');
            lineMap = lineMap.ToLower();

            // TEST BEFORE
            //Console.WriteLine(lineMap);

            List<char> listOfLine = new List<char>();
            listOfLine.AddRange(lineMap);
            listOfLine.Sort();

            var hasChanged = false;
            var changeCounter = 0;

            while (changeCounter < lineMap.Length)
            {
                var charCounter = 0;

                while (!hasChanged && charCounter < lineMap.Length)
                {
                    if (listOfLine[charCounter] > 97 && listOfLine[charCounter] <= 122 && (listOfLine[charCounter] != (listOfLine[charCounter - 1] + 1) && listOfLine[charCounter] != listOfLine[charCounter - 1]))
                    {
                        lineMap = lineMap.Replace(listOfLine[charCounter], (char)(listOfLine[charCounter - 1] + 1));
                        listOfLine = new List<char>();
                        listOfLine.AddRange(lineMap);
                        listOfLine.Sort();
                        hasChanged = true;
                    }
                    charCounter++;
                }
                hasChanged = false;
                changeCounter++;
            }

            // TEST AFTER
            //Console.WriteLine(lineMap);

            for (int i = 0; i < map.Length - 2; i++)
            {
                var newRow = "";
                var partOfRow = lineMap.Substring(i * 6, 6);
                if (partOfRow.Contains('a'))
                {
                    newRow = "#" + partOfRow + "0";
                }
                else
                {
                    newRow = "#" + partOfRow + "#";
                }

                map[i + 1] = newRow;
            }

            map[7] = firstRow;

            mapWithData.Add(map);
            mapWithData.Add(mapData);

            return mapWithData;
        }

        private static string ReadLineFromDatabase(int line)
        {
            using (var reader = new StreamReader("rush.txt"))
            {
                try
                {
                    for (int i = 0; i <= line; i++)
                    {
                        if (reader.EndOfStream)
                            return string.Format("There {1} less than {0} line{2} in the file.", line,
                                                 ((line == 1) ? "is" : "are"), ((line == 1) ? "" : "s"));

                        if (i == line)
                            return reader.ReadLine();

                        reader.ReadLine();
                    }
                }
                catch (IOException ex)
                {
                    return ex.Message;
                }
                catch (OutOfMemoryException ex)
                {
                    return ex.Message;
                }
            }

            throw new Exception("Something bad happened.");
        }
    }
}
