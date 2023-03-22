using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ArrowView : MonoBehaviour
{
    [Inject]
    private void Constructor(ReactiveProperty<float>angle, int angleStep, ArrowType type, 
                             IReactiveProperty<ClockMode> clockMode,
                             [Inject(Id = ZenjectIDs.ARROW_CHANGED_ALARM)] Subject<(ArrowType type, int roundedValue)> alarmAngleValue)
    {
        angle
            .Subscribe(x => SetRotation(x))
            .AddTo(this);

        var image = GetComponentInChildren<Image>();
        image.OnDragAsObservable()
            .Where(_ => clockMode.Value == ClockMode.Alarm)
            .Subscribe(x=> 
            {
                var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var direction = worldPosition - Vector3.zero;
                var rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(direction.x, direction.y, 0.0f));
                var roundedValue = Mathf.RoundToInt(rotation.eulerAngles.z / angleStep);
                if (roundedValue == 0)
                    roundedValue = 360 / angleStep;
                var roundedAngle = roundedValue * angleStep;
                transform.rotation = Quaternion.Euler(0.0f,0.0f, roundedAngle);
                alarmAngleValue.OnNext((type, 360/angleStep - roundedValue));
            })
            .AddTo(this);
    }

    private void SetRotation(float angle)
    {
        transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
    }
}
