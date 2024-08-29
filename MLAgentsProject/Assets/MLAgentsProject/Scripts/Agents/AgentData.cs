using System.Collections.Generic;

public class AgentData
{
    public Dictionary<string, Embedding> Vocabulary { get; set; }
    public double[] ModelInput { get; set; }
    public double[] ModelOutput { get; set; }
    public double[] ExpectedOutput { get; set; }
    public float[] Observations { get; set; }
    public string TrainingFileName { get; set; }
    public List<EmbeddingPair> TrainingDataList { get; set; }
}
