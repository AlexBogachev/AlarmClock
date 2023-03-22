using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class WarningPanel : MonoBehaviour
{
    TMP_Text textComponent;

    [Inject]
    private void Constructor([Inject(Id = ZenjectIDs.WAITING_FOR_RESPONSE)] IReactiveProperty<ResponseStatus> waitingForResponse)
    {
        textComponent =  GetComponentInChildren<TMP_Text>();

        waitingForResponse
            .Subscribe(x =>
            {
                switch (x)
                {
                    case ResponseStatus.Waiting:
                        SetActive(true, 0.0f);
                        textComponent.color = Color.red;
                        textComponent.enabled = true;
                        textComponent.text = "Ожидаю ответа от сервера";
                        break;
                    case ResponseStatus.Complete:
                        textComponent.color = Color.green;
                        textComponent.text = "Загрузка завершена успешно";
                        SetActive(false, 2.0f);
                        break;
                    case ResponseStatus.Failed:
                        textComponent.color = Color.red;
                        textComponent.text = "Ошибка загрузки. Установлено системное время";
                        SetActive(false, 2.0f);
                        break;
                }
            })
            .AddTo(this);
    }

    private async void SetActive(bool isActive, float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        textComponent.enabled = isActive;
    }
}
