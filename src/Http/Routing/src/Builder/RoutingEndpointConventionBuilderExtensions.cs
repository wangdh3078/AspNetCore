// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder
{
    public static class RoutingEndpointConventionBuilderExtensions
    {
        public static IEndpointConventionBuilder RequireHost(this IEndpointConventionBuilder builder, params string[] hosts)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (hosts == null)
            {
                throw new ArgumentNullException(nameof(hosts));
            }

            builder.Add(endpointBuilder =>
            {
                endpointBuilder.Metadata.Add(new HostAttribute(hosts));
            });
            return builder;
        }
    }
}
