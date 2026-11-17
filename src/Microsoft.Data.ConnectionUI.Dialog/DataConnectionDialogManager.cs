using Microsoft.Data.ConnectionUI.Dialog;

namespace Microsoft.Data.ConnectionUI;

public class DataConnectionDialogManager
{
    private readonly DataConnectionConfiguration.IDataConnectionConfiguration _config;

    private DataConnectionDialogManager(DataConnectionConfiguration.IDataConnectionConfiguration config)
    {
        _config = config;
    }

    public static string? Show(DataConnectionConfiguration.IDataConnectionConfiguration config)
    {
        var runner = new DataConnectionDialogManager(config);
        return runner.Execute();
    }

    private string? Execute()
    {
        return DataConnectionDialog.ShowDialog(_config);
    }
}