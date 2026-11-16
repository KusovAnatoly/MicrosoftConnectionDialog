using Microsoft.Data.ConnectionUI.Dialog.Dialogs;
using Microsoft.Data.ConnectionUI.Dialog.ViewModels;
using System.Windows;

namespace Microsoft.Data.ConnectionUI.Dialog;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        new AddPropertyDialog().Show();
    }
}