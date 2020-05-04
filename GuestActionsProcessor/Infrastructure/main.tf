provider azurerm {
  version = "~> 2.8"
  features {}
}

locals {
  eventHubName = "${lower(replace(var.resourceGroupName,"-",""))}eventhub"
}

resource azurerm_resource_group "group" {
  location = var.location
  name = var.resourceGroupName
}

resource azurerm_eventhub_namespace "eventHubNamespace" {
  name = local.eventHubName
  resource_group_name = azurerm_resource_group.group.name
  location = azurerm_resource_group.group.location
  sku = "Basic"
  capacity = 1
}

resource azurerm_eventhub "eventHub" {
  name = "guestActions"
  namespace_name = azurerm_eventhub_namespace.eventHubNamespace.name
  resource_group_name = azurerm_eventhub_namespace.eventHubNamespace.resource_group_name

  partition_count = 1
  message_retention = 1
}


