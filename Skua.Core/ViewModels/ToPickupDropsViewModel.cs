using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;

namespace Skua.Core.ViewModels;
public partial class ToPickupDropsViewModel : ObservableObject, IDisposable
{
    private readonly char[] _dropsSeparator = { '|' };
    public ToPickupDropsViewModel(IScriptDrop drops, IScriptOption options)
    {
        Drops = drops;
        Options = options;
        Drops.PropertyChanged += Drops_PropertyChanged;
        ToggleDropsCommand = new AsyncRelayCommand(ToggleDrops);
        RemoveDropsCommand = new RelayCommand<IList<object>>(RemoveDrops);
        RemoveAllDropsCommand = new RelayCommand(Drops.Clear);
        AddDropCommand = new RelayCommand(AddDrop);
    }
    [ObservableProperty]
    private string _addDropInput = string.Empty;
    public List<string> ToPickup => Drops.ToPickup.ToList();
    public IScriptDrop Drops { get; }
    public IScriptOption Options { get; }
    public IRelayCommand ToggleDropsCommand { get; }
    public IRelayCommand AddDropCommand { get; }
    public IRelayCommand RemoveAllDropsCommand { get; }
    public IRelayCommand RemoveDropsCommand { get; }

    private void RemoveDrops(IList<object>? items)
    {
        if (items is null)
            return;
        IEnumerable<string> drops = items.Cast<string>();
        if (drops.Any())
            Drops.Remove(drops.ToArray());
    }

    private async Task ToggleDrops()
    {
        if (Drops.Enabled)
            await Drops.StopAsync();
        else
            Drops.Start();
    }

    private void AddDrop()
    {
        if (string.IsNullOrWhiteSpace(AddDropInput))
            return;

        IEnumerable<string> drops = AddDropInput.Split(_dropsSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (drops.Any())
            Drops.Add(drops.ToArray());

        AddDropInput = string.Empty;
    }

    private void Drops_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IScriptDrop.ToPickup))
            OnPropertyChanged(nameof(ToPickup));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Drops.PropertyChanged -= Drops_PropertyChanged;
    }
}
