using servicehost.contract;

namespace democlient
{
    [Service]
    public class DemoClientService
    {
        [EntryPoint(HttpMethods.Get, "/client/subtract")]
        public int Subtract(int a, int b) => a - b;
    }
}