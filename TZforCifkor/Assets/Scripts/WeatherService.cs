using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class WeatherService : IInitializable, IDisposable
{
    private const string WeatherApiUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
    private readonly RequestQueue _requestQueue;
    private bool _isActive;
    private bool _isRequesting;

    public event Action<string, string> OnWeatherUpdated;

    [Inject]
    public WeatherService(RequestQueue requestQueue)
    {
        _requestQueue = requestQueue;
    }

    [System.Serializable]
    public class WeatherResponse
    {
        public Properties properties;
    }

    [System.Serializable]
    public class Properties
    {
        public ForecastPeriod[] periods;
    }

    [System.Serializable]
    public class ForecastPeriod
    {
        public string name;
        public int temperature;
        public string shortForecast;
        public string icon;
    }

    public void Initialize() { }

    public void Activate()
    {
        if (_isActive) return;
        _isActive = true;
        StartWeatherUpdates();
    }

    public void Deactivate()
    {
        _isActive = false;
        _requestQueue.CancelAllRequests();
    }

    private async void StartWeatherUpdates()
    {
        while (_isActive)
        {
            if (!_isRequesting)
            {
                _isRequesting = true;
                _requestQueue.EnqueueRequest(GetWeather);
            }
            await Task.Delay(5000);
        }
    }

    private async Task GetWeather()
    {
        using UnityWebRequest request = UnityWebRequest.Get(WeatherApiUrl);
        request.SetRequestHeader("User-Agent", "UnityApp");

        var operation = request.SendWebRequest();
        while (!operation.isDone) await Task.Yield();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var weatherData = JsonUtility.FromJson<WeatherResponse>(request.downloadHandler.text);
            string temperature = weatherData.properties.periods[0].temperature + "F";
            string iconUrl = weatherData.properties.periods[0].icon;

            OnWeatherUpdated?.Invoke(temperature, iconUrl);
        }

        _isRequesting = false;
    }

    

    public void Dispose()
    {
        Deactivate();
    }
}
