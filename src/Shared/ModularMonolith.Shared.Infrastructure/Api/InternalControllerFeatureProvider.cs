using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

// https://andrewlock.net/when-asp-net-core-cant-find-your-controller-debugging-application-parts/
// https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts?view=aspnetcore-5.0
// ASP.NET CORE MVC ANATOMY (PART 1) – ADDMVCCORE
namespace ModularMonolith.Shared.Infrastructure.Api
{
    internal class InternalControllerFeatureProvider : ControllerFeatureProvider
    {
        // override to allow our internal controller in .net core pipline
        protected override bool IsController(TypeInfo typeInfo)
        {
            if (!typeInfo.IsClass)
            {
                return false;
            }

            if (typeInfo.IsAbstract)
            {
                return false;
            }

            if (typeInfo.ContainsGenericParameters)
            {
                return false;
            }

            if (typeInfo.IsDefined(typeof(NonControllerAttribute)))
            {
                return false;
            }

            return typeInfo.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) ||
                   typeInfo.IsDefined(typeof(ControllerAttribute));
        }
    }
}