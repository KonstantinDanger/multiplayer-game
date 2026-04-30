using System;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    private readonly List<UI> _ui = new();
    private readonly Stack<UIView> _viewStack = new();

    private Action OnViewOpen;
    private Action OnAllViewsClose;

    public bool HasStackedUIViews => _viewStack.Count > 0;

    public void Initialize(Action onViewOpen, Action onAllViewsClose)
    {
        OnViewOpen = onViewOpen;
        OnAllViewsClose = onAllViewsClose;
    }

    public void Add(UI ui)
    {
        if (_ui.Contains(ui))
            return;

        _ui.Add(ui);
    }

    public void OpenViewOfType(Type viewType)
    {
        if (!typeof(UIView).IsAssignableFrom(viewType))
            return;

        UIView selected = null;

        foreach (UI ui in _ui)
        {
            if (ui is UIView view && view.GetType() == viewType)
            {
                selected = view;
                break;
            }
        }

        if (!selected || _viewStack.Contains(selected))
            return;

        _viewStack.Push(selected);
        _viewStack.Peek().Open();

        OnViewOpen?.Invoke();
    }

    /// <summary>
    /// Closes last (peek) element of the stack
    /// </summary>
    public void CloseView()
    {
        if (_viewStack.Count == 0)
            return;

        _viewStack.Pop().Close();

        if (_viewStack.Count == 0)
            OnAllViewsClose?.Invoke();
    }

    public void OnSceneChange()
    {
        foreach (UI ui in _ui)
            if (!ui.PersistentThroughScenes)
                Destroy(ui);
    }
}
