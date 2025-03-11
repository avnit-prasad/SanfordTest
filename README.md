# Azure Function App - HTTP Trigger with Bearer Token Authentication

This repository contains an Azure Function app built with .NET 8.0. The function app is an HTTP-triggered function that is secured using Bearer Token authentication via AuthO as the identity service provider.

## Prerequisites

- .NET 8.0 SDK
- Azure Functions Tools for Visual Studio or Visual Studio Code
- An active Azure subscription
- [Auth0 account](https://auth0.com/) for authentication setup
- To run locally you would also require Azure Functions Core Tools
- Via Postman receive an access token via the url: POST https://dev-ze2w21csgte1mgsi.us.auth0.com/oauth/token
- For the purposes of the test the following body can be passed to the URL
- {
    "client_id" : "hS281NBGBURHXmtl6ftKqBHSqjGdGeFp",
    "client_secret" : "76aBCnN-gMKdBoYqrLqw7KNd9Z4PRCQzmUoU-sM-TiRFcZKFQ-v9pPEoQFDnzcAK",
    "grant_type" : "password",
    "audience" : "https://sandfordtest/",
    "username" : "sanford@test.com",
    "password" : "Test!12345"
}

The endpoint should provide an access token that can be used to access the function app via http://localhost:7071/api/DispatchFunction
set the authorisation header as bearer token.

ensure the local.settings.json is correct below:
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "MaxRequestBodySize": "819200",
    "Auth0:Domain": "https://dev-ze2w21csgte1mgsi.us.auth0.com/",
    "Auth0:ApiIdentifier": "https://sandfordtest/"
  }
}

## Architecture Overview: Azure Function App, Blob Storage, and Logic Apps Integration
This architecture involves an Azure Function App that processes a payload, converts it to CSV format, uploads the CSV to Blob Storage, and then triggers a 
Logic App that moves the file to an SFTP server. 
Below is a detailed explanation of each component and the flow of data between them:

### Components:

Authenication:
For the purposes of this test I have used Auth0 using bearer token to demonstrate the use of Authentication for the Function App. The best practice would be to use
0Auth 2.0 client credentials using SSO this would mean users would not require a seperate process of authentication and provide all the benefits of using Microsofts
identity service and access management.

Azure Function App (HTTP Triggered):

Purpose: The Azure Function App serves as the entry point for incoming requests, processing the payload, and transforming the data into CSV format.
Trigger: The function is triggered by an HTTP request via the "Ready for Dispatch Button" available on the D365 screen.
Data Processing: After the payload is received, the function processes the data parsing JSON and converts it into CSV format with the relevant mapping.
Output: Currently the function app returns a csv file however the correct solution would be to have the function app upload the file to Azure Blob Storage.

Logic Apps

Purpose: Logic Apps automate workflows. In this case, a Logic App is used to watch the Blob Storage container for new files, trigger upon new file creation, and send those files to an SFTP server.
Action: Once a new file is detected, the Logic App moves the file from Blob Storage to the designated SFTP server using the SFTP - Upload File action. 
Keys for the SFTP can be stored in Azure Key Vault and accessed via logic app. 
The benefit of using logic app is simplicity; Logic Apps provides a low-code/no-code environment that makes it easy for non-developers to create workflows. 
You can easily define, configure, and automate the process of transferring files to an SFTP server without writing complex code and also 
providing built-in error handling. Logic Apps comes with built-in retry and error handling mechanisms. If a file upload to the SFTP server fails due to transient issues, Logic Apps can automatically retry the operation. 

## Implementation

The implementation took approximately a day including writing the code for the business logic, setting up authentication and the units tests. 
If I had more time I would make sure the function app was deployed in Azure using Azure DevOps Pipleline and demonstrate the end to end implemntation of the required solution. This would also include setting up the correct authentication, configuring blob storage and logic apps. I would also consider adding integration tests for authenication and file upload components. Authorisation was also not implementation so looking into role based access for the function app to ensure those with relevant permissions are authorised. Also look at ways code can be decoupled so for instance a new 3PL was to be introduced it could be implemented with mimial code changes.
