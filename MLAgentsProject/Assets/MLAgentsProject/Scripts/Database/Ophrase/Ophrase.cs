using System.Threading.Tasks;
using System.Collections.Generic;

public class Ophrase : BaseProcess<Ophrase, string>
{
    private static Ophrase _instance;
    public static Ophrase Instance => _instance ??= new Ophrase();

    protected override string GetBatchFileName() => "ophrase.bat";

    public async Task<List<string>> Paraphrase(string inputString)
    {
        string[] args = { inputString };
        return await Execute(args);
    }
}
