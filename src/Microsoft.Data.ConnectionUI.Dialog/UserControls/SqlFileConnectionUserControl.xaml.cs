using System.Windows.Controls;

namespace Microsoft.Data.ConnectionUI.Dialog.UserControls;

internal partial class SqlServerFileConnectionUserControl : UserControl, IDataConnectionControl
{
    public SqlServerFileConnectionUserControl()
    {
        InitializeComponent();
    }

    public string GetConnectionString()
    {
        throw new NotImplementedException();
    }

    public void Initialize(IDataConnectionProperties connectionProperties)
    {
        throw new NotImplementedException();
    }

    public void LoadProperties()
    {
        throw new NotImplementedException();
    }
}
