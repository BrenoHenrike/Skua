using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.DependencyInjection;
using Skua.Core.ViewModels;

namespace Skua.WPF.Views;
/// <summary>
/// Interaction logic for ScriptRepoView.xaml
/// </summary>
public partial class ScriptRepoView : UserControl
{
    private readonly ICollectionView _collectionView;
    public ScriptRepoView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<ScriptRepoViewModel>();
        _collectionView = CollectionViewSource.GetDefaultView(((ScriptRepoViewModel)DataContext).Scripts);
    }

    private bool Search(object obj)
    {
        var flag = false;
        var searchScript = SearchBox.Text;
        if (string.IsNullOrWhiteSpace(searchScript))
            return true;

        var script = (ScriptInfoViewModel)obj;
        if (script is null)
            return false;

        var scriptName = script.Info.Name;
        if (KMPSearch(scriptName, searchScript))
            flag = true;

        foreach (var tag in script.InfoTags)
        {
            if (KMPSearch(tag, searchScript))
            {
                flag = true;
                break;
            }
        }

        return flag;
    }

    private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        await Task.Run(async () =>
        {
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                _collectionView.Filter = Search;
                _collectionView.Refresh();
            }));
        });
    }

    private bool KMPSearch(string text, string pattern)
    {
        int n = text.Length;
        int m = pattern.Length;
        int[] lps = new int[m];
        int j = 0; // index for pattern[]

        // Preprocess the pattern (calculate lps[] array)
        ComputeLPSArray(pattern, m, lps);

        int i = 0;  // index for text[]
        while (i < n)
        {
            if (pattern[j] == text[i])
            {
                j++;
                i++;
            }

            if (j == m)
                return true;

            // mismatch after j matches
            else if (i < n && pattern[j] != text[i])
            {
                // Do not match lps[0..lps[j-1]] characters,
                // they will match anyway
                if (j != 0)
                    j = lps[j - 1];
                else
                    i = i + 1;
            }
        }
        return false;
    }

    private void ComputeLPSArray(string pattern, int m, int[] lps)
    {
        int len = 0;
        int i = 1;
        lps[0] = 0; // lps[0] is always 0

        // the loop calculates lps[i] for i = 1 to m-1
        while (i < m)
        {
            if (pattern[i] == pattern[len])
            {
                len++;
                lps[i] = len;
                i++;
            }
            else // (pat[i] != pat[len])
            {
                if (len != 0)
                {
                    len = lps[len - 1];

                    // Also, note that we do not increment i here
                }
                else  // if (len == 0)
                {
                    lps[i] = 0;
                    i++;
                }
            }
        }
    }
}
