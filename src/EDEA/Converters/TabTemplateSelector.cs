using System.Windows;
using System.Windows.Controls;
using EDEA.ViewModels;

namespace EDEA.Selectors;

public class TabTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is TabViewModel vm && container is FrameworkElement element)
        {
            return element.TryFindResource(vm.TabName) as DataTemplate;
        }

        return null;
    }
}
