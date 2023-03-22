using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StopAlarmButton : MonoBehaviour
{
    private Color alarmColor = Color.red;
    private Color baseColor = Color.white;

    private Image image;

    [Inject]
    private void Constructor([Inject(Id = ZenjectIDs.ALARM_FLAG)] Subject<bool> alarmFlag)
    {
        image = GetComponent<Image>();

        GetComponent<Button>().OnClickAsObservable()
            .Subscribe(x => 
            {
                alarmFlag.OnNext(false);
                Destroy(gameObject);
            })
            .AddTo(this);

        gameObject.UpdateAsObservable()
            .Subscribe(_ => 
            {
                var pingPong = Mathf.PingPong(Time.time, 1);
                var color = Color.Lerp(baseColor, alarmColor, pingPong);
                image.color = color;
            })
            .AddTo(this);
    }
}
