# Azure Media Services Re-Encryption and Publishing Tool

## Description 
Imagine that you already have your assets uploaded into AMS. Now, you need to change the encryption policies, 
and/or delivery policies for all the existing assets. That process tends to be slow, manual, and error prone. 

The objective of this tool is to ease the process of unpublishing, adding content encryption and publishing a 
set of assets to Azure Media Services. 

**NOTE**: This tool has been designed to hook up the existing assets into the VOD Workflow created by using the [Azure Functions for Azure Media Services](https://github.com/johnnyhalife/azamsfunctions-v2/).


## Requirements 
In order to run this tool you must provide the following configuration Settings on App.config file.

```
    <!-- Azure Media Services account credentials -->
    <add key="AMSAADTenantDomain" value="" />
    <add key="AMSRESTAPIEndpoint" value="" />
    <add key="AMSRESTAPIClientId" value="" />
    <add key="AMSRESTAPIClientSecret" value="" />
    
    <!-- VOD Workflow Storage Account Configuration -->
    <add key="MediaStorageAccount" value=""/>
    <add key="ContentProtectionJobsQueueName" value=""/>
```


* **AMSAADTenantDomain**. The Azure Media Services Active Directory Tenant Domain. 
* **AMSRESTAPIEndpoint**. The Azure Media Services REST API Endpoint. 
* **AMSRESTAPIClientId**. The Azure Media Services Client ID (Service Principal AuthN).
* **AMSRESTAPIClientSecret**. The Azure Media Services Client Secret (Service Principal AuthN).
* **MediaStorageAccount**. Azure Storage Account used on [Azure Functions for Azure Media Services](https://github.com/johnnyhalife/azamsfunctions-v2/).
* **ContentProtectionJobsQueueName**. Azure Storage Queue Name used on [Azure Functions for Azure Media Services](https://github.com/johnnyhalife/azamsfunctions-v2/) for triggering the AddContentProtectionFunction.

## Remarks 
On `Program.cs` [there's a line used for retrieving the assets](https://github.com/johnnyhalife/azure-media-services-re-encryption-and-publishing-tool/blob/master/Program.cs#L27). As-is it's currently configured to retrieve all the assets within the account. If you 
need to specify a limited set of Assets just add a LINQ `Where` to filter the desired assets. 

`var assets = context.Assets;`

---
