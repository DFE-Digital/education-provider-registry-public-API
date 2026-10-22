module "sql" {
  source = "./vendor/modules/azure//azure/sql"

  server_name_suffix    = "edubase"
  environment           = var.environment
  azure_resource_prefix = var.azure_resource_prefix
  service_name          = var.service_name
  service_short         = var.service_short
  config_short          = var.config_short

  key_vault_id = module.infrastructure_secrets.key_vault_id

  public_network_access_enabled = false
  
}