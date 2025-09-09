using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Skua.Core.Interfaces;
using Skua.Core.Scripts;
using Skua.Core.Utils;
using Skua.Core.ViewModels;
using System.Reflection;

namespace Skua.Core.AppStartup;

internal class Options
{
    internal static GameOptionsViewModel CreateGameOptions(IServiceProvider s)
    {
        IScriptOption scriptOpt = s.GetService<IScriptOption>()!;

        List<DisplayOptionItemViewModelBase> options = new()
        {
            new CommandOptionItemViewModel<bool>("Upgrade", "1", CreateBoolCommand(b => Ioc.Default.GetRequiredService<IFlashUtil>().SetGameObject("world.myAvatar.pMC.pname.ti.textColor", b ? 0x8CD5FF : 0xFFFFFF))),
            new CommandOptionItemViewModel<bool>("Staff", "1", CreateBoolCommand(b => Ioc.Default.GetRequiredService<IFlashUtil>().SetGameObject("world.myAvatar.pMC.pname.ti.textColor", b ? 0xFECB38 : 0xFFFFFF))),
            new CommandOptionItemViewModel<IRelayCommand>("Reload Map", "4", new RelayCommand(Ioc.Default.GetRequiredService<IScriptMap>().Reload))
        };

        foreach (PropertyInfo pi in scriptOpt.GetType().GetProperties())
        {
            if (pi.Name is nameof(IScriptOption.OptionDictionary) or nameof(ObservableRecipient.IsActive) or nameof(IScriptOption.GuildColor) or nameof(IScriptOption.NameColor))
                continue;
            if (pi.PropertyType == typeof(bool))
                options.Add(CreateOptionItem<bool>(s, pi.Name, "1", new RelayCommand<bool>(b => pi.SetValue(scriptOpt, b))));
            else if (pi.PropertyType == typeof(string))
                options.Add(CreateOptionItem<string>(s, pi.Name, "2", new RelayCommand<string>(value => pi.SetValue(scriptOpt, value))));
            else if (pi.PropertyType == typeof(int))
            {
                // Special handling for SetFPS to add "FPS" suffix
                string? suffixText = pi.Name == nameof(IScriptOption.SetFPS) ? "FPS" : null;
                options.Add(CreateOptionItemWithSuffix<int>(s, pi.Name, "3", suffixText, new RelayCommand<string>(value =>
                {
                    if (!int.TryParse(value, out int result))
                        return;
                    pi.SetValue(scriptOpt, result);
                })));
            }
        }

        return new GameOptionsViewModel(options, s.GetService<IScriptServers>()!, scriptOpt);

        static RelayCommand<bool> CreateBoolCommand(Action<bool> action)
        {
            return new RelayCommand<bool>(action);
        }
        static BindingOptionItemViewModel<T, ScriptOption> CreateOptionItem<T>(IServiceProvider s, string binding, string tag, IRelayCommand command)
        {
            return new BindingOptionItemViewModel<T, ScriptOption>(s.GetRequiredService<IDecamelizer>().Decamelize(binding, null), tag, binding, (ScriptOption)s.GetRequiredService<IScriptOption>(), command);
        }
        static BindingOptionItemViewModel<T, ScriptOption> CreateOptionItemWithSuffix<T>(IServiceProvider s, string binding, string tag, string? suffixText, IRelayCommand command)
        {
            var item = new BindingOptionItemViewModel<T, ScriptOption>(s.GetRequiredService<IDecamelizer>().Decamelize(binding, null), tag, binding, (ScriptOption)s.GetRequiredService<IScriptOption>(), command);
            item.SuffixText = suffixText;
            return item;
        }
    }

    internal static ApplicationOptionsViewModel CreateAppOptions(IServiceProvider s)
    {
        List<DisplayOptionItemViewModelBase> appOptions = new()
        {
            CreateSettingOptionItem<bool>("Auto Update Scripts", "Whether to auto update scripts when launching the Manager, needs \"Check for Scripts Updates\" to be true", "AutoUpdateBotScripts"),
            CreateSettingOptionItem<bool>("Check for Script Updates", "Whether to check for scripts updates when launching the Manager", "CheckBotScriptsUpdates"),
            CreateSettingOptionItem<bool>("Auto Update AdvanceSkill Sets", "Whether to auto update advance skill sets when launching the Manager, needs \"Check for AdvanceSkill Sets updates\" to be true", "AutoUpdateAdvanceSkillSetsUpdates"),
            CreateSettingOptionItem<bool>("Check for AdvanceSkill Sets Updates", "Whether to check for scripts updates when launching the Manager", "CheckAdvanceSkillSetsUpdates"),
            new CommandOptionItemViewModel<int>("* Client Animation Framerate", "Client side animation framerate setting", "ClientAnim", "FPS", new RelayCommand<string>(value =>
            {
                if (!int.TryParse(value, out int result) || result < 1)
                    return;
                Ioc.Default.GetRequiredService<ISettingsService>().Set("AnimationFrameRate", result);
            }), s.GetRequiredService<ISettingsService>().Get<int>("AnimationFrameRate")),
            new CommandOptionItemViewModel<IRelayCommand>("Clear Flash Cache", new RelayCommand(() =>
            {
                string flash = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Adobe", "Flash Player");
                string macromedia = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Macromedia", "Flash Player");

                if (Directory.Exists(flash))
                    Directory.Delete(flash, true);
                if (Directory.Exists(macromedia))
                    Directory.Delete(macromedia, true);
            }))
        };

        return new ApplicationOptionsViewModel(appOptions);

        static RelayCommand<T> CreateSettingCommand<T>(string key)
        {
            return new RelayCommand<T>(b => Ioc.Default.GetRequiredService<ISettingsService>().Set(key, b));
        }
        static CommandOptionItemViewModel<T> CreateSettingOptionItem<T>(string content, string description, string key)
        {
            return new(content, description, key, CreateSettingCommand<T>(key), Ioc.Default.GetRequiredService<ISettingsService>().Get<T>(key));
        }
    }
}