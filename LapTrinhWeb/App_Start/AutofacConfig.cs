using Autofac;
using Autofac.Integration.Mvc;
using LapTrinhWeb.Services.Vnpay;
using System.Reflection;
using System.Web.Mvc;

namespace LapTrinhWeb.App_Start
{
    public static class AutofacConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            // Đăng ký tất cả Controllers trong assembly hiện tại
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // Đăng ký filter provider (cho phép inject vào ActionFilter, v.v.)
            builder.RegisterFilterProvider();

            // Ví dụ: đăng ký service VnPay
            builder.RegisterType<VnPayService>()
                   .As<IVnPayService>()
                   .InstancePerLifetimeScope();

            // Build container
            var container = builder.Build();

            // Set Autofac làm Dependency Resolver cho MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
