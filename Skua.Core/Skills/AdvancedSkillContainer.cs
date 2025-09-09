using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Skills;
using Skua.Core.Utils;

namespace Skua.Core.Skills;

public class AdvancedSkillContainer : ObservableRecipient, IAdvancedSkillContainer, IDisposable
{
    private List<AdvancedSkill> _loadedSkills = new();
    private readonly string _defaultSkillsSetsPath;
    private readonly string _userSkillsSetsPath;
    private CancellationTokenSource? _saveCts;
    private Task? _saveTask;

    public List<AdvancedSkill> LoadedSkills
    {
        get { return _loadedSkills; }
        set { SetProperty(ref _loadedSkills, value, true); }
    }

    public AdvancedSkillContainer()
    {
        _defaultSkillsSetsPath = ClientFileSources.SkuaAdvancedSkillsFile;
        _userSkillsSetsPath = Path.Combine(ClientFileSources.SkuaDIR, "UserAdvancedSkills.txt");

        var rootDefaultSkills = Path.Combine(AppContext.BaseDirectory, "AdvancedSkills.txt");
        if (File.Exists(rootDefaultSkills) && !File.Exists(_defaultSkillsSetsPath))
        {
            File.Copy(rootDefaultSkills, _defaultSkillsSetsPath, true);
        }
        LoadSkills();
    }

    public void Add(AdvancedSkill skill)
    {
        _loadedSkills.Add(skill);
        Save();
    }

    public void Remove(AdvancedSkill skill)
    {
        _loadedSkills.Remove(skill);
        Save();
    }

    public void TryOverride(AdvancedSkill skill)
    {
        if (!_loadedSkills.Contains(skill))
        {
            _loadedSkills.Add(skill);
            Save();
            return;
        }

        int index = _loadedSkills.IndexOf(skill);
        _loadedSkills[index] = skill;
        Save();
    }

    private void _CopyDefaultSkills()
    {
        var getScripts = Ioc.Default.GetRequiredService<IGetScriptsService>();
        if (!File.Exists(_defaultSkillsSetsPath))
            getScripts.UpdateSkillSetsFile().GetAwaiter().GetResult();

        if (File.Exists(_userSkillsSetsPath))
            File.Delete(_userSkillsSetsPath);

        File.Copy(_defaultSkillsSetsPath, _userSkillsSetsPath);
    }

    public async void SyncSkills()
    {
        _saveCts?.Cancel();
        await (_saveTask ?? Task.CompletedTask);
        _saveCts?.Dispose();
        _saveCts = new CancellationTokenSource();

        await Task.Factory.StartNew(() =>
        {
            _CopyDefaultSkills();
            LoadSkills();
        }, _saveCts.Token);
    }

    public void LoadSkills()
    {
        if (!File.Exists(_userSkillsSetsPath))
            _CopyDefaultSkills();

        LoadedSkills.Clear();
        foreach (string line in File.ReadAllLines(_userSkillsSetsPath))
        {
            string[] parts = line.Split(new[] { '=' }, 4);
            if (parts.Length == 3)
                _loadedSkills.Add(new AdvancedSkill(parts[1].Trim(), parts[2].Trim(), 250, parts[0].Trim(), "WaitForCooldown"));
            else if (parts.Length == 4)
            {
                bool waitForCooldown = int.TryParse(parts[3].RemoveLetters(), out int result);
                _loadedSkills.Add(new AdvancedSkill(parts[1].Trim(), parts[2].Trim(), waitForCooldown ? result : 250, parts[0].Trim(), waitForCooldown ? SkillUseMode.WaitForCooldown : SkillUseMode.UseIfAvailable));
            }
        }

        OnPropertyChanged(nameof(LoadedSkills));
        Broadcast(new(), _loadedSkills, nameof(LoadedSkills));
    }

    public void ResetSkillsSets()
    {
        SyncSkills();
    }

    public void Save()
    {
        _saveCts?.Cancel();
        _saveCts?.Dispose();
        _saveCts = new CancellationTokenSource();

        _saveTask = Task.Factory.StartNew(() =>
        {
            try
            {
                File.WriteAllLines(_userSkillsSetsPath, _loadedSkills.OrderBy(s => s.ClassName).Select(s => s.SaveString));
                if (!_saveCts.Token.IsCancellationRequested)
                {
                    LoadSkills();
                }
            }
            catch
            {
                // Handle save errors gracefully
            }
        }, _saveCts.Token);
    }

    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _saveCts?.Cancel();
                try
                {
                    _saveTask?.Wait(1000);
                }
                catch { }
                _saveCts?.Dispose();
                _loadedSkills.Clear();
            }

            _disposed = true;
        }
    }

    ~AdvancedSkillContainer()
    {
        Dispose(false);
    }
}