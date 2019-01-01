// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.AspNetCore.Routing
{
    /// <summary>
    /// Represents host metadata used during routing.
    /// </summary>
    [DebuggerDisplay("{DebuggerToString(),nq}")]
    public class HostMetadata : IHostMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostMetadata" /> class.
        /// </summary>
        /// <param name="hosts">
        /// The hosts used during routing.
        /// An empty collection means any host will be accepted.
        /// </param>
        public HostMetadata(IEnumerable<string> hosts)
        {
            if (hosts == null)
            {
                throw new ArgumentNullException(nameof(hosts));
            }

            Hosts = hosts.ToArray();
        }

        /// <summary>
        /// Returns a read-only collection of hosts used during routing.
        /// An empty collection means any host will be accepted.
        /// </summary>
        public IReadOnlyList<string> Hosts { get; }

        private string DebuggerToString()
        {
            var hostsDisplay = (Hosts.Count == 0)
                ? "*:*"
                : string.Join(",", Hosts.Select(h => h.Contains(':') ? h : h + ":*"));

            return $"Hosts: {hostsDisplay}";
        }
    }
}
