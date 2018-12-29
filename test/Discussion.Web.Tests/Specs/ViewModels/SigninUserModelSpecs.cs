﻿using Discussion.Core.ViewModels;
using Discussion.Tests.Common;
using Discussion.Tests.Common.AssertionExtensions;
using Xunit;

namespace Discussion.Web.Tests.Specs.ViewModels
{
    [Collection("WebSpecs")]
    public class SigninUserModelSpecs
    {
        private readonly TestDiscussionWebApp _app;
        public SigninUserModelSpecs(TestDiscussionWebApp app)
        {
            _app = app;
        }
        
        [Fact]
        public void should_validate_normal_username_and_password_values_as_valid()
        {
            var modelState = _app.ValidateModel(
                new UserViewModel
                {
                    UserName = "validusername",
                    Password = "Mypassword"
                });

            Assert.True(modelState.IsValid);
            modelState.Keys.ShouldNotContain("UserName");
            modelState.Keys.ShouldNotContain("Password");
        }
      
             
        [Theory]
        [InlineData("ausernamelessthan20")]
        [InlineData("userName")]
        [InlineData("valid-user")]
        [InlineData("valid_")]
        [InlineData("00valid")]
        [InlineData("007")]
        public void should_validate_valid_username_values_as_valid(string username)
        {
            var modelState = _app.ValidateModel(
                new UserViewModel
                {
                    UserName = username,
                    Password = "password1"
                });

            Assert.True(modelState.IsValid);
            modelState.Keys.ShouldNotContain("UserName");
            modelState.Keys.ShouldNotContain("Password");
        }

        [Theory]
        [InlineData("ausernamelongerthan20")]
        [InlineData("user name")]
        [InlineData("^valid")]
        [InlineData("valid@")]
        [InlineData("va#lid")]
        [InlineData("in")]
        public void should_validate_invalid_username_values_as_invalid(string username)
        {
            var modelState = _app.ValidateModel(
                new UserViewModel
                {
                    UserName = username,
                    Password = "password1"
                });

            Assert.False(modelState.IsValid);
            modelState.Keys.ShouldContain("UserName");
            modelState.Keys.ShouldNotContain("Password");
        }
   
        [Theory]
        [InlineData("pass word")]
        [InlineData("thePassword01!")]
        [InlineData("LRo39sCeQU7$")]
        [InlineData("#^%&Nz@&^7asd$")]
        [InlineData("passWord")]
        [InlineData("p@ssw0rd")]
        [InlineData("11111a")]
        [InlineData("a11111")]
        [InlineData("$11113")]
        [InlineData("{11113")]
        [InlineData("11113+")]
        [InlineData("11113=")]
        [InlineData("11113<")]
        [InlineData("11113?")]
        public void should_validate_valid_password_values_as_valid(string password)
        {
            var modelState = _app.ValidateModel(
                new UserViewModel
                {
                    UserName = "validuser",
                    Password = password
                });

            Assert.True(modelState.IsValid);
            modelState.Keys.ShouldNotContain("UserName");
            modelState.Keys.ShouldNotContain("Password");
        }
        
        
        
        [Theory]
        [InlineData("Apasswordlongerthan20")]
        [InlineData("password")]
        [InlineData("111111")]
        [InlineData("1111a")]
        [InlineData("F1111")]
        [InlineData("1111G")]
        [InlineData("*&@%~%!")]
        public void should_validate_invalid_password_values_as_invalid(string password)
        {
            var modelState = _app.ValidateModel(
                new UserViewModel
                {
                    UserName = "validuser",
                    Password = password
                });

            Assert.False(modelState.IsValid);
            modelState.Keys.ShouldNotContain("UserName");
            modelState.Keys.ShouldContain("Password");
        }

    }
    
}