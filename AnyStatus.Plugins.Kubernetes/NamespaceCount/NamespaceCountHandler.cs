﻿/*
Anystatus Kubernetes plugin
Copyright 2019 Fatih Boy

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */
using System.Threading;
using System.Threading.Tasks;
using AnyStatus.API;
using AnyStatus.Plugins.Kubernetes.Shared;

namespace AnyStatus.Plugins.Kubernetes.NamespaceCount
{
    public class NamespaceCountHandler : IRequestHandler<MetricQueryRequest<NamespaceCountWidget>>
    {
        /// <summary>
        /// Kubernetes Helper to retrieve Kubernetes client
        /// </summary>
        private readonly KubernetesHelper kubernetesHelper;

        public NamespaceCountHandler() : this(new KubernetesHelper()) { }

        /// <summary>
        /// Constructer used by unit tests
        /// </summary>
        /// <param name="kubernetesHelper">Kubernetes Helper class instance to use</param>
        internal NamespaceCountHandler(KubernetesHelper kubernetesHelper)
        {
            this.kubernetesHelper = kubernetesHelper;
        }

        public async Task Handle(MetricQueryRequest<NamespaceCountWidget> request, CancellationToken cancellationToken)
        {
            var namespaceCountWidget = request.DataContext;

            var client = kubernetesHelper.GetKubernetesClient(namespaceCountWidget);

            var namespacesResponse = await client.GetNamespacesAsync(cancellationToken);

            if (namespacesResponse.IsValid)
            {
                request.DataContext.Value = namespacesResponse.Items.Length;
                request.DataContext.State = State.Ok;
            }
            else
            {
                namespaceCountWidget.State = State.Invalid;
            }
        }
    }
}
