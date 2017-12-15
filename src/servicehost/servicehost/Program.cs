namespace servicehost
{
    public class Program
    {
        public static void Main(string[] args) {
            var cli = new CLI(args);
			ServiceHost.Run(cli.Endpoint);
        }
    }
}