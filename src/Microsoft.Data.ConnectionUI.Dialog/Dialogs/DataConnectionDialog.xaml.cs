using Microsoft.Data.ConnectionUI.Dialog.ViewModels;
using Microsoft.Data.DataConnectionConfiguration;
using System.Windows;

namespace Microsoft.Data.ConnectionUI.Dialog;

internal partial class DataConnectionDialog : Window
{
    internal DataConnectionViewModel ViewModel { get; private set; }
    private DataConnectionDialog(DataConnectionViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
    }

    private new string ShowDialog()
    {
        base.ShowDialog();
        if (ViewModel.ConnectionStringInitialized)
        {
            return ViewModel.ConnectionString;
        }
        throw new Exception("User did not set up connection string");
    }

    internal static string ShowDialog(IDataConnectionConfiguration config)
    {
        var viewModel = new DataConnectionViewModel(config);
        var window = new DataConnectionDialog(viewModel);
        return window.ShowDialog();
    }

    private void Button_Click(object sender, RoutedEventArgs e) => Close();
}