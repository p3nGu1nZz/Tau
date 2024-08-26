using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct CosineSimilarityJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<double> Vectors;
    [ReadOnly] public NativeArray<double> QueryVector;
    public NativeArray<double> Results;
    public int VectorSize;

    public void Execute(int index)
    {
        Debug.Log($"Starting CosineSimilarityJob for index {index}...");

        double dotProduct = 0.0;
        double magnitudeA = 0.0;
        double magnitudeB = 0.0;

        for (int i = 0; i < VectorSize; i++)
        {
            double a = Vectors[index * VectorSize + i];
            double b = QueryVector[i];
            dotProduct += a * b;
            magnitudeA += a * a;
            magnitudeB += b * b;
        }

        magnitudeA = math.sqrt(magnitudeA);
        magnitudeB = math.sqrt(magnitudeB);
        Results[index] = dotProduct / (magnitudeA * magnitudeB);

        Debug.Log($"Completed CosineSimilarityJob for index {index} with similarity {Results[index]:G4}.");
    }
}
