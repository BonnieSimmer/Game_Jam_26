using System;

namespace One_Tap_UI.Utilities
{
    public static class Enums
    {
        public static T Next<T>(this T src) where T : struct // This method returns the next enum value
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
    
            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = System.Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length==j) ? Arr[0] : Arr[j];            
        }
    }
}
