using CommandTerminal;

public static class DataAuditAction
{
    public static void Execute(CommandArg[] args)
    {
        if (args.Length < 1)
        {
            Log.Error("No file specified for data audit.");
            return;
        }

        string fileName = args[0].String;
        DataAuditor.Audit(fileName);
    }
}
