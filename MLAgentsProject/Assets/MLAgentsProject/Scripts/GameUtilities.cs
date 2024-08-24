using UnityEngine;

public static class GameUtilities
{
    public static void RemoveExistingInstances(string prefabName)
    {
        GameObject[] existingObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in existingObjects)
        {
            if (obj.name == prefabName)
            {
                Object.Destroy(obj);
            }
        }

        Log.Message($"Removed existing instances of {prefabName} from the scene.");
    }
}
