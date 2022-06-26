using Skua.Core.Models;

namespace Skua.Core.Interfaces.Services;
public interface IGrabberService
{
    List<object> Grab(GrabberTypes grabType);
}
