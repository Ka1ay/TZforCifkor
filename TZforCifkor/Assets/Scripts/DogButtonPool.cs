using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DogButtonPool : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform container;

    private Queue<GameObject> _buttonPool = new();

    public GameObject GetButton()
    {
        if (_buttonPool.Count > 0)
        {
            GameObject button = _buttonPool.Dequeue();
            button.SetActive(true);
            return button;
        }
        return Instantiate(buttonPrefab, container);
    }

    public void ReturnButton(GameObject button)
    {
        button.SetActive(false);
        button.transform.SetParent(container);
        _buttonPool.Enqueue(button);
    }

    public void ClearPool()
    {
        foreach (var button in _buttonPool)
        {
            Destroy(button);
        }
        _buttonPool.Clear();
    }
}

