using System.Threading.Tasks;

public class Ophrase : BaseProcess<Ophrase>
{
    private static Ophrase _instance;
    public static Ophrase Instance => _instance ??= new Ophrase();

    protected override string GetBatchFileName() => "ophrase.bat";

    public async Task<string[]> Paraphrase(string inputString) => await Execute(inputString);
}
