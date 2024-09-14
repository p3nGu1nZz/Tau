using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class Oproof : BaseProcess<Oproof, Response>
{
    private static Oproof _instance;
    public static Oproof Instance => _instance ??= new Oproof();

    protected override string GetBatchFileName() => "oproof.bat";

    public async Task<List<Response>> Proof(string userPrompt, string agentResponse)
    {
        string[] args = { userPrompt, agentResponse };
        Log.Message($"Proof: Starting with userPrompt='{userPrompt}' and agentResponse='{agentResponse}'");

        List<Response> result = await Execute(args);

        foreach (var response in result)
        {
            Log.Message($"Proof: Execution completed with response: " +
                        $"prompt='{response.prompt}', " +
                        $"response='{response.response}', " +
                        $"is_valid='{response.is_valid}', " +
                        $"domain='{response.domain}', " +
                        $"context='{response.context}', " +
                        $"reason='{response.reason ?? string.Empty}'");
        }

        return result;
    }
}
