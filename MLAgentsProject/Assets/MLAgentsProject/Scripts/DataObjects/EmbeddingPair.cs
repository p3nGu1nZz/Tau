public struct EmbeddingPair
{
    public double[] InputEmbedding { get; set; }
    public double[] OutputEmbedding { get; set; }

    public EmbeddingPair(double[] inputEmbedding, double[] outputEmbedding)
    {
        InputEmbedding = inputEmbedding;
        OutputEmbedding = outputEmbedding;
    }
}
