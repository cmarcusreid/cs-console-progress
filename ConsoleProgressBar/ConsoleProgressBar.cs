using System;

namespace ConsoleProgressBar
{
    class ConsoleProgressBar : IDisposable
    {
        private const char ProgressBlockCharacter = ' ';
        private readonly float _unitsOfWorkPerProgressBlock;
        private readonly bool _originalCursorVisible;

        /// <summary>
        /// Color for completed portion of progress bar.
        /// </summary>
        public ConsoleColor CompletedColor { get; private set; }

        /// <summary>
        /// Color for incomplete portion of progress bar.
        /// </summary>
        public ConsoleColor RemainingColor { get; private set; }

        /// <summary>
        /// Progress bar starting position.
        /// </summary>
        public int StartingPosition { get; private set; }

        /// <summary>
        /// Size of progress bar in characters.
        /// </summary>
        public uint WidthInCharacters { get; private set; }

        /// <summary>
        /// Total amount of work. Used for calculating current percentage complete .
        /// </summary>
        public uint TotalUnitsOfWork { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="totalUnitsOfWork">Total amount of work. Used for calculating current percentage complete.</param>
        /// <param name="startingPosition">Progress bar starting position. Defaults to 0.</param>
        /// <param name="widthInCharacters">Size of progress bar in characters. Defaults to 40.</param>
        /// <param name="completedColor">Color for completed portion of progress bar. Defaults to Cyan.</param>
        /// <param name="remainingColor">Color for incomplete portion of progress bar. Defaults to Black.</param>
        public ConsoleProgressBar(
            uint totalUnitsOfWork,
            int startingPosition = 0,
            uint widthInCharacters = 40,
            ConsoleColor completedColor = ConsoleColor.Cyan,
            ConsoleColor remainingColor = ConsoleColor.Black)
        {
            TotalUnitsOfWork = totalUnitsOfWork;
            StartingPosition = startingPosition;
            WidthInCharacters = widthInCharacters;
            CompletedColor = completedColor;
            RemainingColor = remainingColor;

            _unitsOfWorkPerProgressBlock = (float) TotalUnitsOfWork / WidthInCharacters;
            _originalCursorVisible = Console.CursorVisible;
        }

        /// <summary>
        /// Draws progress bar.
        /// </summary>
        /// <Param name="currentUnitOfWork">Current unit of work in relation to TotalUnitsOfWork.</Param>
        public void Draw(uint currentUnitOfWork)
        {
            if (currentUnitOfWork > TotalUnitsOfWork)
            {
                throw new ArgumentOutOfRangeException("currentUnitOfWork", "currentUnitOfWork must be less than TotalUnitsOfWork");
            }

            var originalBackgroundColor = Console.BackgroundColor;
            Console.CursorVisible = false;
            Console.CursorLeft = StartingPosition;

            try
            {
                var completeProgressBlocks = (uint) (currentUnitOfWork / _unitsOfWorkPerProgressBlock);
                WriteCompletedProgressBlocks(completeProgressBlocks);
                WriteRemainingProgressBlocks(completeProgressBlocks);

                var percentComplete = (float) currentUnitOfWork / TotalUnitsOfWork * 100;
                WriteProgressText(currentUnitOfWork, percentComplete, originalBackgroundColor);

                if (currentUnitOfWork == TotalUnitsOfWork)
                {
                    Console.WriteLine();
                }
            }
            finally
            {
                Console.BackgroundColor = originalBackgroundColor;
            }
        }

        private void WriteCompletedProgressBlocks(uint completeProgressBlocks)
        {
            Console.BackgroundColor = CompletedColor;
            for (var i = 0; i < completeProgressBlocks; ++i)
            {
                Console.Write(ProgressBlockCharacter);
            }
        }

        private void WriteRemainingProgressBlocks(uint completeProgressBlocks)
        {
            Console.BackgroundColor = RemainingColor;
            for (var i = completeProgressBlocks; i < WidthInCharacters; ++i)
            {
                Console.Write(ProgressBlockCharacter);
            }
        }

        private void WriteProgressText(uint currentUnitOfWork, float percentComplete, ConsoleColor originalBackgroundColor)
        {
            Console.BackgroundColor = originalBackgroundColor;
            Console.Write(" {0,5}% ({1} of {2})", percentComplete.ToString("n2"), currentUnitOfWork, TotalUnitsOfWork);
        }

        public void Dispose()
        {
            Console.CursorVisible = _originalCursorVisible;
        }
    }
}
