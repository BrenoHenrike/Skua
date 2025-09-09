using Skua.Core.ViewModels.Manager;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace Skua.WPF.Views
{
    /// <summary>
    /// Interaction logic for ScriptRepoManagerView.xaml
    /// </summary>
    public partial class ScriptRepoManagerView : UserControl
    {
        public ScriptRepoManagerView()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            CollectionViewSource.GetDefaultView(ScriptsDataGrid.ItemsSource).Filter = item =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                    return true;

                if (item is ScriptInfoManagerViewModel scriptItem)
                {
                    return scriptItem.Info.Name.Contains(textBox.Text, StringComparison.OrdinalIgnoreCase)
                        || scriptItem.Info.Description.Contains(textBox.Text, StringComparison.OrdinalIgnoreCase)
                        || scriptItem.Info.RelativePath.Contains(textBox.Text, StringComparison.OrdinalIgnoreCase)
                        || scriptItem.InfoTags.Any(t => t.Contains(textBox.Text, StringComparison.OrdinalIgnoreCase));
                }

                return false;
            };
        }
    }
}