using System;

namespace ConsoleGraphics
{
    /// <summary>
    /// Console Progress Bar
    /// </summary>
    public class ConsoleProgressBar : IDisposable
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
        /// The Start Time used for estimating the end time!
        /// </summary>
        public DateTime? StartTime { get; private set; }

        /// <summary>
        /// Enable or not the ETA calculation
        /// </summary>
        public bool ShowETA { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="totalUnitsOfWork">Total amount of work. Used for calculating current percentage complete.</param>
        /// <param name="startingPosition">Progress bar starting position. Defaults to 0.</param>
        /// <param name="widthInCharacters">Size of progress bar in characters. Defaults to 40.</param>
        /// <param name="completedColor">Color for completed portion of progress bar. Defaults to Cyan.</param>
        /// <param name="remainingColor">Color for incomplete portion of progress bar. Defaults to Black.</param>
        /// <param name="showETA">Show or not ETA</param>
        public ConsoleProgressBar(
            uint totalUnitsOfWork,
            int startingPosition = 0,
            uint widthInCharacters = 40,
            ConsoleColor completedColor = ConsoleColor.Cyan,
            ConsoleColor remainingColor = ConsoleColor.Black,
            bool showETA = false)
        {
            TotalUnitsOfWork = totalUnitsOfWork;
            StartingPosition = startingPosition;
            WidthInCharacters = widthInCharacters;
            CompletedColor = completedColor;
            RemainingColor = remainingColor;

            _unitsOfWorkPerProgressBlock = (float)TotalUnitsOfWork / WidthInCharacters;
            _originalCursorVisible = Console.CursorVisible;
        }

        /// <summary>
        /// Declare the start of the work to calculate properly the ETA
        /// </summary>
        public void StartWork()
        {
            StartTime = DateTime.UtcNow;
            ShowETA = true;
        }

        /// <summary>
        /// Draws progress bar.
        /// </summary>
        /// <param name="currentUnitOfWork">Current unit of work in relation to TotalUnitsOfWork.</param>
        public void Draw(uint currentUnitOfWork)
        {
            if (currentUnitOfWork > TotalUnitsOfWork)
            {
                throw new ArgumentOutOfRangeException("currentUnitOfWork", "currentUnitOfWork must be less than TotalUnitsOfWork");
            }

            if (ShowETA && !StartTime.HasValue)
            {
                throw new Exception("Need to call StartWork if want to calculate the ETA");
            }

            var originalBackgroundColor = Console.BackgroundColor;
            Console.CursorVisible = false;
            Console.CursorLeft = StartingPosition;

            try
            {
                var completeProgressBlocks = (uint)Math.Round(currentUnitOfWork / _unitsOfWorkPerProgressBlock);
                WriteCompletedProgressBlocks(completeProgressBlocks);
                WriteRemainingProgressBlocks(completeProgressBlocks);

                var percentComplete = (double)((double)currentUnitOfWork / (double)TotalUnitsOfWork);
                WriteProgressText(currentUnitOfWork, percentComplete * 100.0, originalBackgroundColor);

                if (ShowETA && StartTime.HasValue && currentUnitOfWork > 0)
                {
                    //elasped time till now
                    TimeSpan elapsedTime = DateTime.UtcNow - StartTime.Value;

                    var elapsedTicks = elapsedTime.Ticks;

                    //calculate per item elapsed time
                    var perItemElapsedTicks = ((double)elapsedTicks / currentUnitOfWork);

                    var etaTicks = perItemElapsedTicks * TotalUnitsOfWork;

                    //project for the whole number of item
                    TimeSpan missingTimeEstimated = TimeSpan.FromTicks((long)etaTicks) - elapsedTime;

                    WriteETA(missingTimeEstimated, originalBackgroundColor);
                }

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

        private void WriteProgressText(uint currentUnitOfWork, double percentComplete, ConsoleColor originalBackgroundColor)
        {
            Console.BackgroundColor = originalBackgroundColor;
            Console.Write(" {0,5}% ({1} of {2})", percentComplete.ToString("n2"), currentUnitOfWork, TotalUnitsOfWork);
        }

        private void WriteETA(TimeSpan eta, ConsoleColor originalBackgroundColor)
        {
            Console.BackgroundColor = originalBackgroundColor;
            
            if (eta < TimeSpan.FromMinutes(1))
            {
                Console.Write(" Less than a minute");
            }
            else
            {
                Console.Write(" {0} hour(s) {1} minute(s)", Math.Floor(eta.TotalHours), eta.Minutes);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Console.CursorVisible = _originalCursorVisible;
        }
    }
}