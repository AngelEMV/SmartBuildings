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

An Azure function will emulate user actions, sending events to an Azure EventHub. One or more Azure Functions will be subscribed to the different hubs to precess events and perform different actions. In this example case it will store event information in an CosmosDB and emulate calling 3rd party services, or raising more events.s

## Steps to Run:

## 1) Infrastructure Creation:

## 2) Starting Functions:

## 3) Sending Events:

## 4) Destroying infrastructure: