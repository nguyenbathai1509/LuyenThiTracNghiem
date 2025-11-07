using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LuyenThiTracNghiem.Filters
{
    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetInt32("Role");

            if (role == null || role != 1)
            {
                context.Result = new RedirectToActionResult("NotFoundPage", "Error", new { area = "Admin" });
            }

            base.OnActionExecuting(context);
        }
    }
}