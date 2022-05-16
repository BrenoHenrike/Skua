namespace Skua.Core.Interfaces;
public interface IScriptHandlers
{
    /// <summary>
    /// A list holding currently registered handlers.
    /// </summary>
    List<IHandler> CurrentHandlers { get; }

    /// <summary>
    /// Register an <paramref name="function"/> to be executed every time the specified number of <paramref name="ticks"/> has passed. A tick is 20ms.
    /// </summary>
    /// <param name="ticks">Number of ticks between consecutive executions of the action.</param>
    /// <param name="function">Action to carry out. If this function returns false, the handler will be removed.</param>
    /// <param name="name">Name of this handler (must be unique). Passing null will assign it a unique name.</param>
    /// <returns>The <see cref="IHandler"/> registered.</returns>
    IHandler RegisterHandler(int ticks, Func<IScriptInterface, bool> function, string name = null!);
    /// <summary>
    /// Register an <paramref name="function"/> to be executed every time the specified number of <paramref name="ticks"/> has passed. A tick is 20ms.
    /// </summary>
    /// <param name="ticks">Number of ticks between consecutive executions of the action.</param>
    /// <param name="function">Action to carry out at every interval</param>
    /// <param name="name">Name of this handler (must be unique). Passing null will assign it a unique name.</param>
    /// <returns>The <see cref="IHandler"/> registered.</returns>
    IHandler RegisterHandler(int ticks, Action<IScriptInterface> function, string name = null!)
    {
        return RegisterHandler(ticks, b =>
        {
            function(b);
            return true;
        }, name);
    }
    /// <summary>
    /// Register an <paramref name="function"/> to be executed every time the specified number of <paramref name="ticks"/> has passed. A tick is 20ms.
    /// </summary>
    /// <param name="ticks">Number of ticks between consecutive executions of the action.</param>
    /// <param name="function">Action to be executed once.</param>
    /// <param name="name">Name of this handler (must be unique). Passing null will assign it a unique name.</param>
    /// <returns>The <see cref="IHandler"/> registered.</returns>
    IHandler RegisterOnce(int ticks, Action<IScriptInterface> funtion, string name = null!)
    {
        return RegisterHandler(ticks, b =>
        {
            funtion(b);
            return false;
        }, name);
    }
    /// <summary>
    /// Removes the handler with specified name.
    /// </summary>
    /// <param name="name">Name of the handler to remove.</param>
    /// <returns><see langword="true"/> if the handler was removed.</returns>
    bool Remove(string name)
    {
        return CurrentHandlers.RemoveAll(h => h.Name == name) > 0;
    }
    /// <summary>
    /// Removes the specified handler.
    /// </summary>
    /// <param name="handler">Handler to remove.</param>
    /// <returns><see langword="true"/> if the handler was removed.</returns>
    bool Remove(IHandler handler)
    {
        return CurrentHandlers.Remove(handler);
    }
    /// <summary>
    /// Removes all the specified handlers.
    /// </summary>
    /// <param name="handlers">Handlers to remove</param>
    /// <returns><see langword="true"/> if all the handlers where removed.</returns>
    bool Remove(List<IHandler> handlers)
    {
        return CurrentHandlers.RemoveAll(h => handlers.Contains(h)) == handlers.Count;
    }
}
