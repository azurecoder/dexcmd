using CommandLine;
using Microsoft.Extensions.Configuration;

namespace dexcmd
{
   public class Options
   {
      public string ClientId { get; set; }
      public string InteractiveClientId { get; set; }
      public string ClientSecret { get; set; }
      public string TenantId { get; set; }
      public string KustoClusterName { get; set; }
      public string ResourceGroup { get; set; }
      public string SubscriptionId { get; set; }
      [Option("list-databases", Required = false, HelpText = "Gets a list of databases")]
      public bool ListDatabases { get; set; }
      [Option("list-tables", Required = false, HelpText = "Gets a list of tables")]
      public bool ListTables { get; set; }
      [Option('d', "databaseName", Required = false, HelpText = "The name of the database to query")]
      public string DatabaseName { get; set; }
      [Option('t', "tableName", Required = false, HelpText = "The name of a table to query")]
      public string TableName { get; set; }
      [Option('u', "userName", Required = false, HelpText = "Adds user to a particular table")]
      public string UserName { get; set; }

      internal static Options GetConfigArguments(Options options)
      {
         IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("kusto.json", true, true)
            .Build();
         options.ClientId = config["application_id"];
         options.ClientSecret = config["application_key"];
         options.ResourceGroup = config["resource_group"];
         options.SubscriptionId = config["subscription_id"];
         options.TenantId = config["tenant_id"];
         options.KustoClusterName = config["cluster_name"];
         options.InteractiveClientId = config["interactive_application_id"];
         return options;
      }
   }
}