#!/usr/bin/env bash

set -euo pipefail

if [[ $# -ne 3 ]]; then
    echo "Usage: $0 <resource-group> <subscription> <entra-group>" >&2
    exit 2
fi

resource_group=$1
subscription=$2
entra_group=$3

if ! command -v az >/dev/null 2>&1; then
    echo "Azure CLI (az) is required" >&2
    exit 1
fi

if ! subscription_id=$(az account show --subscription "$subscription" --query id --output tsv); then
    echo "Unable to resolve subscription: $subscription" >&2
    exit 1
fi

if ! resource_group_scope=$(az group show --name "$resource_group" --subscription "$subscription" --query id --output tsv); then
    echo "Unable to resolve resource group: $resource_group" >&2
    exit 1
fi

if ! group_id=$(az ad group show --group "$entra_group" --query id --output tsv); then
    echo "Unable to resolve Entra group: $entra_group" >&2
    exit 1
fi

if [[ -z $subscription_id || -z $resource_group_scope || -z $group_id ]]; then
    echo "Azure CLI returned an empty subscription, resource group, or Entra group ID" >&2
    exit 1
fi

subscription_scope="/subscriptions/$subscription_id"

if ! assignments=$(az role assignment list \
    --subscription "$subscription" \
    --assignee-object-id "$group_id" \
    --all \
    --include-inherited \
    --query '[].[roleDefinitionName, scope]' \
    --output tsv); then
    echo "Unable to list role assignments for Entra group: $entra_group" >&2
    exit 1
fi

has_role_at_scope() {
    local required_role=${1,,}
    local required_scope=${2,,}
    local assigned_role
    local assigned_scope

    while IFS=$'\t' read -r assigned_role assigned_scope; do
        if [[ ${assigned_role,,} == "$required_role" && ${assigned_scope,,} == "$required_scope" ]]; then
            return 0
        fi
    done <<< "$assignments"

    return 1
}

failed=0

for role in "s189-Contributor and Key Vault editor"; do
    if has_role_at_scope "$role" "$resource_group_scope"; then
        printf 'OK: %s assigned at resource group %s\n' "$role" "$resource_group"
    else
        printf 'MISSING: %s at resource group %s\n' "$role" "$resource_group" >&2
        failed=1
    fi
done

if has_role_at_scope "Reader" "$subscription_scope"; then
    printf 'OK: Reader inherited from subscription %s\n' "$subscription"
else
    printf 'MISSING: Reader at subscription %s (required to inherit at resource group %s)\n' \
        "$subscription" "$resource_group" >&2
    failed=1
fi

exit "$failed"