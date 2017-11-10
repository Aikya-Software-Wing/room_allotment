using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ExamRoomAllocation.Startup))]
namespace ExamRoomAllocation
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
