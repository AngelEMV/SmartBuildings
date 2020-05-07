# SmartBuildings

## Introduction:

PoC of hypotethical smart building system.

We start from the hypothesis of a complete booking system. 

When a client books a room in the SmartBuilding system he/she should have access to an interanal mobile app to perform Checkin/Checkout, open room doors, manage room gadgets (climate, TV, etc...), access to restricted areas (Gym, Simming Pool, etc...). Also the building itself can send events of status, temperatures, etc...

```
The whole booking system is supposed to exist, and is out of the scope of this PoC.
```

To emulate the behavior of a user we created a system as follows:

![Diagram](https://github.com/AngelEMV/SmartBuildings/blob/master/Assets/Diagram1.png "Diagram")

An Azure function will emulate user actions, sending events to an Azure EventHub. One or more Azure Functions will be subscribed to the different hubs to precess events and perform different actions. In this example case it will store event information in an CosmosDB and emulate calling 3rd party services, or raising more events.

## Steps to Run:

## 1) Infrastructure Creation:

Dependencies:

As the infrastucture is created in Azure you have to be connected to an Azure subscription (It doesn't matter if is free or pay as you go). 

Secondly infrastructure scripts are done using [Hashicorp](https://www.hashicorp.com/ "Hashicorp")'s tool called [Terraform](https://www.terraform.io/ "Terraform"), so you must have to have it installed before running the scripts.

Steps:

Place yourself inside Infrastructure folder of GuestActionsProcessor and run ``` terraform apply ```

A message like this will appear if everything goes well:

![Diagram](https://github.com/AngelEMV/SmartBuildings/blob/master/Assets/Infra_TerraformApplyComplete.png "Diagram")

Now if you go to your Azure portal you will see that a resource group was created:

![Diagram](https://github.com/AngelEMV/SmartBuildings/blob/master/Assets/Infra_ResourceGroup.png "Diagram")

And inside of it an EventHub and a CosmosDB will appear:

![Diagram](https://github.com/AngelEMV/SmartBuildings/blob/master/Assets/Infra_Content.png "Diagram")

Once infrastructure is created you need to copy both connection strings, and place them in your ```local.settings.json``` file to run it locally. 

```
{
  "IsEncrypted": false,

  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",

    "EventHubSettings:ConnectionString": "place here your connection string",
    "EventHubSettings:HubName": "guestactions",

    "CosmosSettings:ConnectionString": "place here your connection string",
    "CosmosSettings:DatabaseName": "Smart-Buildings-DB",
    "CosmosSettings:ContainerName": "GuestActions"
  }
}
```

## 2) Starting Functions:

In this solution functions are organized in two main projects.

- GuestActionsProcessor: This project will be the one with the function in charge of processing events.

- GuestActionsProcessor.Test: This project will be the one with the function in charge of generating dummy events to test the system.

You can run them using Azure Functions CLI (```func start```) or your IDE. They are configured to run in defferent ports to avoid collisions.

Once you run both functions console output will be as follows:

GuestActionsProcessor: 

![Diagram](https://github.com/AngelEMV/SmartBuildings/blob/master/Assets/Running_GuestActionsProcessor.png "Diagram")

This function now will be suscribed and linked hearing from EventHub events

GuestActionsProcessor.Test: 

![Diagram](https://github.com/AngelEMV/SmartBuildings/blob/master/Assets/Running_GuestActionsProcessorTest.png "Diagram")

This function is now exposing and endpoint which will trigger the function

## 3) Sending Events:

Now you can use the test Azure function to generate ramdom events. You can perform this action sending a GET HTTP call to the exposed endpoint. It can be done through curl or Postman, like in this example:

![Diagram](https://github.com/AngelEMV/SmartBuildings/blob/master/Assets/postman.png "Diagram")

The Function will be triggered, and will perform theese actions:
- A Checkin Action
- 10 random Room/Area Accesses
- A Checkout Action

The output from the function console whenever you call it is as follows:

![Diagram](https://github.com/AngelEMV/SmartBuildings/blob/master/Assets/SendingEvents.png "Diagram")

## 3) Receiving Events:

The other function, the one suscribed to EventHub will be automatically triggered and will process every event.

The output of the Function processing events is as follows:

![Diagram](https://github.com/AngelEMV/SmartBuildings/blob/master/Assets/ProcessingEvents.png "Diagram")

## 4) Destroying infrastructure: