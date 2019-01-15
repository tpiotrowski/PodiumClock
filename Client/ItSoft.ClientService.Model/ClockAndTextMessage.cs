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

        public override string ToString()
        {
            return $"{nameof(IndicatorEnabled)}: {IndicatorEnabled}, {nameof(Sign)}: {Sign}, {nameof(Minutes)}: {Minutes}, {nameof(Seconds)}: {Seconds}, {nameof(Text)}: {Text}, {nameof(Time)}: {Time}";
        }
    }
}