using UniRx;
using Zenject;

public class AlarmButtonBuilder
{
    [Inject]
    private IFactory<StopAlarmButton> stopAlarmButonFactory;

    public AlarmButtonBuilder([Inject(Id = ZenjectIDs.ALARM_FLAG)] Subject<bool> alarmFlag)
    {
        alarmFlag
            .Where(x => x)
            .Subscribe(x =>
            {
                stopAlarmButonFactory.Create();
            });
    }
}
