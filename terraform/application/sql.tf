module "sql" {
  source = "./vendor/modules/aks//aks/sql"

  namespace                                     = var.namespace
  server_name_suffix                            = "edubase"
  environment                                   = var.environment
  azure_resource_prefix                         = var.azure_resource_prefix
  service_name                                  = var.service_name
  service_short                                 = var.service_short
  config_short                                  = var.config_short
  use_azure                                     = var.deploy_azure_backing_services
  cluster_configuration_map                     = module.cluster_data.configuration_map
  azure_enable_backup_storage                   = var.enable_sql_backup_storage
  express_vulnerability_assessment_enabled      = true
  azure_backup_storage_private_endpoint_enabled = true
  azure_sql_private_endpoint_enabled            = true

  key_vault_id = module.infrastructure_secrets.key_vault_id

  public_network_access_enabled = false
}
