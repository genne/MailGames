using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailGames.Context;
using MailGames.Controllers;
using MailGames.Logic;

namespace MailGames.Filters
{
    public class EnsureFullNameAttribute : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated) return;
            if ((string) filterContext.RouteData.Values["controller"] == "Account") return;
            if (filterContext.HttpContext.Session != null && filterContext.HttpContext.Session["fullname"] != null) return;

            var player = PlayerManager.GetCurrent(new MailGamesContext());
            if (player.FullName == null)
            {
                var url = new UrlHelper(filterContext.RequestContext).Action("EnterFullName", "Account", new { returnUrl = filterContext.HttpContext.Request.Url.ToString() });
                filterContext.Result = new RedirectResult(url);
            }
            else
            {
                filterContext.HttpContext.Session["fullname"] = player.FullName;
            }
        }
    }
}