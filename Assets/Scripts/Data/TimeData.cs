public struct TimeData
{
    public int Hours;
    public int Minutes;
    public int Seconds;

    public TimeData(int hours, int minutes, int second)
    {
        Hours = hours;
        Minutes = minutes;
        Seconds = second;
    }

    public int ConvertToSeconds()
        => Seconds + ClockHandler.SECONDS_IN_MINUTE * Minutes + ClockHandler.SECONDS_IN_HOUR * Hours;
}
