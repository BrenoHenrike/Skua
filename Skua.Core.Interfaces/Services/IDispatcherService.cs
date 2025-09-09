namespace Skua.Core.Interfaces;

public interface IDispatcherService
{
    void Invoke(Action action);
}