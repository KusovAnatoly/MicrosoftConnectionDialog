using System.Windows.Controls;

namespace Microsoft.Data.ConnectionUI.Dialog.UserControls;

/// <summary>
/// Interaction logic for AccessConnectionUserControl.xaml
/// </summary>
public partial class AccessConnectionUserControl : UserControl, IAccessConnectionControl
{
    public AccessConnectionUserControl()
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
