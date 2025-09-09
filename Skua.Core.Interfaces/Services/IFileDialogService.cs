namespace Skua.Core.Interfaces;

public interface IFileDialogService
{
    string? OpenFile();

    string? OpenFile(string filters);

    string? OpenFile(string initialDirectory, string filters);

    string? OpenFolder();

    string? OpenFolder(string initialDirectory);

    IEnumerable<string>? OpenText();

    string? Save();

    string? Save(string filters);

    string? Save(string initialDirectory, string filters);

    void SaveText(string contents);

    void SaveText(IEnumerable<string> contents);
}