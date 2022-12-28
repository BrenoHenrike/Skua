using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skua.Core.ViewModels;
public class ClientItemViewModel : ObservableObject
{
    public ClientItemViewModel()
    {

    }

    public string Name { get; set; }

    public string Path { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
