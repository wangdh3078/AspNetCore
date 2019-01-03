// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures
{
    /// <summary>
    /// Finds and executes an <see cref="IView"/> for a <see cref="PartialViewResult"/>.
    /// </summary>
    public class ViewResultTemplateExecutor : ViewTemplateExecutor, IActionResultExecutor<ViewResult>
    {
        private const string ActionNameKey = "action";

        /// <summary>
        /// Creates a new <see cref="PartialViewResultExecutor"/>.
        /// </summary>
        /// <param name="viewOptions">The <see cref="IOptions{MvcViewOptions}"/>.</param>
        /// <param name="writerFactory">The <see cref="IHttpResponseStreamWriterFactory"/>.</param>
        /// <param name="viewFactory">The <see cref="IViewTemplateFactory"/>.</param>
        /// <param name="tempDataFactory">The <see cref="ITempDataDictionaryFactory"/>.</param>
        /// <param name="diagnosticListener">The <see cref="DiagnosticListener"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        /// <param name="modelMetadataProvider">The <see cref="IModelMetadataProvider"/>.</param>
        public ViewResultTemplateExecutor(
            IOptions<MvcViewOptions> viewOptions,
            IHttpResponseStreamWriterFactory writerFactory,
            IViewTemplateFactory viewFactory,
            ITempDataDictionaryFactory tempDataFactory,
            DiagnosticListener diagnosticListener,
            ILoggerFactory loggerFactory,
            IModelMetadataProvider modelMetadataProvider)
            : base(viewOptions, writerFactory, viewFactory, tempDataFactory, diagnosticListener, modelMetadataProvider)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            Logger = loggerFactory.CreateLogger<ViewResultTemplateExecutor>();
        }

        /// <summary>
        /// Gets the <see cref="ILogger"/>.
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc />
        public virtual async Task ExecuteAsync(ActionContext context, ViewResult result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var stopwatch = ValueStopwatch.StartNew();

            var viewName = result.ViewName ?? GetActionName(context);
            var viewFactoryContext = new ViewFactoryContext(context, executingFilePath: null, viewName, isMainPage: true);
            var locateViewResult = await ViewFactory.LocateViewAsync(viewFactoryContext);

            if (!locateViewResult.Success)
            {
                throw new InvalidOperationException();
            }

            Logger.PartialViewResultExecuting(viewName);
            await ExecuteAsync(
                context,
                locateViewResult.ViewTemplate,
                result.ViewData,
                result.TempData,
                result.ContentType,
                result.StatusCode);

            Logger.PartialViewResultExecuted(result.ViewName, stopwatch.GetElapsedTime());
        }

        private static string GetActionName(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.RouteData.Values.TryGetValue(ActionNameKey, out var routeValue))
            {
                return null;
            }

            var actionDescriptor = context.ActionDescriptor;
            string normalizedValue = null;
            if (actionDescriptor.RouteValues.TryGetValue(ActionNameKey, out var value) &&
                !string.IsNullOrEmpty(value))
            {
                normalizedValue = value;
            }

            var stringRouteValue = Convert.ToString(routeValue, CultureInfo.InvariantCulture);
            if (string.Equals(normalizedValue, stringRouteValue, StringComparison.OrdinalIgnoreCase))
            {
                return normalizedValue;
            }

            return stringRouteValue;
        }
    }
}
