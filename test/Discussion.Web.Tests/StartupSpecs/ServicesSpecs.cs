﻿using Discussion.Web.Models;
using Discussion.Web.Data.InMemory;
using Jusfr.Persistent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Discussion.Web.Data;

namespace Discussion.Web.Tests.StartupSpecs
{
    public class ServicesSpecs
    {
        [Fact]
        public void should_add_mvc_service()
        {
            // arrage
            IServiceCollection services = null;

            // act
            CreateApplicationServices(c => { }, serviceCollection => {
                services = serviceCollection;
            });

            // assert
            services.Count.ShouldGreaterThan(0);
            services.ShouldNotEmpty();
            services.ShouldContain(x => x.ServiceType.ToString().EndsWith("IControllerActivator"));
            services.ShouldContain(x => x.ServiceType.ToString().EndsWith("IControllerFactory"));
            services.ShouldContain(x => x.ServiceType.ToString().EndsWith("IApiDescriptionProvider"));
            services.ShouldContain(x => x.ServiceType.ToString().EndsWith("MvcMarkerService"));
        }


        [Fact]
        public void should_add_ravendb_reposotiry()
        {
            // arrange
            var applicationServices = CreateApplicationServices((configuration) => {
                configuration["ravenConnectionString"] = "Url=http://ravendb.mydomain.com;Database=Northwind";
            }, s => { });

            // act
            var repo = applicationServices.GetRequiredService<Repository<Article>>();

            // assert
            repo.ShouldNotBeNull();
            repo.GetType().GetGenericTypeDefinition().ShouldEqual(typeof(RavenDataRepository<>));
        }


        [Fact]
        public void should_add_base_implemention_for_repository()
        {
            // arrage
            var applicationServices = CreateApplicationServices();

            // act
            var repo = applicationServices.GetRequiredService<IRepository<Article>>();

            // assert
            repo.ShouldNotBeNull();
            repo.GetType().ShouldEqual(typeof(InMemoryDataRepository<Article>));
        }

        public static IServiceProvider CreateApplicationServices()
        {
            return CreateApplicationServices(c => { },  s => { });
        }

        public static IServiceProvider CreateApplicationServices(Action<IConfigurationRoot> configureSettings, Action<IServiceCollection> configureServices) {
            var services = new ServiceCollection();
            var startup = CreateMockStartup();
            configureSettings(startup.Configuration);

            startup.ConfigureServices(services);
            configureServices(services);

            return services.BuildServiceProvider();
        }

        public static Startup CreateMockStartup()
        {
            var hostingEnv = new Mock<IHostingEnvironment>();
            hostingEnv.SetupGet(e => e.EnvironmentName).Returns("UnitTest");
            hostingEnv.SetupGet(e => e.ContentRootPath).Returns(TestEnv.WebProjectPath());

            return new Startup(hostingEnv.Object);
        }

    }
}