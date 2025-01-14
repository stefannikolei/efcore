// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class SingletonOptionsInitializer : ISingletonOptionsInitializer
    {
        private volatile bool _isInitialized;
        private readonly object _lock = new();

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void EnsureInitialized(
            IServiceProvider serviceProvider,
            IDbContextOptions options)
        {
            if (!_isInitialized)
            {
                lock (_lock)
                {
                    if (!_isInitialized)
                    {
                        foreach (var singletonOptions in serviceProvider.GetRequiredService<IEnumerable<ISingletonOptions>>())
                        {
                            singletonOptions.Initialize(options);
                        }

                        _isInitialized = true;
                    }
                }
            }

            foreach (var singletonOptions in serviceProvider.GetRequiredService<IEnumerable<ISingletonOptions>>())
            {
                singletonOptions.Validate(options);
            }
        }
    }
}
