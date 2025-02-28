﻿/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: AGPL-3.0-only
 */
using Corsinvest.AppHero.Core.Options;
using Corsinvest.AppHero.Core.Service;
using Corsinvest.AppHero.Core.UI;
using MudExtensions;
using MudExtensions.Enums;

namespace Corsinvest.ProxmoxVE.Admin.Core.UI.Dialogs;

public partial class DialogFastConfig
{
    [Inject] private IPveClientService PveClientService { get; set; } = default!;
    [Inject] private IWritableOptionsService<AdminOptions> WritableOptionsService { get; set; } = default!;
    [Inject] private IOptionsSnapshot<AdminOptions> AdminOptions { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IBrowserService BrowserService { get; set; } = default!;

    private MudStepper? RefStepper { get; set; } = default!;
    private MudForm? RefForm { get; set; } = default!;

    public ClusterOptions ClusterOptions => AdminOptions.Value.Clusters[0];

    protected override void OnInitialized()
    {
        if (!AdminOptions.Value.Clusters.Any()) { AdminOptions.Value.Clusters.Add(new()); }
    }

    private async Task<bool> PreventStepChangeAsync(StepChangeDirection direction)
    {
        var ret = false;

        if (direction == StepChangeDirection.Forward)
        {
            switch (RefStepper!.GetActiveIndex())
            {
                case 0:
                    ret = ClusterOptions.UseApiToken
                            ? string.IsNullOrEmpty(ClusterOptions.ApiToken)
                                || string.IsNullOrWhiteSpace(ClusterOptions.ApiToken)
                            : string.IsNullOrEmpty(ClusterOptions.ApiCredential.Username)
                                || string.IsNullOrWhiteSpace(ClusterOptions.ApiCredential.Username);

                    if (ret) { UINotifier.Show(L[(ClusterOptions.UseApiToken ? "Api Token" : "Credential") + " is required!"], UINotifierSeverity.Error); }
                    break;

                case 1:
                    ret = string.IsNullOrEmpty(ClusterOptions.SshCredential.Username)
                            || string.IsNullOrWhiteSpace(ClusterOptions.SshCredential.Username);
                    if (ret) { UINotifier.Show(L["Credential is required!"], UINotifierSeverity.Error); }
                    break;

                case 2:
                    try
                    {
                        //check pve login
                        var client = await PveClientService.GetClient(ClusterOptions);
                        ret = client == null;
                        if (!ret && !await PveClientService.CheckIsValidVersion(client!))
                        {
                            UINotifier.Show(L["Proxmoxm VE version nont valid! Required {0}", PveAdminHelper.MinimalVersion], UINotifierSeverity.Error);
                        }
                        else
                        {
                            //get cluster name
                            if (client != null)
                            {
                                var info = await client.GetClusterInfo();
                                ClusterOptions.Name = info.Name;
                                ClusterOptions.Type = info.Type;
                            }

                            UINotifier.Show(!ret,
                                            L["Successful connection!"],
                                            L["Problem connection!"],
                                            L["Test connection Proxmox VE cluster!"]);
                        }
                    }
                    catch (Exception ex)
                    {
                        UINotifier.Show(ex.Message, UINotifierSeverity.Error);
                        ret = true;
                    }
                    break;

                case 3:
                    await RefForm!.Validate();
                    ret = !RefForm.IsValid;
                    break;

                default: break;
            }
        }

        WritableOptionsService.Update(AdminOptions.Value);

        return ret;
    }

    private async Task ResultStepClick() => await BrowserService.Open(NavigationManager.Uri.ToString(), "_self");
}