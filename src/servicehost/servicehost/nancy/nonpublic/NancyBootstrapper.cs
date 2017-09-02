using System.Collections.Generic;
using servicehost.nonpublic;

namespace servicehost.nancy.nonpublic
{
    class NancyBootstrapper : Nancy.DefaultNancyBootstrapper {
        readonly IEnumerable<ServiceInfo> services;

        public NancyBootstrapper(IEnumerable<ServiceInfo> services) {
            this.services = services;
        }

        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<IEnumerable<ServiceInfo>>(this.services);
        }
    }
}