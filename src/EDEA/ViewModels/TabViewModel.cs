namespace EDEA.ViewModels;

public class TabViewModel : ViewModelBase
{
    public string TabHeader { get; set; }

    public string TabVisibility { get; set; }

    public virtual string TabName => TabHeader;

    public TabViewModel(string tabHeader, string tabVisibility)
    {
        TabHeader = tabHeader;
        TabVisibility = tabVisibility;
    }
}
