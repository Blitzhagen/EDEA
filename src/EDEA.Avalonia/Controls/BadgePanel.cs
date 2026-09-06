using System;
using Avalonia;
using Avalonia.Controls;

namespace EDEA.Avalonia.Controls;

/// <summary>
/// Centers the first child horizontally and places all following children
/// as badges to its right. The desired size reserves symmetric space so
/// the main child stays centered under a centered header icon.
/// </summary>
public sealed class BadgePanel : Panel
{
    private const double BadgeGap = 2;

    protected override Size MeasureOverride(Size availableSize)
    {
        double mainWidth = 0;
        double badgesWidth = 0;
        double maxHeight = 0;
        bool first = true;

        foreach (Control child in Children)
        {
            child.Measure(availableSize);

            if (first)
            {
                mainWidth = child.DesiredSize.Width;
                first = false;
            }
            else
            {
                badgesWidth += child.DesiredSize.Width + BadgeGap;
            }

            maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
        }

        // Add the same amount of empty space on the left as the badges
        // use on the right so the main child is truly centered.
        double totalWidth = mainWidth + 2 * badgesWidth;
        return new Size(Math.Max(0, totalWidth), maxHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0)
        {
            return finalSize;
        }

        var main = Children[0];
        double mainX = (finalSize.Width - main.DesiredSize.Width) / 2;
        double mainY = (finalSize.Height - main.DesiredSize.Height) / 2;

        main.Arrange(new Rect(mainX, mainY, main.DesiredSize.Width, main.DesiredSize.Height));

        double x = mainX + main.DesiredSize.Width + BadgeGap;
        for (int i = 1; i < Children.Count; i++)
        {
            var badge = Children[i];
            double badgeY = (finalSize.Height - badge.DesiredSize.Height) / 2;

            badge.Arrange(new Rect(x, badgeY, badge.DesiredSize.Width, badge.DesiredSize.Height));
            x += badge.DesiredSize.Width + BadgeGap;
        }

        return finalSize;
    }
}
