namespace Skua.Core.Interfaces;

public interface IHotKey
{
    string Binding { get; set; }
    string Title { get; set; }
    string KeyGesture { get; set; }
}