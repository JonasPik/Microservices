using System;

namespace Play.Inventory.Service.Dtos
{
    public record GrantItemsDto(Guid UserId, Guid CatalogueItemId, int Quantity);
    public record InventoryItemDto(Guid CatalogueItemId, string Name, string Description, int Quantity, DateTimeOffset AcquiredDate);
    public record CatalogueItemDto(Guid Id, string Name, string Description);
}