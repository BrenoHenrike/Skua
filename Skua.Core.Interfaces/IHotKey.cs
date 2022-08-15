using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skua.Core.Interfaces;
public interface IHotKey
{
    string Binding { get; set; }
    string Title { get; set; }
    string KeyGesture { get; set; }
}
