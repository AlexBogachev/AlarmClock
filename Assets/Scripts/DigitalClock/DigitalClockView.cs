using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class DigitalClockView : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField hoursInput;

    [SerializeField]
    private TMP_InputField minutesInput;

    IReactiveProperty<TimeData> alarmTime;

    [Inject]
    private void Constructor([Inject(Id = ZenjectIDs.CURRENT_TIME)] IReactiveProperty<TimeData> currentTime,
                             [Inject(Id = ZenjectIDs.ALARM_TIME)] IReactiveProperty<TimeData> alarmTime,
                             IReactiveProperty<ClockMode>mode)
    {
        this.alarmTime = alarmTime;

        mode
            .Subscribe(x => InputFieldsSetActive(x))
            .AddTo(this);

        currentTime
            .Where(x=>mode.Value!=ClockMode.Alarm)
            .Subscribe(x=>UpdateValues(x))
            .AddTo(this);

        alarmTime
            .Subscribe(x => UpdateValues(x)) 
            .AddTo(this);

        hoursInput.onEndEdit.AddListener(x =>
        {
            var currenValue = alarmTime.Value;
            if (int.TryParse(x, out int hours))
            {
                var clampedHours = Mathf.Clamp(hours, 0, 23);
                alarmTime.Value = new TimeData(clampedHours, currenValue.Minutes, 0);
                hoursInput.text = clampedHours.ToString();  
            }
                
            else
                Debug.LogWarning("Неудалось распарсить часы");
                
        });

        minutesInput.onEndEdit.AddListener(x =>
        {
            var currenValue = alarmTime.Value;
            if (int.TryParse(x, out int minutes))
            {
                var clampedMinutes = Mathf.Clamp(minutes, 0, 59);
                alarmTime.Value = new TimeData(currenValue.Hours, clampedMinutes, 0);
                minutesInput.text = clampedMinutes.ToString();
            }
                
            else
                Debug.LogWarning("Неудалось распарсить минуты");
        });
    }

    private void UpdateValues(TimeData data)
    {
        hoursInput.text = data.Hours.ToString();
        minutesInput.text = data.Minutes.ToString();
    }

    private void InputFieldsSetActive(ClockMode mode)
    {
        if(mode == ClockMode.Clock)
        {
            hoursInput.enabled = false;
            minutesInput.enabled = false;
        }
        else
        {
            hoursInput.enabled = true;
            minutesInput.enabled = true;

            var alarm = alarmTime.Value;
            hoursInput.text = alarm.Hours.ToString();
            minutesInput.text = alarm.Minutes.ToString();
        }
    }

    private void OnDestroy()
    {
        hoursInput.onEndEdit.RemoveAllListeners();
        minutesInput.onEndEdit.RemoveAllListeners();
    }
}
