using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsoft.Data.ConnectionUI.Dialog.UserControls;
/// <summary>
/// Interaction logic for SqlServerConnectionUserControl.xaml
/// </summary>
public partial class SqlServerConnectionUserControl : UserControl, ISqlServerConnectionControl
{
    public SqlServerConnectionUserControl()
    {
        InitializeComponent();
    }

    public string GetConnectionString()
    {
        return string.Empty;
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
