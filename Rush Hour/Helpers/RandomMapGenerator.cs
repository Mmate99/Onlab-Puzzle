using System;

namespace Rush_Hour.Helpers
{
    class RandomMapGenerator
    {
        private string[] map;
        private int carNumber = 0;
        private bool isCarHorizontal, isCarSizeTwo;
        private int i, j;
        private char carLetter = 'b';

        public string[] GenerateMap(int carNum)
        {
            InitMap();
            var r = new Random();

            while (carNumber < carNum)
            {
                isCarHorizontal = (r.NextDouble() - 0.5) <= 0;
                isCarSizeTwo = (r.NextDouble() - 0.7) <= 0;

                int verticalOffset = 0;
                int horizontalOffset = 0;
                if (isCarHorizontal)
                {
                    verticalOffset = 2;
                    horizontalOffset = isCarSizeTwo ? 1 : 0;
                }
                else
                {
                    verticalOffset = isCarSizeTwo ? 1 : 0;
                    horizontalOffset = 2;
                }

                i = r.Next(1, 5 + verticalOffset);    //ha horizontális, akkor az utsó oszlopban nem kezdődhet, ha vertikális akkor meg a legalsó sorban nem kezdődhet
                j = r.Next(1, 5 + horizontalOffset);

                PutCarOnMap();
            }

            foreach (var line in map)
            {
                Console.WriteLine(line);
            }

            return map;
        }

        private void InitMap()
        {
            map = new string[8];

            map[0] = "########";
            map[7] = "########";

            for (int i = 1; i < 7; i++)
            {
                map[i] = "#      #";
            }

            map[3] = "#    aa0";
        }

        private void PutCarOnMap()
        {
            var diffHor = isCarHorizontal ? 1 : 0;
            var diffVer = isCarHorizontal ? 0 : 1;

            if ((map[i][j] == ' ') && (map[i+diffVer][j+diffHor] == ' ') && (isCarSizeTwo ? true : (map[i + diffVer * 2][j + diffHor * 2] == ' ')))
            {
                map[i] = map[i].Insert(j, carLetter.ToString()).Remove(j+1,1);
                map[i + diffVer] = map[i + diffVer].Insert(j + diffHor, carLetter.ToString()).Remove(j + diffHor + 1, 1);

                if (!isCarSizeTwo)
                    map[i + diffVer * 2] = map[i + diffVer * 2].Insert(j + diffHor * 2, carLetter.ToString()).Remove(j + diffHor * 2 + 1, 1);

                carNumber++;
                carLetter++;
            }
        }
    }
}
