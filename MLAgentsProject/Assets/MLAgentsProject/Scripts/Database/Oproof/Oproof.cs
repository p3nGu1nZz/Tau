using System.Threading.Tasks;
using System.Collections.Generic;

public class Oproof : BaseProcess<Oproof, Response>
{
    private static Oproof _instance;
    public static Oproof Instance => _instance ??= new Oproof();

    protected override string GetBatchFileName() => "oproof.bat";

    public async Task<List<Response>> Proof(string userPrompt, string agentResponse)
    {
        string[] args = { userPrompt, agentResponse };
        return await Execute(args);
    }
}
