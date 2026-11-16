using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Microsoft.Data.ConnectionUI.Dialog.ViewModels;

internal partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        DataSources = new ObservableCollection<DataSource>
        {
            DataSource.SqlDataSource,
            DataSource.SqlFileDataSource,
            DataSource.OracleDataSource,
            DataSource.AccessDataSource,
            DataSource.OdbcDataSource,
        };
        UnspecifiedDataSource = DataSource.CreateUnspecified();
        DataSource.AddProviders(UnspecifiedDataSource);
        DataSources.Add(UnspecifiedDataSource);
    }

    public ObservableCollection<DataSource> DataSources { get; set; }

    public DataSource UnspecifiedDataSource { get; set; }
}
