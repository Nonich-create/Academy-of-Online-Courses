using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Students.MVC.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public GlobalExceptionFilter(
            IWebHostEnvironment hostingEnvironment,
            IModelMetadataProvider modelMetadataProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public void OnException(ExceptionContext context)
        {
            if (!_hostingEnvironment.IsDevelopment())
            {
                return;
            }

            var result = new ViewResult { ViewName = "Error" };
            result.ViewData = new ViewDataDictionary(_modelMetadataProvider,
                context.ModelState);
            result.ViewData.Add("Exception", context.Exception);
            context.Result = result;
        }
    }
}
