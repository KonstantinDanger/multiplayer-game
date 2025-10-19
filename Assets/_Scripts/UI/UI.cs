using Mirror;
using System;

public class UI : NetworkBehaviour, IDisposable
{
    //[SerializeField] private bool _persistent = false;

    //public bool Persistent => _persistent;

    //private void OnValidate()
    //{
    //    if (this is MainUI)
    //        _persistent = true;
    //}

    //private void Start()
    //{
    //    if (this is MainUI)
    //        if (!_persistent)
    //            _persistent = true;
    //}

    public virtual void Dispose() { }
}
