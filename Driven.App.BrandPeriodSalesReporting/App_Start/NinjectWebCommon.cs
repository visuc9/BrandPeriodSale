[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Driven.App.BrandPeriodSalesReporting.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Driven.App.BrandPeriodSalesReporting.App_Start.NinjectWebCommon), "Stop")]


namespace Driven.App.BrandPeriodSalesReporting.App_Start
{
    using System;
    using System.Web;
    using System.Configuration;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    
    using Moq;
    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;

    public static class NinjectWebCommon 
    {
        private static readonly Ninject.Web.Common.Bootstrapper bootstrapper = new Ninject.Web.Common.Bootstrapper();
        private const string mc_Configuration_AuthType = "AuthType";


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
            //Mock<Driven.App.BrandPeriodSalesReporting.Helpers.IDomainGroupService> groupServiceMock = new Mock<Driven.App.BrandPeriodSalesReporting.Helpers.IDomainGroupService>();
            //groupServiceMock.Setup(m => m.GetGroups()).Returns(new System.Collections.Generic.List<string> { "A", "B", "C" });
            //kernel.Bind<Driven.App.BrandPeriodSalesReporting.Helpers.IDomainGroupService>().ToConstant(groupServiceMock.Object);


            // *************************************************
            // *** Fruit Stand Mock / Inject ***
            //Mock<Driven.Business.Playground.Interfaces.IFruitStandService> citrisFruitStandMock = new Mock<Driven.Business.Playground.Interfaces.IFruitStandService>();
            //citrisFruitStandMock.Setup(m => m.GetAllFruit()).Returns(new System.Collections.Generic.List<Driven.Data.Entity.Playground.Fruit>
            //{
            //    new Driven.Data.Entity.Playground.Fruit { Id = 1, Name = "Grapefruit", Price = 1.99M },
            //    new Driven.Data.Entity.Playground.Fruit { Id = 2, Name = "Lemon", Price = 2.99M },
            //    new Driven.Data.Entity.Playground.Fruit { Id = 3, Name = "Tangerine", Price = 5.99M },
            //    new Driven.Data.Entity.Playground.Fruit { Id = 4, Name = "Orange", Price = .98M }
            //});
            //citrisFruitStandMock.Setup(m => m.GetFruitById(It.IsAny<int>())).Returns(new Driven.Data.Entity.Playground.Fruit() { Id = 1, Name = "Mandarin", Price = 1.49M });
            //kernel.Bind<Driven.Business.Playground.Interfaces.IFruitStandService>().ToConstant(citrisFruitStandMock.Object);


            // *************************************************
            // *** Product Mock / Inject ***
            //Mock<Driven.Business.Playground.Interfaces.IProductService> mock = new Mock<Driven.Business.Playground.Interfaces.IProductService>();
            //mock.Setup(m => m.GetAllProducts()).Returns(new System.Collections.Generic.List<Driven.Data.Entity.Playground.Product>
            //{
            //    new Driven.Data.Entity.Playground.Product { ProductId = 1, Name = "Apples", Price = 19},
            //    new Driven.Data.Entity.Playground.Product { ProductId = 10, Name = "Peaches", Price = 199},
            //    new Driven.Data.Entity.Playground.Product { ProductId = 100, Name = "Pears", Price = 299}
            //});
            //mock.Setup(m => m.GetProductById(It.IsAny<int>())).Returns(new Driven.Data.Entity.Playground.Product() { ProductId = 12344, Name = "Grapes" });
            //kernel.Bind<Driven.Business.Playground.Interfaces.IProductService>().ToConstant(mock.Object);
            //kernel.Bind<Driven.Business.BrandPeriodSalesReporting.Interfaces.IProductService>().ToConstructor(x => new Driven.Business.BrandPeriodSalesReporting.ProductService());


            // *************************************************

            //kernel.Bind<Microsoft.Owin.Security.IAuthenticationManager>().ToMethod(c => HttpContext.Current.GetOwinContext().Authentication).InRequestScope();

            // *** Active Directory Authentication ***
            //kernel.Bind<Driven.Business.Security.Interfaces.IAuthenticationService>()
            //    .To<Driven.Business.Security.AdAuthenticationService>()
            //    .WithConstructorArgument("authenticationType", ConfigurationManager.AppSettings[mc_Configuration_AuthType]);

            // *** OKTA Authentication ***
            //kernel.Bind<Driven.Business.Security.Interfaces.IAuthenticationService>()
            //    .To<Driven.Business.Security.OktaAuthenticationService>()
            //    .WithConstructorArgument("authenticationType", ConfigurationManager.AppSettings[mc_Configuration_AuthType]);




            // *** Fake Authentication ***
            //kernel.Bind<Driven.Business.Security.Interfaces.IAuthenticationService>()
            //    .To<Driven.Business.Security.FakeAuthenticationService>()
            //    .WithConstructorArgument("authenticationType", ConfigurationManager.AppSettings[mc_Configuration_AuthType]);
        }        
    }
}
