using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using System.Text;

namespace Skua.Core.ViewModels;
public partial class CBOClassEquipmentViewModel : ObservableObject, IManageCBOptions
{
    public CBOClassEquipmentViewModel(IScriptInventory inventory)
    {
        _inventory = inventory;
    }

    private readonly IScriptInventory _inventory;

    public List<string> Helms {get; private set; } = new();
    public List<string> Armors {get; private set; } = new();
    public List<string> Capes {get; private set; } = new();
    public List<string> Weapons {get; private set; } = new();
    public List<string> Pets {get; private set; } = new();
    public List<string> GroundItems { get; private set; } = new();

    [ObservableProperty]
    private string? _selectedFarmHelm;
    [ObservableProperty]
    private string? _selectedFarmArmor;
    [ObservableProperty]
    private string? _selectedFarmCape;
    [ObservableProperty]
    private string? _selectedFarmWeapon;
    [ObservableProperty]
    private string? _selectedFarmPet;
    [ObservableProperty]
    private string? _selectedFarmGroundItem;

    [ObservableProperty]
    private string? _selectedSoloHelm;
    [ObservableProperty]
    private string? _selectedSoloArmor;
    [ObservableProperty]
    private string? _selectedSoloCape;
    [ObservableProperty]
    private string? _selectedSoloWeapon;
    [ObservableProperty]
    private string? _selectedSoloPet;
    [ObservableProperty]
    private string? _selectedSoloGroundItem;

    [RelayCommand]
    private void RefreshInventory()
    {
        Helms = _inventory.Items?.Where(i => i.Category == ItemCategory.Helm && i.EnhancementLevel > 0).Select(i => i.Name).ToList() ?? new();
        Armors = _inventory.Items?.Where(i => i.Category == ItemCategory.Armor).Select(i => i.Name).ToList() ?? new();
        Capes = _inventory.Items?.Where(i => i.Category == ItemCategory.Cape && i.EnhancementLevel > 0).Select(i => i.Name).ToList() ?? new();
        Weapons = _inventory.Items?.Where(i => i.ItemGroup == "Weapon" && i.EnhancementLevel > 0).Select(i => i.Name).ToList() ?? new();
        Pets = _inventory.Items?.Where(i => i.Category == ItemCategory.Pet).Select(i => i.Name).ToList() ?? new();
        GroundItems = _inventory.Items?.Where(i => i.Category == ItemCategory.Misc).Select(i => i.Name).ToList() ?? new();

        OnPropertyChanged(nameof(Helms));
        OnPropertyChanged(nameof(Armors));
        OnPropertyChanged(nameof(Capes));
        OnPropertyChanged(nameof(Weapons));
        OnPropertyChanged(nameof(Pets));
        OnPropertyChanged(nameof(GroundItems));
    }

    public StringBuilder Save(StringBuilder builder)
    {
        builder.AppendLine($"Helm1Select: {SelectedSoloHelm}");
        builder.AppendLine($"Armor1Select: {SelectedSoloArmor}");
        builder.AppendLine($"Cape1Select: {SelectedSoloCape}");
        builder.AppendLine($"Weapon1Select: {SelectedSoloWeapon}");
        builder.AppendLine($"Pet1Select: {SelectedSoloPet}");
        builder.AppendLine($"GroundItem1Select: {SelectedSoloGroundItem}");

        builder.AppendLine($"Helm2Select: {SelectedFarmHelm}");
        builder.AppendLine($"Armor2Select: {SelectedFarmArmor}");
        builder.AppendLine($"Cape2Select: {SelectedFarmCape}");
        builder.AppendLine($"Weapon2Select: {SelectedFarmWeapon}");
        builder.AppendLine($"Pet2Select: {SelectedFarmPet}");
        builder.AppendLine($"GroundItem2Select: {SelectedFarmGroundItem}");

        return builder;
    }

    public void SetValues(Dictionary<string, string> values)
    {
        if (!string.IsNullOrEmpty(SelectedSoloHelm = GetValue("Helm1Select")))
            Helms.Add(SelectedSoloHelm);
        if(!string.IsNullOrEmpty(SelectedSoloArmor = GetValue("Armor1Select")))
            Armors.Add(SelectedSoloArmor);
        if(!string.IsNullOrEmpty(SelectedSoloCape = GetValue("Cape1Select")))
            Capes.Add(SelectedSoloCape);
        if(!string.IsNullOrEmpty(SelectedSoloWeapon = GetValue("Weapon1Select")))
            Weapons.Add(SelectedSoloWeapon);
        if(!string.IsNullOrEmpty(SelectedSoloPet = GetValue("Pet1Select")))
            Pets.Add(SelectedSoloPet);
        if(!string.IsNullOrEmpty(SelectedSoloGroundItem = GetValue("GroundItem1Select")))
            GroundItems.Add(SelectedSoloGroundItem);

        if(!string.IsNullOrEmpty(SelectedFarmHelm = GetValue("Helm2Select")))
            Helms.Add(SelectedFarmHelm);
        if(!string.IsNullOrEmpty(SelectedFarmArmor = GetValue("Armor2Select")))
            Armors.Add(SelectedFarmArmor);
        if(!string.IsNullOrEmpty(SelectedFarmCape = GetValue("Cape2Select")))
            Capes.Add(SelectedFarmCape);
        if(!string.IsNullOrEmpty(SelectedFarmWeapon = GetValue("Weapon2Select")))
            Weapons.Add(SelectedFarmWeapon);
        if(!string.IsNullOrEmpty(SelectedFarmPet = GetValue("Pet2Select")))
            Pets.Add(SelectedFarmPet);
        if(!string.IsNullOrEmpty(SelectedFarmGroundItem = GetValue("GroundItem2Select")))
            GroundItems.Add(SelectedFarmGroundItem);

        string GetValue(string key) => values.TryGetValue(key, out string? value) ? value : string.Empty;
    }
}
