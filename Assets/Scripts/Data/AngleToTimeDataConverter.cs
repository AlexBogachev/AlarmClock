using UniRx;
using UnityEngine;
using Zenject;

public class AngleToTimeDataConverter
{
    private int prevMinutes = 0;
    private int prevHours = 0;

    public AngleToTimeDataConverter([Inject (Id = ZenjectIDs.ALARM_TIME)] IReactiveProperty<TimeData>alarmTime,
                                    [Inject (Id = ZenjectIDs.ARROW_CHANGED_ALARM)] Subject<(ArrowType type, int roundedValue)> alarmAngleValue)
    {
        alarmAngleValue
            .Subscribe(x =>
            {
                var alarm = alarmTime.Value;
                switch (x.type)
                {
                    case ArrowType.Hours:
                        var hours0 = alarm.Hours;
                        if (x.roundedValue != prevHours)
                            hours0 = UpdateHourValue(x.roundedValue, hours0);
                        alarmTime.Value = new TimeData(hours0, alarm.Minutes, 0);
                        prevHours = x.roundedValue;
                        break;
                    case ArrowType.Minutes:
                        var hours1 = alarm.Hours;
                        if (x.roundedValue != prevMinutes)
                            hours1 = CheckMinutesChangedHour(x.roundedValue, alarm.Hours);
                        alarmTime.Value = new TimeData(hours1, x.roundedValue, 0);
                        prevMinutes = x.roundedValue;
                        break;
                    case ArrowType.Seconds:
                        Debug.LogWarning("Секундная стрелка не влият на будильник");
                        break;
                }
            });
    }

    private int CheckMinutesChangedHour(int minutes, int hour)
    {
        var updatedValue = hour;
        if (prevMinutes == 0)
        {
            if (minutes > 30)
            {
                updatedValue = hour - 1;
                if (updatedValue == -1)
                    updatedValue = 23;
                return updatedValue;
            }
            else
            {
                return updatedValue;
            }
        }
        else if (minutes == 0)
        {
            if (prevMinutes >= 30)
            {
                updatedValue = hour + 1;
                if(updatedValue == 24)
                    updatedValue = 0;
                return updatedValue;
            }
            else
            {
                return updatedValue;
            }
        }
        else
            return updatedValue;
    }

    private int UpdateHourValue(int hour12, int alarmHour24)
    {
        if(prevHours == 0)
        {
            if(hour12 > 6)
            {
                if (alarmHour24 >= 12)
                {
                    alarmHour24 = 11;
                }
                else
                {
                    alarmHour24 = 23;
                }
                return alarmHour24;
            }
            else
            {
                if (alarmHour24 >= 12)
                {
                    alarmHour24 = 13;
                }
                else
                {
                    alarmHour24 = 1;
                }
                return alarmHour24;
            }
        }
        else if (hour12 == 0)
        {
            if (prevHours >= 6)
            {
                if (alarmHour24 > 12)
                {
                    alarmHour24 = 0;
                }
                else
                {
                    alarmHour24 = 12;
                }
                return alarmHour24;
            }
            else
            {
                if (alarmHour24 > 12)
                {
                    alarmHour24 = 12;
                }
                else
                {
                    alarmHour24 = 0;
                }
                return alarmHour24;
            }
        }
        else
        {
            if (alarmHour24 >= 12)
            {
                alarmHour24 = 12 + hour12;
            }
            else
            {
                alarmHour24 = hour12;
            }
            return alarmHour24;
        }
    }
}
