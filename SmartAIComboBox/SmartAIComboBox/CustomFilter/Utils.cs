using System.Collections.ObjectModel;

namespace SmartAIComboBox.SmartAIComboBox
{
    internal static class Utils
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
