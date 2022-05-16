using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Skua.Core.Interfaces;

namespace Skua.Core.PostSharp;
internal class Test
{
    static internal IFlashUtil FlashUtil { get; } = Ioc.Default.GetService<IFlashUtil>()!;
}
