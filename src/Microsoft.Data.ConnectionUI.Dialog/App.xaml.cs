using System.Windows;

namespace Microsoft.Data.ConnectionUI.Dialog;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var config = new DataConnectionConfiguration.DataConnectionConfiguration(string.Empty);
        DataConnectionDialogManager.Show(config);
    }
}
