namespace CodingTracker
{
    internal class CodingSession
    {
        // Class fields must be public for ConsoleTableExt to retrieve column count from different assembly
        public long ID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Duration { get; set; }

        public CodingSession(long ID, string StartTime, string EndTime, string Duration)
        {
            this.ID = ID;
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.Duration = Duration;
        }
    }
}
