using servicehost.nancy;

namespace servicehost
{
    public static class ServiceModuleFactory
    {
        public static NancyServiceModule Build() {
            var collector = new ServiceCollector();   
            var services = collector.Collect();
            return new NancyServiceModule(services);
        }
    }
}