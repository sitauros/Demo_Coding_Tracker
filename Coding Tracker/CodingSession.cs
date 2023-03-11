using System;

namespace CodingTracker
{
    internal class CodingSession
    {
        private int ID;
        private DateTime StartTime;
        private DateTime EndTime;

        public CodingSession(int ID,  DateTime StartTime, DateTime EndTime)
        {
            this.ID = ID;
            this.StartTime = StartTime;
            this.EndTime = EndTime;

        }

        private int CalculateDuration()
        {
            int delta = 0;

            return delta;
        }
    }
}
