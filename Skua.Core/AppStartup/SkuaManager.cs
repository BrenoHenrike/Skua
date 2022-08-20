using Microsoft.Extensions.DependencyInjection;
using Skua.Core.ViewModels;

namespace Skua.Core.AppStartup;
internal class SkuaManager
{
    internal static ManagerMainViewModel CreateViewModel(IServiceProvider s)
    {
        return new(new List<TabItemViewModel>() { new("Github Auth", s.GetRequiredService<GitHubAuthViewModel>()) });
    }
}
