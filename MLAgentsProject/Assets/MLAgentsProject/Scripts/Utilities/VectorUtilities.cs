using UnityEngine;

public static class VectorUtilities
{
    public static double[] GetRandomVector(int size)
    {
        double[] vector = new double[size];
        for (int i = 0; i < size; i++)
        {
            vector[i] = Random.Range(-1f, 1f);
        }
        return vector;
    }
}
