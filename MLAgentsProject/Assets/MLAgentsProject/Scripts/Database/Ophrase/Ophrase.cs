using System.Threading.Tasks;

public class Ophrase : BaseProcess
{
    private static Ophrase _instance;
    public static Ophrase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Ophrase();
            }
            return _instance;
        }
    }

    protected override string GetBatchFileName()
    {
        return "ophrase.bat";
    }

    public async Task<string[]> Paraphrase(string inputString)
    {
        return await Execute(inputString);
    }
}
