using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class ClockInstaller : MonoInstaller
{
    [SerializeField]
    private Canvas mainCanvas;

    [SerializeField]
    private Transform clockFace;

    [SerializeField]
    private StopAlarmButton stopAlarmButtonPrefab;

    private List<Type> simpleBind = new List<Type>()
    {
        typeof(ClockHandler),
        typeof(AlarmChecker),
        typeof(AlarmButtonBuilder),
        typeof(AngleToTimeDataConverter),
        typeof(DateTimeProvider)
    };

    public override void InstallBindings()
    {
        foreach (Type type in simpleBind)
            Container.BindInterfacesAndSelfTo(type)
                .AsSingle()
                .NonLazy();

        Container.Bind(typeof(ReactiveProperty<ClockMode>), typeof(IReactiveProperty<ClockMode>))
            .FromInstance(new ReactiveProperty<ClockMode>(ClockMode.Clock))
            .AsCached()
            .NonLazy();

        Container.Bind(typeof(ReactiveProperty<TimeData>), typeof(IReactiveProperty<TimeData>))
            .WithId(ZenjectIDs.CURRENT_TIME)
            .FromInstance(new ReactiveProperty<TimeData>())
            .AsCached()
            .NonLazy();

        Container.Bind(typeof(ReactiveProperty<TimeData>), typeof(IReactiveProperty<TimeData>))
            .WithId(ZenjectIDs.ALARM_TIME)
            .FromInstance(new ReactiveProperty<TimeData>())
            .AsCached()
            .NonLazy();

        Container.Bind(typeof(ReactiveProperty<ResponseStatus>), typeof(IReactiveProperty<ResponseStatus>))
            .WithId(ZenjectIDs.WAITING_FOR_RESPONSE)
            .FromInstance(new ReactiveProperty<ResponseStatus>(ResponseStatus.Waiting))
            .AsCached()
            .NonLazy();

        Container.Bind<Subject<bool>>()
            .WithId(ZenjectIDs.ALARM_FLAG)
            .FromInstance(new Subject<bool>())
            .AsCached()
            .NonLazy();

        Container.Bind<Subject<(ArrowType, int)>>()
            .WithId(ZenjectIDs.ARROW_CHANGED_ALARM)
            .FromInstance(new Subject<(ArrowType, int)>())
            .AsCached()
            .NonLazy();

        Container.BindIFactory<StopAlarmButton>()
            .FromMethod(CreateAlarmButton);

        Container.BindFactory<int, ArrowType, Arrow, Arrow.Factory>()
            .AsSingle()
            .NonLazy();

        Container.BindIFactory<ArrowView, ArrowType, ArrowView>()
            .FromMethod(CreateArrowView);
    }

    private StopAlarmButton CreateAlarmButton(DiContainer container)
    {
        StopAlarmButton stopAlarmButton = Instantiate(stopAlarmButtonPrefab, mainCanvas.transform);
        container.Inject(stopAlarmButton);
        return stopAlarmButton;
    }

    private ArrowView CreateArrowView(DiContainer container, ArrowView prefab, ArrowType arrowType)
    {
        var fullCircleSteps = 0;
        var angleStep = 0;
        switch (arrowType)
        {
            case ArrowType.Seconds:
                fullCircleSteps = ClockHandler.SECONDS_IN_MINUTE;
                angleStep = 360 / 60;
                break;
            case ArrowType.Minutes:
                fullCircleSteps = ClockHandler.SECONDS_IN_HOUR;
                angleStep = 360 / 60;
                break;
            case ArrowType.Hours:
                angleStep = 360 / 12;
                fullCircleSteps = ClockHandler.SECONDS_IN_DAY / 2;
                break;
        }

        var arrowFactory = container.Resolve<Arrow.Factory>();
        Arrow arrow = arrowFactory.Create(fullCircleSteps, arrowType);

        var arrowView = Instantiate(prefab, clockFace);

        container.InjectExplicit(arrowView, new List<TypeValuePair>()
        {
            new TypeValuePair(typeof(ReactiveProperty<float>), arrow.AngleValue),
            new TypeValuePair(typeof(int), angleStep),
            new TypeValuePair(typeof(ArrowType), arrowType)
        });

        return arrowView;
    }
}

