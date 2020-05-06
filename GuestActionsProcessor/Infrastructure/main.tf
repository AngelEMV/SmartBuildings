provider azurerm {
  version = "~> 2.8"
  features {}
}

locals {
  prefix = "${lower(replace(var.resourceGroupName,"-",""))}"
  eventHubName = "${local.prefix}eventhub"
}

resource "azurerm_resource_group" "group" {
  location = "${var.location}"
  name = "${var.resourceGroupName}"
}

resource "azurerm_eventhub_namespace" "eventHubNamespace" {
  name = "${local.eventHubName}"
  resource_group_name = "${azurerm_resource_group.group.name}"
  location = "${azurerm_resource_group.group.location}"
  sku = "Basic"
  capacity = 1
}

resource "azurerm_eventhub" "eventHub" {
  name = "guestActions"
  namespace_name = "${azurerm_eventhub_namespace.eventHubNamespace.name}"
  resource_group_name = "${azurerm_eventhub_namespace.eventHubNamespace.resource_group_name}"

  partition_count = 1
  message_retention = 1
}

resource "random_integer" "ri" {
  min = 10000
  max = 99999
}

resource "azurerm_cosmosdb_account" "cosmos-db-account" {
  name                = "${local.prefix}-cosmos-db-${random_integer.ri.result}"
  location            = "${azurerm_resource_group.group.location}"
  resource_group_name = "${azurerm_resource_group.group.name}"
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"

  consistency_policy {
    consistency_level       = "BoundedStaleness"
    max_interval_in_seconds = 10
    max_staleness_prefix    = 200
  }

  geo_location {
    prefix            = "${local.prefix}-custom-id-${random_integer.ri.result}"
    location          = "${azurerm_resource_group.group.location}"
    failover_priority = 0
  }
}

resource "azurerm_cosmosdb_sql_database" "cosmos-db" {
  name = "Smart-Buildings-DB"
  resource_group_name = "${azurerm_cosmosdb_account.cosmos-db-account.resource_group_name}"
  account_name = "${azurerm_cosmosdb_account.cosmos-db-account.name}"
}

resource "azurerm_cosmosdb_sql_container" "cosmos-db-container" {
  name = "GuestActions"
  resource_group_name = "${azurerm_cosmosdb_account.cosmos-db-account.resource_group_name}"
  account_name = "${azurerm_cosmosdb_account.cosmos-db-account.name}"
  database_name = "${azurerm_cosmosdb_sql_database.cosmos-db.name}"
  partition_key_path = "/ActionID"
}