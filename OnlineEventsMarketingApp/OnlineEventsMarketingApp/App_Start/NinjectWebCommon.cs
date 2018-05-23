using Ninject.Web.Common.WebHost;
using OnlineEventsMarketingApp.BusinessLogic;
using OnlineEventsMarketingApp.Entities.Users;
using OnlineEventsMarketingApp.Infrastructure.Implementations;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Interfaces;
using OnlineEventsMarketingApp.Services.Implementations;
using OnlineEventsMarketingApp.Services.Interfaces;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(OnlineEventsMarketingApp.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(OnlineEventsMarketingApp.App_Start.NinjectWebCommon), "Stop")]

namespace OnlineEventsMarketingApp.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            //Infrastructure
            kernel.Bind<IDatabaseFactory>().To<DatabaseFactory>().InRequestScope();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();
            kernel.Bind(typeof(IRepository<>)).To(typeof(Repository<>));

            //Services
            kernel.Bind<IUserService>().To<UserService>().InRequestScope();
            kernel.Bind<ITagService>().To<TagService>().InRequestScope();
            kernel.Bind<IDataSheetService>().To<DataSheetService>().InRequestScope(); 
            kernel.Bind<IPaginationService>().To<PaginationService>().InRequestScope();
        }        
    }
}
