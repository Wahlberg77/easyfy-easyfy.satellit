// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Principal;
using System.Threading;
using System.Web;
using Easyfy.Data.RavenDb;
using Easyfy.Integration.Mandrill;
using Easyfy.Satellit.Contracts;
using Easyfy.Satellit.Model.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Raven.Client;
using RavenDB.AspNet.Identity;
using StructureMap.Web;

namespace Easyfy.Satellit.Web.DependencyResolution {
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
	
    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });
            //For<IExample>().Use<Example>();

            For<IDocumentSession>().HttpContextScoped().Use(o => RavenStore.GetSession());
            For<IPrincipal>().HttpContextScoped().Use(o => Thread.CurrentPrincipal);
            For<HttpContextBase>().Use(x => new HttpContextWrapper(HttpContext.Current));

            For<IMailService>().Use<MandrillMailService>();
            For<NLog.Logger>().Use(x => NLog.LogManager.GetLogger(x.ParentType == null ? x.RootType.FullName : x.ParentType.FullName));

            For<IAuthenticationManager>().Use(() => HttpContext.Current.GetOwinContext().Authentication);
            For<IUserStore<User>>().Use<UserStore<User>>();

        }

        #endregion
    }
}