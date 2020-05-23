using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Jellyfin.Extensions
{
    public static class XamlExtensions
    {
        /// <summary>
        /// Finds visual child for a parent.
        /// </summary>
        /// <typeparam name="T">The child type.</typeparam>
        /// <param name="parent">The parent reference.</param>
        /// <returns></returns>
        public static T FindVisualChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    T candidate = child as T;
                    if (candidate != null)
                    {
                        return candidate;
                    }

                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return default;
        }
    }
}
