using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using Zenject;

public class WeatherView : MonoBehaviour
{
    [SerializeField] private TMP_Text weatherText;
    [SerializeField] private Image weatherIcon;

    private WeatherService _weatherService;

    [Inject]
    public void Construct(WeatherService weatherService)
    {
        _weatherService = weatherService;
        _weatherService.OnWeatherUpdated += UpdateWeatherUI;
    }

    private void UpdateWeatherUI(string temperature, string iconUrl)
    {
        if (weatherText != null)
        {
            weatherText.text = "Сегодня - " + temperature;
        }

        if (!string.IsNullOrEmpty(iconUrl))
        {
            StartCoroutine(LoadIcon(iconUrl));
        }

    }


    private IEnumerator LoadIcon(string url)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        Texture2D texture = DownloadHandlerTexture.GetContent(request);


        weatherIcon.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));

    }



    private void OnEnable() => _weatherService.Activate();
    private void OnDisable()
    {
        _weatherService.OnWeatherUpdated -= UpdateWeatherUI;
        _weatherService.Deactivate();
    }
}
