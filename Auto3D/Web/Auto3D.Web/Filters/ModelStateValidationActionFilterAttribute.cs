﻿namespace Auto3D.Web.Filters
{
    using System.Linq;

    using Auto3D.Web.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ModelStateValidationActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var model = context.ActionArguments.First().Value;
                var controller = (BaseController)context.Controller;
                var actionName = (string)context.RouteData.Values["action"];
                context.Result = controller.View(actionName, model);
            }
        }
    }
}
