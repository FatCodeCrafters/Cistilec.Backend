using Pulumi;
using System.Collections.Generic;

/// <summary>
/// [Configuration | Pulumi Concepts | Pulumi Docs](https://www.pulumi.com/docs/concepts/config/)
/// </summary>
public class StackConfiguration
{
    private Dictionary<string, string> _tags = [];

    protected readonly Config StackConfig;

    protected readonly Config AzureStackConfig;

    public string Environment => StackConfig.Require("environment");

    public string ResourceGroup => StackConfig.Require("resourcegroup");

    public string Region => AzureStackConfig.Require("region");

    public Dictionary<string, string> Tags
    {
        get
        {
            if (_tags.Count != 0) { return _tags; }
            
            _tags = StackConfig.RequireObject<Dictionary<string, string>>("tags");
            _tags.Add("environment", Environment);
            _tags.Add("createdBy", "Pulumi");
            
            return _tags;
        }
    }
    public StackConfiguration()
    {
        StackConfig = new Config();
        AzureStackConfig = new Config("azure-native");
    }

    public bool IsEnvironment(string environment)
    {
        return Environment == environment;
    }
}