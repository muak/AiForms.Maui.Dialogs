namespace AiForms.Dialogs;

internal static class PageExtensions
{
    public static Page GetActivePage(this Page page)
    {
        if(page is FlyoutPage flyoutPage)
        {           
            return flyoutPage.Detail.GetActivePage();
        }

        if(page is TabbedPage tabbedPage)
        {            
            return tabbedPage.CurrentPage.GetActivePage();
        }

        if(page is NavigationPage navPage)
        {            
            return navPage.CurrentPage.GetActivePage();
        }

        return page;
    }
}
