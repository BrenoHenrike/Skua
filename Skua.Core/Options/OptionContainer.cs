using CommunityToolkit.Mvvm.ComponentModel;
using Skua.Core.Interfaces;
using Skua.Core.ViewModels;

namespace Skua.Core.Options;
public class OptionContainer : ObservableObject, IOptionContainer
{
    private readonly IDialogService _dialogService;

    public List<IOption> Options { get; } = new List<IOption>();

    public Dictionary<string, List<IOption>> MultipleOptions { get; } = new Dictionary<string, List<IOption>>();

    public Dictionary<IOption, string> OptionValues { get; } = new Dictionary<IOption, string>();
    public virtual string OptionsFile { get; set; } = string.Empty;

    public OptionContainer(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public void SetDefaults()
    {
        Options.ForEach(o => OptionValues[o] = o.DefaultValue?.ToString() ?? string.Empty);
        foreach (KeyValuePair<string, List<IOption>> kvp in MultipleOptions)
            foreach (IOption option in kvp.Value)
                OptionValues[option] = option.DefaultValue?.ToString() ?? string.Empty;
    }

    public T? Get<T>(string name) where T : IConvertible
    {
        return Get<T>(Options.Find(o => o.Name == name));
    }

    public T? Get<T>(string category, string name) where T : IConvertible
    {

        return category == "Options" ? Get<T>(name) : Get<T>(MultipleOptions[category].Find(o => o.Name == name));
    }


    public T? Get<T>(IOption? option) where T : IConvertible
    {
        if (option is null)
            return default;

        if (OptionValues.TryGetValue(option, out string? value))
        {
            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), value.Replace(' ', '_'));
            return (T)Convert.ChangeType(value, typeof(T));
        }

        return default;
    }

    public string GetDirect(IOption? option)
    {
        return option is null ? string.Empty : OptionValues[option];
    }

    public void Set(string name, object value)
    {
        Set(Options.Find(o => o.Name == name), value);
    }

    public void Set(string category, string name, object value)
    {
        Set(MultipleOptions[category].Find(o => o.Name == name), value);
    }

    public void Set<T>(IOption? option, T value)
    {
        if (option is null)
            return;
        OptionValues[option] = value?.ToString() ?? string.Empty;
        if (!option.Transient)
            Save();
    }


    public void Configure()
    {
        SetDefaults();
        Load();
        _dialogService.ShowDialog<OptionContainerViewModel>(new(this), SaveOptions);
    }

    private void SaveOptions(OptionContainerViewModel vm)
    {
        foreach (OptionContainerItemViewModel optionViewModel in vm.Options)
            Set(optionViewModel.Option, optionViewModel.Type.IsEnum ? optionViewModel.SelectedValue : optionViewModel.Value);
    }

    public void Load()
    {
        if (File.Exists(OptionsFile))
        {
            foreach (string line in File.ReadLines(OptionsFile))
            {
                string[] parts = line.Trim().Split(new char[] { ':', '=' }, 3);
                if (parts.Length == 3)
                {
                    IOption? option = parts[0] switch
                    {
                        "Options" => Options.Find(o => o.Name == parts[1]),
                        _ => MultipleOptions[parts[0]].Find(o => o.Name == parts[1]),
                    };
                    if (option is not null)
                        OptionValues[option] = parts[2];
                }
                if (parts.Length == 2)
                {
                    IOption? option = Options.Find(o => o.Name == parts[0]);
                    if (option is not null)
                        OptionValues[option] = parts[1];
                }
            }
            OnPropertyChanged(nameof(MultipleOptions));
            OnPropertyChanged(nameof(OptionValues));
            OnPropertyChanged(nameof(Options));
        }
    }

    public void Save()
    {
        List<string> linesToSave = new();
        linesToSave.AddRange(Options.Where(o => !o.Transient).Select(o => $"Options:{o.Name}={OptionValues[o]}"));
        foreach (KeyValuePair<string, List<IOption>> item in MultipleOptions)
            linesToSave.AddRange(item.Value.Where(o => !o.Transient).Select(o => $"{item.Key}:{o.Name}={OptionValues[o]}"));
        File.WriteAllLines(OptionsFile, linesToSave);
    }
}
