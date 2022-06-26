using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skua.Core.Interfaces;
public interface IFileDialogService
{
    string? Open();
    string? Open(string filters);
    string? Open(string initialDirectory, string filters);
    IEnumerable<string>? OpenText();
    string? Save();
    string? Save(string filters);
    string? Save(string initialDirectory, string filters);
    void SaveText(string contents);
    void SaveText(IEnumerable<string> contents);
}
