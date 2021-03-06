﻿using System.Configuration;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DFC.Configuration.AzureTableStorage
{
    public class AzureTableStorageConfigurationRepository : IConfigurationRepository
    {
        private CloudStorageAccount _storageAccount;

        public AzureTableStorageConfigurationRepository() : this(ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"])
        {
        }
        public AzureTableStorageConfigurationRepository(string storageConnectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        }

        public string Get(string serviceName, string environmentName, string version)
        {
            var table = GetTable();
            var operation = GetOperation(serviceName, environmentName, version);
            var result = table.ExecuteAsync(operation).Result;

            var configItem = (ConfigurationItem)result.Result;
            return configItem?.Data;
        }
        public async Task<string> GetAsync(string serviceName, string environmentName, string version)
        {
            var table = GetTable();
            var operation = GetOperation(serviceName, environmentName, version);
            var result = await table.ExecuteAsync(operation);

            var configItem = (ConfigurationItem)result.Result;
            return configItem?.Data;
        }


        private CloudTable GetTable()
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference("Configuration");
        }
        private TableOperation GetOperation(string serviceName, string environmentName, string version)
        {
            return TableOperation.Retrieve<ConfigurationItem>(environmentName, $"{serviceName}_{version}" );
        }
    }
}