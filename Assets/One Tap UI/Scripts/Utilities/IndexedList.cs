using System;

namespace One_Tap_UI.Utilities
{
    [Serializable]
    public class IndexedList<T>
    {
        public T[] list;
        public int index;

        public IndexedList(T[] list)
        {
            this.list = list;
        }
        
        public IndexedList(int length)
        {
            list = new T[length];
        }

        public T Next
        {
            get => list[(index + 1) % list.Length];
            set => index = System.Array.IndexOf(list, value);
        }

        public T Previous
        {
            get => list[(index - 1 + list.Length) % list.Length];
            set => index = System.Array.IndexOf(list, value);
        }

        public T Current
        {
            get => list[index];
            set => index = System.Array.IndexOf(list, value);
        }

        public T this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }
    }
}