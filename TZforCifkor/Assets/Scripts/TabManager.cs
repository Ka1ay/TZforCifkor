using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TabManager : MonoBehaviour
{
    [SerializeField] private GameObject weatherPanel;
    [SerializeField] private GameObject dogBreedsPanel;

    [SerializeField] private Button weatherButton;
    [SerializeField] private Button dogsButton;

    private void Start()
    {
        weatherButton.onClick.AddListener(() => ShowWeatherPanel());
        dogsButton.onClick.AddListener(() => ShowDogBreedsPanel());

        ShowWeatherPanel();
    }

    private void ShowWeatherPanel()
    {
        weatherPanel.SetActive(true);
        dogBreedsPanel.SetActive(false);

        weatherPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        dogBreedsPanel.transform.DOScale(0f, 0.3f);
    }

    private void ShowDogBreedsPanel()
    {
        weatherPanel.SetActive(false);
        dogBreedsPanel.SetActive(true);

        dogBreedsPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        weatherPanel.transform.DOScale(0f, 0.3f);
    }
}

