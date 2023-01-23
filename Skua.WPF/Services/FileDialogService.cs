using System;
using System.Collections.Generic;
using Skua.Core.Interfaces;
using Microsoft.Win32;
using System.IO;
using Ookii.Dialogs.Wpf;

namespace Skua.WPF.Services;
public class FileDialogService : IFileDialogService
{
    private const string defaultFilter = "Text Files (*.txt)|*.txt";
    public string? OpenFile()
    {
        OpenFileDialog opf = new();
        opf.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua");
        return opf.ShowDialog() == true ? opf.FileName : null;
    }

    public string? OpenFile(string filter)
    {
        OpenFileDialog opf = new()
        {
            InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua"),
            Filter = filter
        };
        return opf.ShowDialog() == true ? opf.FileName : null;
    }

    public string? OpenFile(string initialDirectory, string filter)
    {
        OpenFileDialog opf = new()
        {
            InitialDirectory = initialDirectory,
            Filter = filter
        };
        return opf.ShowDialog() == true ? opf.FileName : null;
    }

    public string? OpenFolder(string initialDirectory)
    {
        VistaFolderBrowserDialog fbd = new()
        {
            Description = "Select the download folder.",
            Multiselect = false
        };
        return fbd.ShowDialog() == true ? fbd.SelectedPath : null;
    }

    public string? OpenFolder()
    {
        VistaFolderBrowserDialog fbd = new()
        {
            Description = "Select the download folder.",
            Multiselect = false
        };
        return fbd.ShowDialog() == true ? fbd.SelectedPath : null;
    }

    public IEnumerable<string>? OpenText()
    {
        OpenFileDialog opf = new()
        {
            InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua"),
            Filter = defaultFilter
        };
        return opf.ShowDialog() == true ? File.ReadAllLines(opf.FileName) : null;
    }

    public string? Save()
    {
        SaveFileDialog sfd = new()
        {
            InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua"),
            Filter = defaultFilter
        };
        return sfd.ShowDialog() == true ? sfd.FileName : null;
    }

    public string? Save(string filter)
    {
        SaveFileDialog sfd = new()
        {
            InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua"),
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
            InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua"),
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
            InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Skua"),
            Filter = defaultFilter
        };
        string? file = sfd.ShowDialog() == true ? sfd.FileName : null;
        if (string.IsNullOrEmpty(file))
            return;

        File.WriteAllLines(file, contents);
    }
}
