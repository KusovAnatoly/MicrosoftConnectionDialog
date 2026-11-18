using Microsoft.Data.ConnectionUI.Dialog;
using Microsoft.Data.DataConnectionConfiguration;

namespace Microsoft.Data.ConnectionUI;

public class DataConnectionDialogManager
{
    private readonly IDataConnectionConfiguration _config;

    private DataConnectionDialogManager(IDataConnectionConfiguration config)
    {
        _config = config;
    }

    private string? Execute()
    {
        return DataConnectionDialog.Show(_config);
    }

    public static string? Show(IDataConnectionConfiguration config)
    {
        var runner = new DataConnectionDialogManager(config);
        return runner.Execute();
    }
}