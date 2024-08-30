using UnityEngine;

public static class ComponentUtilities
{
    public static void EnableAllComponents(GameObject gameObject)
    {
        foreach (var component in gameObject.GetComponents<Component>())
        {
            var type = component.GetType();
            var enabledProperty = type.GetProperty("enabled");
            if (enabledProperty != null && enabledProperty.PropertyType == typeof(bool))
            {
                enabledProperty.SetValue(component, true);
            }
        }
    }
}
