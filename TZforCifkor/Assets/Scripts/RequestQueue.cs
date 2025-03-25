using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class RequestQueue : IInitializable, IDisposable
{
    private readonly Queue<Func<Task>> _requestQueue = new();
    private bool _isProcessing;

    public void EnqueueRequest(Func<Task> request)
    {
        _requestQueue.Enqueue(request);
        ProcessQueue();
    }

    private async void ProcessQueue()
    {
        if (_isProcessing || _requestQueue.Count == 0) return;

        _isProcessing = true;
        var request = _requestQueue.Dequeue();
        await request();
        _isProcessing = false;
        ProcessQueue();
    }

    public void CancelAllRequests()
    {
        _requestQueue.Clear();
    }

    public void Initialize() { }
    public void Dispose() => CancelAllRequests();
}
