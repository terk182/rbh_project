using System.ComponentModel.Composition;
using Resolver;

namespace BL
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IAirPromotionServices, AirPromotionServices>();
            registerComponent.RegisterType<IFlightSearchServices, FlightSearchServices>();
            registerComponent.RegisterType<INamingServices, NamingServices>();
            registerComponent.RegisterType<IShortenURLServices, ShortenURLServices>();
        }
    }
}
