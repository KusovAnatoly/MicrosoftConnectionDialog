using System.Windows.Controls;

namespace Microsoft.Data.ConnectionUI.Dialog.UserControls;
/// <summary>
/// Interaction logic for OracleConnectionUserControl.xaml
/// </summary>
public partial class OracleConnectionUserControl : UserControl, IDataConnectionControl
{
    public OracleConnectionUserControl()
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
