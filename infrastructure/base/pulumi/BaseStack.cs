using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using System.Collections.Generic;
using Azure = Pulumi.AzureNative;
public class BaseStack : Stack
{
    private const string Name = "cistilec";

    private readonly StackConfiguration _configuration;
    public BaseStack()
    {
        _configuration = new StackConfiguration();

        var resourceGroup = CreateResourceGroup($"rg-{Name}-{_configuration.Environment}");

        //User Assigned Managed Identity
        var userAssignedIdentity = new Azure.ManagedIdentity.UserAssignedIdentity(
            "myUserAssignedIdentity", 
            new Azure.ManagedIdentity.UserAssignedIdentityArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Location = _configuration.Region,
                Tags = _configuration.Tags,
            });
        var principalId = userAssignedIdentity.PrincipalId;

        var exampleAssignment = new Azure.Authorization.RoleAssignment(
            "exampleAssignment", 
            new()
            {
                Scope = "/subscriptions/d0f36a26-d10f-4b60-8bc9-05b8087c19b2",
                RoleDefinitionId = "/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c",
                PrincipalId = principalId//.Apply(getClientConfigResult => getClientConfigResult),
            });

    }

    private ResourceGroup CreateResourceGroup(string resourceGroupName)
    {
        return new ResourceGroup(
            resourceGroupName,
            new ResourceGroupArgs
            {
                ResourceGroupName = resourceGroupName,
                Location = _configuration.Region,
                Tags = _configuration.Tags
            });
    }
}