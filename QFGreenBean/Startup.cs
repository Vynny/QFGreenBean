using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QFGreenBean.Startup))]
namespace QFGreenBean
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
