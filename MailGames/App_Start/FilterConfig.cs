using System.Web;
using System.Web.Mvc;
using MailGames.Filters;

namespace MailGames
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new EnsureFullNameAttribute());
        }
    }
}