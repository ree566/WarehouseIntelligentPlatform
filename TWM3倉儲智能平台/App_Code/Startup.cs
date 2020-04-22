using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IE_資訊平台.Startup))]
namespace IE_資訊平台
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
