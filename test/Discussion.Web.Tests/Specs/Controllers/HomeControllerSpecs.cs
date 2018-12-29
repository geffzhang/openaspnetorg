﻿using System.Net;
using Discussion.Tests.Common;
using Discussion.Tests.Common.AssertionExtensions;
using Discussion.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Discussion.Web.Tests.Specs.Controllers
{
    [Collection("WebSpecs")]
    public class HomeControllerSpecs
    {
        private readonly TestDiscussionWebApp _theApp;
        public HomeControllerSpecs(TestDiscussionWebApp app)
        {
            _theApp = app;
        }



        [Fact]
        public void should_serve_about_page_as_view_result()
        {
            var homeController = new HomeController(NullLogger<HomeController>.Instance);

            var aboutResult = homeController.About();

            Assert.NotNull(aboutResult);
            Assert.IsType<ViewResult>(aboutResult);
        }


        [Fact]
        public void should_serve_error_page_as_view_result()
        {
            var homeController = _theApp.CreateController<HomeController>();

            var errorResult = homeController.Error();

            Assert.NotNull(errorResult);
            Assert.IsType<ViewResult>(errorResult);

            homeController.Response.StatusCode.ShouldEqual((int)HttpStatusCode.InternalServerError);
        }
    }
}
