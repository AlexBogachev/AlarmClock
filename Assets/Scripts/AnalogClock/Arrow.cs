using UniRx;
using Zenject;

public class Arrow
{
    public ReactiveProperty <float> AngleValue { get; private set; }

    private int fullCircleSteps;
    private float angleStep;

    public Arrow(int fullCircleSteps, ArrowType arrowType,
                 [Inject (Id = ZenjectIDs.CURRENT_TIME)] IReactiveProperty<TimeData> currentTime,
                 [Inject(Id = ZenjectIDs.ALARM_TIME)] IReactiveProperty<TimeData> alarmTime)
    {
        AngleValue = new ReactiveProperty<float>();

        this.fullCircleSteps = fullCircleSteps;

        angleStep = 360.0f / fullCircleSteps;

        currentTime
            .Subscribe(x =>
            {
                UpdateRotation(x.ConvertToSeconds());
            });
        
        alarmTime
            .Skip(1)
            .Subscribe(x =>
            {
                UpdateAlarmRotation(x.ConvertToSeconds(), arrowType);
            });
    }

    private void UpdateRotation(int currentValue)
    {
        var valueOnCircle = currentValue % fullCircleSteps;
        AngleValue.Value = angleStep * valueOnCircle;
    }

    private void UpdateAlarmRotation(int currentValue, ArrowType arrowType)
    {
        if (arrowType == ArrowType.Hours)
            currentValue = currentValue - currentValue % ClockHandler.SECONDS_IN_HOUR;
        var valueOnCircle = currentValue % fullCircleSteps;
        AngleValue.Value = angleStep * valueOnCircle;
    }

    public class Factory : PlaceholderFactory<int, ArrowType, Arrow> { }
}
