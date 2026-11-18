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
/// Interaction logic for OleDBConnectionUserControl.xaml
/// </summary>
public partial class OleDBConnectionUserControl : UserControl, IDataConnectionControl
{
    public OleDBConnectionUserControl()
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
