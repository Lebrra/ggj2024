using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kickstarter.Extensions
{
    public static class DictionaryExtensions
    {
        public static void LoadDictionary<TEnum, TType>(this Dictionary<TEnum, TType> dictionary, TType[] items) where TEnum : Enum
        {
            var values = Enum.GetValues(typeof(TEnum));
            if (values.Length != items.Length)
            {
                Debug.LogError("Invalid Use of LoadDictionary Extension Method. Items must have the same length as the enum.");
                return;
            }
            var valuesArray = new TEnum[values.Length];
            values.CopyTo(valuesArray, 0);
            for (int i = 0; i < values.Length; i++)
                dictionary.Add(valuesArray[i], items[i]);
        }
    }
}
