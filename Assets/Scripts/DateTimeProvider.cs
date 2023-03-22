using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public class DateTimeProvider
{
    List<string> urls = new List<string>()
    {
        "http://www.msdn.com",
        "http://www.google.com",
        "http://www.microsoft.com",
    };


    public async Task<(DateTime date, bool fromSystem)> GetDateTimeAsync()
    {
        List <UniTask<(DateTime date, bool fromSystem)>> tasks = new List<UniTask<(DateTime date, bool fromSystem)>>();
        var tokenSource = new CancellationTokenSource();
        foreach (string url in urls)
        {
            var request = UnityWebRequest.Get(url);
            tasks.Add(GetDateTime(request, url, tokenSource));
        }

        var resultArrray = await UniTask.WhenAll(tasks);
        var result = resultArrray.LastOrDefault(x => !x.fromSystem);
        if(result == default)
            result = resultArrray[0];
        return result;
        
    }

    private async UniTask<(DateTime date, bool fromSystem)> GetDateTime(UnityWebRequest request, string url, CancellationTokenSource tokenSource)
    {
        try
        {
            var result = await request.SendWebRequest().WithCancellation(tokenSource.Token);

            tokenSource.Cancel();
            var header = result.GetResponseHeaders()["date"];

            var date = DateTime.ParseExact(header,
                        "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture.DateTimeFormat,
                        DateTimeStyles.AssumeUniversal);
            return (date, false);
        }
        catch (Exception e)
        {
            
            Debug.LogWarning("EXCEPTION: " + e);
            return (DateTime.Now, true);
        }
        finally
        {
            request.Dispose();
        }
    }
}

