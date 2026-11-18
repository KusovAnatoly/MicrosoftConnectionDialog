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

    private new string? Show()
    {
        base.Show();
        if (ViewModel.ConnectionStringInitialized)
        {
            return ViewModel.ConnectionString;
        }
        return null;
    }

    internal static string? Show(IDataConnectionConfiguration config)
    {
        var viewModel = new DataConnectionViewModel(config);
        var window = new DataConnectionDialog(viewModel);
        return window.Show();
    }

    private void Button_Click(object sender, RoutedEventArgs e) => Close();

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        Hide();
    }
}