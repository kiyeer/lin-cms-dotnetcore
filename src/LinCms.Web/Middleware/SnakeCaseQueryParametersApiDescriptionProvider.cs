﻿using System.Linq;
using LinCms.Zero.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace LinCms.Web.Middleware
{
    public class SnakeCaseQueryParametersApiDescriptionProvider : IApiDescriptionProvider
    {
        public int Order => 1;

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
        }

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
            foreach (ApiParameterDescription parameter in context.Results.SelectMany(x => x.ParameterDescriptions).Where(x => x.Source.Id == "Query" || x.Source.Id == "ModelBinding"))
            {
                parameter.Name = parameter.Name.ToSnakeCase();
            }
        }
    }
}
