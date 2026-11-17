using System.Windows.Controls;

namespace Microsoft.Data.ConnectionUI.Dialog.UserControls;

internal partial class SqlFileConnectionUserControl : UserControl, ISqlServerFileConnectionControl
{
    public SqlFileConnectionUserControl()
    {
        InitializeComponent();
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
