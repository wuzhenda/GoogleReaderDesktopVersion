namespace DesktopGoogleReader
{
    using System.Windows;
    using System.Windows.Media;

    public static class TreeHelper
    {
        public static FrameworkElement FindChildByName(DependencyObject parent, string name) 
        {
            return IterateChildren(parent, name);
        }

        private static FrameworkElement IterateChildren(DependencyObject parent, string name) 
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) 
            {
                var child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if (child != null)
                {
                    if(child.Name == name)
                        return child;
                    var grandChild = IterateChildren(child, name);
                    if (grandChild != null)
                    {
                        return grandChild;
                    }
                }
            }
            return null;
        }
    }
}
