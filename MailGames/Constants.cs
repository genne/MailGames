using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailGames
{
    public class Constants
    {
#if DEBUG        
        public static string FACEBOOK_APP_ID = "259801964178698";
        public static string FACEBOOK_SECRET = "d827973aacdbb23c876367b7e90945c7";
#else
        public static string FACEBOOK_APP_ID = "221909777970862";
        public static string FACEBOOK_SECRET = "a1af42d7c6ab1528f80c1c47e1b652aa";
#endif

        public static string OpponentComputerId = "[computer]";
        public static string OpponentRandomPlayerId = "[random player]";
    }
}