using Skua.Core.Interfaces.Skills;
using Skua.Core.Models.Skills;

namespace Skua.Core.Skills;

public class AdvancedSkillContainer : IAdvancedSkillContainer
{
    public List<AdvancedSkill> LoadedSkills { get; set; } = new List<AdvancedSkill>();
    public AdvancedSkillContainer()
    {
        LoadSkills();
    }

    public void Add(AdvancedSkill skill)
    {

    }
    public void Remove(AdvancedSkill skill)
    {

    }

    public void LoadSkills()
    {
        //string path = Path.Combine(AppContext.BaseDirectory, "Skills", "AdvancedSkills.txt");
        //lstSavedSkills.Items.Clear();
        //LoadedSkills.Clear();

        //if (!File.Exists(path))
        //    return;

        //File.ReadAllLines(path).ToList().ForEach(l =>
        //{
        //    string[] parts = l.Split(new[] { '=' }, 3);
        //    if (parts.Length == 2)
        //    {
        //        if (int.TryParse(Regex.Replace(parts[1].Split(':').Last(), "[^0-9.]", ""), out int result))
        //            LoadedSkills.Add(new AdvancedSkill(parts[0].Trim(), parts[1].Trim(), result));
        //        else
        //            LoadedSkills.Add(new AdvancedSkill(parts[0].Trim(), parts[1].Trim()));
        //    }
        //    else if (parts.Length == 3)
        //    {
        //        if (parts[2].Contains("timeout", StringComparison.OrdinalIgnoreCase) && int.TryParse(Regex.Replace(parts[2].Split(':').Last(), "[^0-9.]", ""), out int result))
        //            LoadedSkills.Add(new AdvancedSkill(parts[1].Trim(), parts[2].Trim(), result, parts[0].Trim()));
        //        else
        //            LoadedSkills.Add(new AdvancedSkill(parts[1].Trim(), parts[2].Trim(), 1, parts[0].Trim()));
        //    }
        //});
        //lstSavedSkills.Items.AddRange(LoadedSkills.ToArray());
    }

    public void Save()
    {
        //if (txtSaveName.Text.Length == 0)
        //    return;

        //if (lstSkillSequence.Items.Count > 0)
        //    Convert();

        //string path = Path.Combine(AppContext.BaseDirectory, "Skills", "AdvancedSkills.txt");
        //string mode = cbModes.SelectedIndex == -1 ? "Base" : cbModes.SelectedItem.ToString();
        //if (chkOverride.Checked && File.Exists(path))
        //{
        //    List<string> savedSkill = File.ReadAllLines(path).ToList();

        //    int index = savedSkill.FindIndex(l => l.Contains(txtSaveName.Text));
        //    if (index != -1)
        //    {
        //        savedSkill[index] = $"{mode} = {txtSaveName.Text} = {txtSkillString.Text}";
        //        File.WriteAllLines(path, savedSkill);
        //    }
        //    else
        //        File.AppendAllLines(path, new[] { $"{mode} = {txtSaveName.Text} = {txtSkillString.Text}" });
        //}
        //else
        //    File.AppendAllLines(path, new[] { $"{mode} = {txtSaveName.Text} = {txtSkillString.Text}" });
        //LoadSkills();
    }

    public AdvancedSkill ConvertFromString(string skillString)
    {
        return new();
        //List<SkillListBoxObj> list = new();
        //List<string> skills = skillString.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        //bool timeoutChanged = false, modeChanged = false;
        //skills.ForEach(s =>
        //{
        //    if (s.Contains("Timeout"))
        //    {
        //        int.TryParse(Regex.Replace(s, "[^0-9.]", ""), out int result);
        //        numSkillTimeout.Value = result;
        //        timeoutChanged = true;
        //    }
        //    else if (s.Contains("Mode"))
        //    {
        //        if (s.Trim().Split(' ').Last() == "Optimistic")
        //            rbOptMode.Checked = true;
        //        modeChanged = true;
        //    }
        //    else
        //        list.Add(new SkillListBoxObj(s.Trim()));
        //});
        //if (!timeoutChanged)
        //    numSkillTimeout.Value = -1;
        //if (!modeChanged)
        //    rbWaitMode.Checked = true;
        //return list;
    }
}
