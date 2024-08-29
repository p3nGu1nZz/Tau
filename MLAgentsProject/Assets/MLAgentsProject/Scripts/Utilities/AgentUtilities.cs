using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;

public static class AgentUtilities
{
    public static float[] ConvertToFloatArray(double[] vector)
    {
        return vector.Select(d => (float)d).ToArray();
    }

    public static double[] ConvertActionsToDouble(ActionSegment<float> actions)
    {
        double[] result = new double[actions.Length];
        for (int i = 0; i < actions.Length; i++)
        {
            result[i] = actions[i];
        }
        return result;
    }

    public static void AddObservations(VectorSensor sensor, double[] observations)
    {
        float[] floatObservations = ConvertToFloatArray(observations);
        sensor.AddObservation(floatObservations);
    }
}
