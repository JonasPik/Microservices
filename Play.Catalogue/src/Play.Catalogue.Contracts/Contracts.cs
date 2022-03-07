using System;

namespace Play.Catalogue.Contracts
{
    public record CatalogueItemCreated(Guid ItemId, string Name, string Description);
    public record CatalogueItemUpdated(Guid ItemId, string Name, string Description);
    public record CatalogueItemDeleted(Guid ItemId);
}