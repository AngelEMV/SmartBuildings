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

Secondly infrastructure scripts are done using [Hashicorp](https://www.hashicorp.com/ "Hashicorp")'s tool called [Hashicorp](https://www.terraform.io/ "Hashicorp"), so you must have to have it installed before running the scripts.

Steps:

Place yourself inside Infrastructure folder of GuestActionsProcessor and run ``` terraform apply ```

Amessage like this will appear if everything goes well:

![Diagram](https://github.com/AngelEMV/SmartBuildings/blob/master/Assets/Infra_TerraformApplyComplete.png "Diagram")

Now if you go to your Azure portal you will see that a resource group was created:

![Diagram](https://github.com/AngelEMV/SmartBuildings/blob/master/Assets/Infra_ResourceGroup.png "Diagram")

And inside of it an EventHub and a CosmosDB will appear:

![Diagram](https://github.com/AngelEMV/SmartBuildings/blob/master/Assets/Infra_Content.png "Diagram")

## 2) Starting Functions:

## 3) Sending Events:

## 3) Receiving Events:

## 4) Destroying infrastructure: