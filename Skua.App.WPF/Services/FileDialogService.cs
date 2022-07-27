using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skua.Core.Interfaces;
using Microsoft.Win32;
using System.IO;

namespace Skua.App.WPF.Services;
public class FileDialogService : IFileDialogService
{
    private const string defaultFilter = "Text Files (*.txt)|*.txt";
    public string? Open()
    {
        OpenFileDialog opf = new();
        opf.InitialDirectory = AppContext.BaseDirectory;
        return opf.ShowDialog() == true ? opf.FileName : null;
    }

    public string? Open(string filter)
    {
        OpenFileDialog opf = new()
        {
            InitialDirectory = AppContext.BaseDirectory,
            Filter = filter
        };
        return opf.ShowDialog() == true ? opf.FileName : null;
    }

    public string? Open(string initialDirectory, string filter)
    {
        OpenFileDialog opf = new()
        {
            InitialDirectory = initialDirectory,
            Filter = filter
        };
        return opf.ShowDialog() == true ? opf.FileName : null;
    }

    public IEnumerable<string>? OpenText()
    {
        OpenFileDialog opf = new()
        {
            InitialDirectory = AppContext.BaseDirectory,
            Filter = defaultFilter
        };
        return opf.ShowDialog() == true ? File.ReadAllLines(opf.FileName) : null;
    }

    public string? Save()
    {
        SaveFileDialog sfd = new()
        {
            InitialDirectory = AppContext.BaseDirectory,
            Filter = defaultFilter
        };
        return sfd.ShowDialog() == true ? sfd.FileName : null;
    }

    public string? Save(string filter)
    {
        SaveFileDialog sfd = new()
        {
            InitialDirectory = AppContext.BaseDirectory,
            Filter = filter
        };
        return sfd.ShowDialog() == true ? sfd.FileName : null;
    }

    public string? Save(string initialDirectory, string filter)
    {
        SaveFileDialog sfd = new()
        {
            InitialDirectory = initialDirectory,
            Filter = filter
        };
        return sfd.ShowDialog() == true ? sfd.FileName : null;
    }

    public void SaveText(string contents)
    {
        SaveFileDialog sfd = new()
        {
            InitialDirectory = AppContext.BaseDirectory,
            Filter = defaultFilter
        };
        string? file = sfd.ShowDialog() == true ? sfd.FileName : null;
        if (string.IsNullOrEmpty(file))
            return;

        File.WriteAllText(file, contents);
    }

    public void SaveText(IEnumerable<string> contents)
    {
        SaveFileDialog sfd = new()
        {
            InitialDirectory = AppContext.BaseDirectory,
            Filter = defaultFilter
        };
        string? file = sfd.ShowDialog() == true ? sfd.FileName : null;
        if (string.IsNullOrEmpty(file))
            return;

        File.WriteAllLines(file, contents);
    }
}
