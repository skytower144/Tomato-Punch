using UnityEngine;

public class ProgressInterfaceAttribute : PropertyAttribute
{
    public System.Type requiredType { get; private set; }

    public ProgressInterfaceAttribute(System.Type type)
    {
        this.requiredType = type;
    }
}
