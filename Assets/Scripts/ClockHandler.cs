using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UniRx;
using Zenject;

public enum ArrowType
{
    Seconds,
    Minutes,
    Hours
}

public enum ClockMode
{
    Clock,
    Alarm
}

public enum ResponseStatus
{
    Waiting,
    Complete,
    Failed
}

public class ClockHandler: IInitializable
{

    public const int SECONDS_IN_MINUTE = 60;
    public const int SECONDS_IN_HOUR = 60 * 60;
    public const int SECONDS_IN_DAY = 60 * 60 * 24;

    private IDisposable everySeconds;
    private IDisposable everyHour;

    [Inject]
    private DateTimeProvider timeProvider;

    private DateTime time;

    private ReactiveProperty<TimeData> currentTime;

    private IReactiveProperty<ResponseStatus> waitingForResponse;

    public ClockHandler([Inject (Id = ZenjectIDs.CURRENT_TIME)] ReactiveProperty<TimeData> currentTime,
                        [Inject (Id = ZenjectIDs.WAITING_FOR_RESPONSE)] IReactiveProperty<ResponseStatus> waitingForResponse,
                        IReactiveProperty<ClockMode> clockMode,
                        AlarmChecker alarmChecker) 
    {
        this.currentTime = currentTime;
        this.waitingForResponse = waitingForResponse;

        everySeconds = Observable.Interval(TimeSpan.FromSeconds(1))
            .Where(_=>waitingForResponse.Value != ResponseStatus.Waiting)
            .Subscribe(x =>
            {
                time = time.AddSeconds(1);
                alarmChecker.CheckAlarm(ConvertDateTimeToTimeData(time));
                if(clockMode.Value == ClockMode.Clock)
                    UpdateTimeData();
            });

        everyHour = Observable.Interval(TimeSpan.FromMinutes(60))
            .Subscribe(x =>
            {
                waitingForResponse.Value = ResponseStatus.Waiting;
                Initialize();
            });
    }

    public async void Initialize()
    {
        await SetData();
    }

    private async UniTask SetData()
    {
        var result = await timeProvider.GetDateTimeAsync();

        time = result.date;

        if (result.fromSystem)
            waitingForResponse.Value = ResponseStatus.Failed;
        else
            waitingForResponse.Value = ResponseStatus.Complete;
    }

    private void UpdateTimeData()
        => currentTime.Value = ConvertDateTimeToTimeData(time);

    private TimeData ConvertDateTimeToTimeData(DateTime timeToConvert)
    {
        var hours = timeToConvert.Hour;
        var minutes = timeToConvert.Minute;
        var seconds = timeToConvert.Second;

        return new TimeData(hours, minutes, seconds);
    } 
}