namespace MauiIBGE.App_Code.ConnectFactory
{
    public class Services
    {
        public HttpClient Client { get; }

        public Services()
        {
            Client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(25)
            };
        }
    }
}
