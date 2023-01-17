using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.Models.Skills;
using Skua.Core.Utils;

namespace Skua.Core.Skills;

public class AdvancedSkillContainer : ObservableRecipient, IAdvancedSkillContainer
{
    private List<AdvancedSkill> _loadedSkills = new();
    private readonly string _defaultSkillsSetsPath;
    private readonly string _userSkillsSetsPath;
    
    public List<AdvancedSkill> LoadedSkills
    {
        get { return _loadedSkills; }
        set { SetProperty(ref _loadedSkills, value, true); }
    }
    
    public AdvancedSkillContainer()
    {
        _defaultSkillsSetsPath = Path.Combine(AppContext.BaseDirectory, "AdvancedSkills.txt");
        _userSkillsSetsPath = Path.Combine(AppContext.BaseDirectory, "UserAdvancedSkills.txt");
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
        await Task.Factory.StartNew(() =>
        {
            _CopyDefaultSkills();
            LoadSkills();
        });
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
        Task.Factory.StartNew(() =>
        {
            File.WriteAllLines(_userSkillsSetsPath, _loadedSkills.OrderBy(s => s.ClassName).Select(s => s.SaveString));
            LoadSkills();
        });
    }
}
