using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Zenject;

public class DogService : IInitializable, IDisposable
{
    private const string DogApiUrl = "https://dogapi.dog/api/v2/breeds";
    private const string DogDetailsApiUrl = "https://dogapi.dog/api/v2/breeds/{0}";

    private readonly RequestQueue _requestQueue;
    private bool _isRequesting;

    public event Action<List<(string id, string name)>> OnBreedsLoaded;
    public event Action<string, string> OnBreedDetailsLoaded;

    [Inject]
    public DogService(RequestQueue requestQueue)
    {
        _requestQueue = requestQueue;
    }

    public void CancelCurrentRequest()
    {
        if (_isRequesting)
        {
            _requestQueue.CancelAllRequests();
            _isRequesting = false;
        }
    }

    [System.Serializable]
    public class BreedDetailsResponse
    {
        public BreedData data;
    }

    [System.Serializable]
    public class BreedAttributes
    {
        public string name;
        public string description;
    }

    [System.Serializable]
    public class BreedData
    {
        public string id;
        public string type;
        public BreedAttributes attributes;
    }

    [System.Serializable]
    public class DogApiResponse
    {
        public List<BreedData> data;
    }

    public void Initialize() { }

    public async void FetchBreeds()
    {
        if (_isRequesting) return;
        _isRequesting = true;
        await GetBreeds();
    }

    private async Task GetBreeds()
    {
        UnityWebRequest.ClearCookieCache();
        using UnityWebRequest request = UnityWebRequest.Get(DogApiUrl);
        request.SetRequestHeader("User-Agent", "UnityApp");

        var operation = request.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var jsonResponse = request.downloadHandler.text;
            var response = JsonUtility.FromJson<DogApiResponse>(jsonResponse);

            List<(string id, string name)> breeds = new();
            foreach (var breed in response.data)
            {
                breeds.Add((breed.id, breed.attributes.name));
            }

            OnBreedsLoaded?.Invoke(breeds);
        }
        _isRequesting = false;
    }

    public async void FetchBreedDetails(string breedId)
    {
        if (_isRequesting) return;
        _isRequesting = true;
        await GetBreedDetails(breedId);
    }

    private async Task GetBreedDetails(string breedId)
    {
        UnityWebRequest.ClearCookieCache();

        string url = string.Format(DogDetailsApiUrl, breedId);
        using UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("User-Agent", "UnityApp");

        var operation = request.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;

            var breedResponse = JsonUtility.FromJson<BreedDetailsResponse>(jsonResponse);

            var breedData = breedResponse.data;

            OnBreedDetailsLoaded?.Invoke(breedData.attributes.name, breedData.attributes.description);
        }

        _isRequesting = false;
    }

    public void Dispose()
    {
        CancelCurrentRequest();
        _requestQueue.CancelAllRequests();
    }
}




