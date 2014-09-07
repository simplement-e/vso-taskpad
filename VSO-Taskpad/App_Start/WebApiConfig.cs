using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;

namespace VSO_Taskpad.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new NotImplExceptionFilterAttribute());
            config.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            //config.MessageHandlers.Add(new AuthOverrideHandler());
            // Attribute routing.
            config.MapHttpAttributeRoutes();
        }
    }


    class TraceExceptionLogger : IExceptionLogger
    {
        public Task LogAsync(ExceptionLoggerContext context,
                                     CancellationToken cancellationToken)
        {
            if (!ShouldLog(context))
            {
                return Task.FromResult(0);
            }

            return LogAsyncCore(context, cancellationToken);
        }

        public Task LogAsyncCore(ExceptionLoggerContext context,
                                         CancellationToken cancellationToken)
        {
            LogCore(context);
            return Task.FromResult(0);
        }

        public virtual bool ShouldLog(ExceptionLoggerContext context)
        {
            if (context.Exception is NotImplementedException)
                return false;
            return true;
        }

        public void LogCore(ExceptionLoggerContext context)
        {
            // todo
        }
    }

    class NotImplExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is NotImplementedException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
        }
    }

}