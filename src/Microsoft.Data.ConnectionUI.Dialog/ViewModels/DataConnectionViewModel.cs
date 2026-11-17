using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace Microsoft.Data.ConnectionUI.Dialog.ViewModels;

public partial class DataConnectionViewModel : ObservableObject
{
    public ObservableCollection<DataSource> DataSources { get; set; }

    [ObservableProperty]
    public partial DataSource SelectedDataSource { get; set; }

    [ObservableProperty]
    public partial DataProvider SelectedDataProvider { get; set; }

    [ObservableProperty]
    public partial DataSource UnspecifiedDataSource { get; set; }

    public DataConnectionViewModel(DataConnectionConfiguration.IDataConnectionConfiguration config)
    {
        DataSources = new ObservableCollection<DataSource>(config.GetDataSources());
        SelectedDataSource = DataSources.FirstOrDefault();
    }

    partial void OnSelectedDataSourceChanged(DataSource value)
    {
        SelectedDataProvider = value.Providers.FirstOrDefault();
    }

    [RelayCommand]
    private void OpenConnectionProperties()
    {
        var connectionControl = SelectedDataProvider.CreateConnectionControl(SelectedDataSource);
        var window = new Window();
        window.Content = UiMapper.GetUserControl(connectionControl);
        window.Show();
    }
}
