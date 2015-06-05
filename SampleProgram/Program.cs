using System;
using System.Threading;
using ConsoleGraphics;

namespace SampleProgram
{
    /// <summary>
    /// Simple program with sample usage of ConsoleProgressBar.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var progressBar = new ConsoleProgressBar(totalUnitsOfWork: 3500))
            {
                for (uint i = 0; i < 3500; ++i)
                {
                    progressBar.Draw(i + 1);
                    Thread.Sleep(1);
                }
            }

            using (var progressBar = new ConsoleProgressBar(
                totalUnitsOfWork: 2000,
                startingPosition: 3,
                widthInCharacters: 48,
                completedColor: ConsoleColor.DarkBlue,
                remainingColor: ConsoleColor.DarkGray))
            {
                for (uint i = 0; i < 2000; ++i)
                {
                    progressBar.Draw(i + 1);
                    Thread.Sleep(1);
                }
            }
        }
    }
}
