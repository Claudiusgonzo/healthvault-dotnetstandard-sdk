﻿// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Grace.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.HealthVault.AspNetCore.Connection;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Extensions;

namespace Microsoft.HealthVault.AspNetCore.Internal
{
    internal static class WebIoc
    {
        private static readonly object s_registrationLock = new object();

        private static bool s_typesRegistered;

        public static void EnsureTypesRegistered(HealthVaultAuthenticationOptions configuration)
        {
            lock (s_registrationLock)
            {
                if (!s_typesRegistered)
                {
                    RegisterTypes(Ioc.Container, configuration);
                    s_typesRegistered = true;
                }
            }
        }

        internal static void RegisterTypes(DependencyInjectionContainer container, HealthVaultAuthenticationOptions configuration)
        {
            RegisterConfiguration(container, configuration);
            container.RegisterSingleton<IHttpClientFactory, WebHttpClientFactory>();
            container.RegisterSingleton<ICertificateInfoProvider, CertificateInfoProvider>();
            container.RegisterSingleton<IServiceInstanceProvider, ServiceInstanceProvider>();
            container.RegisterTransient<IWebSessionCredentialClient, WebSessionCredentialClient>();
            container.RegisterTransient<IWebHealthVaultConnection, WebHealthVaultConnection>();
            container.RegisterTransient<IOfflineHealthVaultConnection, OfflineHealthVaultConnection>();
            container.RegisterSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        private static void RegisterConfiguration(DependencyInjectionContainer container, HealthVaultAuthenticationOptions configuration)
        {
           container.Configure(c => c.ExportInstance(configuration).As<HealthVaultAuthenticationOptions>());
           container.Configure(c => c.ExportInstance(configuration.Configuration).As<HealthVaultConfiguration>());
        }
    }
}