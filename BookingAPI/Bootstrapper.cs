using Microsoft.Practices.Unity;
using System.Web.Http;
//
using Resolver;
using Unity.Mvc5;

namespace TGBookingAPI
{
    public class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            System.Web.Mvc.DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            // register dependency resolver for WebAPI RC
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {

            //Component initialization via MEF
            ComponentLoader.LoadContainer(container, ".\\bin", "TGBookingAPI.dll");
            ComponentLoader.LoadContainer(container, ".\\bin", "DataModel.dll");
            ComponentLoader.LoadContainer(container, ".\\bin", "BL.dll");

        }
    }
}