namespace FastLaunch.Server
{
    internal class ServerLaunch
    {
        static void Main(string[] args)
        {
            FastLaunch.Common.FastStartupLauncher.ClientMain(args, -1);
        }
    }
}
