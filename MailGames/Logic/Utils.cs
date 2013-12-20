namespace MailGames.Logic
{
    public static class Utils
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            var first = a;
            a = b;
            b = first;
        }
    }
}