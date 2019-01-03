namespace ItSoft.ClientService
{
    public class ClockAndTextMessage
    {
        public bool IndicatorEnabled { get; set; }

        public char Sign { get; set; }

        public string Minutes { get; set; }

        public string Seconds { get; set; }

        public string Text { get; set; }

        public string Time => $"{Sign} {Minutes}:{Seconds}";

    }
}