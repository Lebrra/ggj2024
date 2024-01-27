using System;
using UnityEngine;

public class EnumData : PropertyAttribute
{
    public readonly string[] Names;

    public EnumData(Type enumType)
    {
        Names = Enum.GetNames(enumType);
    }
}