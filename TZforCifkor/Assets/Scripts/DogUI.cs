using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

public class DogUI : MonoBehaviour
{
    [SerializeField] private DogButtonPool buttonPool;
    [SerializeField] private Transform breedListContainer;
    [SerializeField] private GameObject loader;
    [SerializeField] private GameObject popup;
    [SerializeField] private TMP_Text popupTitle;
    [SerializeField] private TMP_Text popupDescription;

    private DogService _dogService;
    private List<GameObject> _activeButtons = new();

    [Inject]
    public void Construct(DogService dogService)
    {
        _dogService = dogService;
        _dogService.OnBreedsLoaded += UpdateBreedList;
        _dogService.OnBreedDetailsLoaded += ShowPopup;
    }

    private void OnEnable()
    {
        loader.SetActive(true);
        _dogService.FetchBreeds();
    }

    private void UpdateBreedList(List<(string id, string name)> breeds)
    {
        loader.SetActive(false);

        foreach (var button in _activeButtons)
        {
            buttonPool.ReturnButton(button);
        }
        _activeButtons.Clear();

        for (int i = 0; i < breeds.Count; i++)
        {
            var breed = breeds[i];
            GameObject breedButton = buttonPool.GetButton();
            Button button = breedButton.GetComponent<Button>();
            TMP_Text buttonText = breedButton.GetComponentInChildren<TMP_Text>();

            buttonText.text = $"{i + 1} - {breed.name}";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnBreedSelected(breed.id));

            _activeButtons.Add(breedButton);
        }
    }

    public void OnBreedSelected(string breedId)
    {
        loader.SetActive(true);
        _dogService.FetchBreedDetails(breedId);
    }

    private void ShowPopup(string name, string description)
    {
        loader.SetActive(false);
        popup.SetActive(true);
        popupTitle.text = name;
        popupDescription.text = description;
        popup.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void ClosePopup()
    {
        popup.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() => popup.SetActive(false));
    }
}







