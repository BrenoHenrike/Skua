using Skua.Core.Models;

namespace Skua.Core.Interfaces;
public interface IGrabberService
{
    List<object> Grab(GrabberTypes grabType);
}
