﻿using UnityEngine;
using Zenject;

public class ClockFaceConstructor: MonoBehaviour
{
    [SerializeField]
    ArrowView hoursArrowPrefab;

    [SerializeField]
    ArrowView minutesArrowPrefab;

    [SerializeField]
    ArrowView secondsArrowPrefab;

    [Inject]
    private void Constructor(IFactory<ArrowView, ArrowType, ArrowView> arrowsFactory)
    {
        ArrowView hoursArrow = arrowsFactory.Create(hoursArrowPrefab, ArrowType.Hours);
        ArrowView minutesArrow = arrowsFactory.Create(minutesArrowPrefab, ArrowType.Minutes);
        ArrowView secondsArrow = arrowsFactory.Create(secondsArrowPrefab, ArrowType.Seconds);
    }
}
