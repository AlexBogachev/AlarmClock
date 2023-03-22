using UniRx;
using Zenject;

public class AlarmChecker
{
    private Subject<bool> alarmFlag;

    private int alarmSeconds;

    public AlarmChecker([Inject (Id = ZenjectIDs.ALARM_TIME)] IReactiveProperty<TimeData>alarmTime,
                        [Inject (Id = ZenjectIDs.ALARM_FLAG)] Subject<bool>alarmFlag) 
    {
        this.alarmFlag = alarmFlag;
        alarmSeconds = alarmTime.Value.ConvertToSeconds();

        alarmTime
            .Subscribe(x =>
            {
                alarmSeconds = x.ConvertToSeconds();
            });
    }

    public void CheckAlarm(TimeData currentTime)
    {
        var currentSeconds = currentTime.ConvertToSeconds();
        if (currentSeconds == alarmSeconds)
            alarmFlag.OnNext(true);
    }
}
