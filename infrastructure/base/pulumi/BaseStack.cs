using Pulumi;
using Pulumi.AzureNative.AppConfiguration;
using Pulumi.AzureNative.AppConfiguration.Inputs;
using Pulumi.AzureNative.ManagedIdentity;
using Pulumi.AzureNative.Resources;
using System.Collections.Generic;
using Azure = Pulumi.AzureNative;
using SkuArgs = Pulumi.AzureNative.AppConfiguration.Inputs.SkuArgs;
public class BaseStack : Stack
{
    private const string Name = "cistilec";

    private readonly StackConfiguration _configuration;
    
    private Output<string> _userManagedIdentityId;
    private Output<string> _userManagedIdentityClientId;
    private Output<string> _userManagedIdentityName;

    private Output<string> _configurationStoreName;
    private Output<string> _configurationStoreEndpoint;

    public BaseStack()
    {
        _configuration = new StackConfiguration();
        
        var cistilecUserAssignedIdentity = CreateUserAssignedIdentity(
            "cistilecUserAssignedIdentity",
            "msiusr-cistilec-dev");
        _userManagedIdentityId = cistilecUserAssignedIdentity.Id;
        _userManagedIdentityClientId = cistilecUserAssignedIdentity.ClientId;
        _userManagedIdentityName = cistilecUserAssignedIdentity.Name;

        var configurationStore = CreateConfigurationStore(
            "cistilec-appconf-dev", 
            _userManagedIdentityId);
        _configurationStoreName = configurationStore.Name;
        _configurationStoreEndpoint = configurationStore.Endpoint;
    }

    private UserAssignedIdentity CreateUserAssignedIdentity(
        string resourceName, 
        string userAssignedIdentityName)
    {
        return new UserAssignedIdentity(
            resourceName,
            new UserAssignedIdentityArgs
            {
                Location = _configuration.Region,
                ResourceGroupName = _configuration.ResourceGroup,
                ResourceName = userAssignedIdentityName,
                Tags = _configuration.Tags,
            });
    }
    private ConfigurationStore CreateConfigurationStore(
        string configurationStoreName, Output<string> userManagedIdentityId)
    {
        return new ConfigurationStore(
            configurationStoreName,
            new ConfigurationStoreArgs 
            { 
                ResourceGroupName = _configuration.ResourceGroup,
                Location = _configuration.Region,
                SoftDeleteRetentionInDays = 0,
                Sku = new SkuArgs
                {
                    Name = "Free"
                },
                Identity = new ResourceIdentityArgs
                {
                    Type = IdentityType.UserAssigned,
                    UserAssignedIdentities = userManagedIdentityId
                    .Apply(Id => new List<string> { Id })
                },
                Tags = _configuration.Tags
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