using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace AzureMediaServicesReEncryptionAndPublishingTool
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Azure Media Services Re-Encryption and Publishing Tool");
                Console.WriteLine("=======================================");
                Console.WriteLine();

                Console.WriteLine("Creating Azure Media Services context...");
                var context = CreateCloudMediaContext();
                Console.WriteLine("Azure Media Services context created.");

                Console.WriteLine("Retrieving assets from Azure Media Services context...");
                var assets = context.Assets;
                Console.WriteLine("Assets retrieved from Azure Media Services context...");

                foreach (var asset in assets)
                {
                    Console.WriteLine($"\tProcessing asset: {asset.Name}...");

                    // Prepare asset for re-encryption and publishing
                    await RemoveLocatorAndEncryption(asset);

                    // Pipe asset into a VOD Workflow (through AddContentProtection invokation)
                    await TriggerContentProtectionWorkflow(asset);

                    Console.WriteLine($"\tAsset processing completed for: {asset.Name}...");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: ${ex.ToString()}");
            }

            Console.WriteLine("Done.");
            Console.ReadKey();
        }

        private static async Task TriggerContentProtectionWorkflow(IAsset asset)
        {
            var storage = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["MediaStorageAccount"]);
            var client = storage.CreateCloudQueueClient();

            var contentProtectionQueue = client.GetQueueReference(ConfigurationManager.AppSettings["ContentProtectionJobsQueueName"]);
            await contentProtectionQueue.CreateIfNotExistsAsync();

            await contentProtectionQueue.AddMessageAsync(new CloudQueueMessage(asset.Id));
        }

        private static async Task RemoveLocatorAndEncryption(IAsset asset)
        {
            // Delete Locators
            var deleteLocatorOperations = new List<Task>();
            asset.Locators.ToList().ForEach(locator => deleteLocatorOperations.Add(locator.DeleteAsync()));
            Task.WaitAll(deleteLocatorOperations.ToArray());

            // Remove delivery policies and content encryption keys
            asset.DeliveryPolicies.ToList().ForEach(policy => asset.DeliveryPolicies.Remove(policy));
            asset.ContentKeys.ToList().ForEach(contentKey => asset.ContentKeys.Remove(contentKey));

            await asset.UpdateAsync();
        }

        private static CloudMediaContext CreateCloudMediaContext()
        {
            var tenant = ConfigurationManager.AppSettings["AMSAADTenantDomain"];
            var endpoint = ConfigurationManager.AppSettings["AMSRESTAPIEndpoint"];
            var clientId = ConfigurationManager.AppSettings["AMSRESTAPIClientId"];
            var clientSecret = ConfigurationManager.AppSettings["AMSRESTAPIClientSecret"];

            var tokenCredentials = new AzureAdTokenCredentials(tenant,
                new AzureAdClientSymmetricKey(clientId, clientSecret),
                AzureEnvironments.AzureCloudEnvironment);

            var tokenProvider = new AzureAdTokenProvider(tokenCredentials);

            return new CloudMediaContext(new Uri(endpoint), tokenProvider);
        }
    }
}
