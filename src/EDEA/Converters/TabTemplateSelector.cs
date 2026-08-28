using System.Windows;
using System.Windows.Controls;
using EDEA.ViewModels;

namespace EDEA.Selectors;

/// <summary>
/// Selects a data template for a tab based on the tab view model name.
/// </summary>
public class TabTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Selects a data template for the specified tab item.
    /// </summary>
    /// <param name="item">The tab view model whose name is used as the template resource key.</param>
    /// <param name="container">The container element used to look up the resource.</param>
    /// <returns>The matching <see cref="DataTemplate"/> or <c>null</c> when no template is found.</returns>
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is TabViewModel vm && container is FrameworkElement element)
        {
            return element.TryFindResource(vm.TabName) as DataTemplate;
        }

        return null;
    }
}
