﻿/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.DependencyInjection;
using Corsinvest.ProxmoxVE.Api;

namespace Corsinvest.ProxmoxVE.Admin.Core.Services;

public interface IPveClientService : IScopedDependency
{
    Task<PveClient?> GetClient(ClusterOptions clusterOptions);
    Task<PveClient?> GetClient(ClusterOptions clusterOptions, ILogger logger);
    Task<PveClient?> GetClient(string clusterName);
    Task<PveClient> GetClientCurrentCluster();
    ClusterOptions? GetClusterOptions(string clusterName);
    Task<ClusterOptions?> GetCurrentClusterOptions();
    IEnumerable<ClusterOptions> GetClusters();
    Task SetCurrentClusterName(string clusterName);
    Task<string> GetCurrentClusterName();
    Task<bool> ExistsCurrentClusterName();
    Task<bool> ClusterIsValid(string clusterName);
    Task<int> PopulateInfoNodes(ClusterOptions clusterOptions);
    Task<bool> CheckIsValidVersion(PveClient client);
    Task<Api.Shared.Models.Cluster.ClusterStatus?> GetClusterStatus(PveClient client);
    string GetUrl(ClusterOptions clusterOptions);
}