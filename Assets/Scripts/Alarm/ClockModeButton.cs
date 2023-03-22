using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ClockModeButton : MonoBehaviour
{
    private TMP_Text text;

    [Inject]
    private void Constructor(ReactiveProperty<ClockMode> clockMode)
    {
        text = GetComponentInChildren<TMP_Text>();

        GetComponent<Button>().OnClickAsObservable()
            .Subscribe(x => ChangeMode(clockMode))
            .AddTo(this);
    }

    private void ChangeMode(ReactiveProperty<ClockMode> clockMode)
    {
        var currentMode = clockMode.Value;
        if(currentMode == ClockMode.Clock)
        {
            clockMode.Value = ClockMode.Alarm;
            text.text = "◊¿—€";
        }
        else
        {
            clockMode.Value = ClockMode.Clock;
            text.text = "¡”ƒ»À‹Õ» ";
        }
    }
}
