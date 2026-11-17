using Microsoft.Data.ConnectionUI.Dialog.UserControls;
using System.Windows.Controls;

namespace Microsoft.Data.ConnectionUI.Dialog;

public static class UiMapper
{
    public static UserControl GetUserControl(IDataConnectionControl control)
    {
        return control switch
        {
            IAccessConnectionControl => new AccessConnectionUserControl(),
            IOleDBConnectionControl => new OleDBConnectionUserControl(),
            IOdbcConnectionControl => new OdbcConnectionUserControl(),
            IOracleConnectionControl => new OracleConnectionUserControl(),
            ISqlServerConnectionControl => new SqlConnectionUserControl(),
            ISqlServerFileConnectionControl => new SqlFileConnectionUserControl(),
            _ => new OdbcConnectionUserControl()
        };
    }
}