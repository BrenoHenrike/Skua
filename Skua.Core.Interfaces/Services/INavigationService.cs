using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skua.Core.Interfaces.Services;
public interface INavigationService
{
    void Navigate<TViewModel>();
}
