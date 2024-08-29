using System.Collections.Generic;

public class AgentData
{
    public Dictionary<string, Embedding> Vocabulary { get; set; }
    public double[] CachedInputVector { get; set; }
    public double[] CachedOutputVector { get; set; }
    public double[] ExpectedOutputVector { get; set; }
    public float[] InputVector { get; set; }
    public float[] OutputVector { get; set; }
    public Dictionary<double[], double[]> TrainingData { get; set; }
    public string TrainingFileName { get; set; }
    public List<EmbeddingPair> TrainingDataList { get; set; }
}
