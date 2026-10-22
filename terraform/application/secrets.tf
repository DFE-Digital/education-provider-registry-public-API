module "infrastructure_secrets" {
  source = "./vendor/modules/azure//azure/secrets"

  azure_resource_prefix = var.azure_resource_prefix
  service_short         = var.service_short
  config_short          = var.config_short
  key_vault_short       = "inf"
}
