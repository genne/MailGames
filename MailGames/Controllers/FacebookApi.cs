using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Facebook;
using MailGames.Models;

namespace MailGames.Controllers
{
    public class FacebookApi
    {
        public static object Me()
        {
            return Get("me");
        }

        private static object Get(string path, object parameters = null)
        {
            var accessToken = HttpContext.Current.Session["facebooktoken"] as string;
            if (accessToken == null) return null;
            var client = new FacebookClient(accessToken);
            return client.Get(path, parameters);
        }

        public static IEnumerable<StartGameHomeViewModel.Friend> Friends()
        {
            dynamic friends = Get("me/friends", new{ fields = "picture,name"});
            if (friends == null) return null;
            return ((IEnumerable<dynamic>)friends.data).Select(f => new StartGameHomeViewModel.Friend
            {
                Id = f.id,
                Name = f.name,
                Picture = f.picture.data.url
            });
        }
    }
}