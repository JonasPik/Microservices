using System.Threading.Tasks;
using MassTransit;
using Play.Catalogue.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogueItemDeletedConsumer : IConsumer<CatalogueItemDeleted>
    {
        private readonly IRepository<CatalogueItem> repository;

        public CatalogueItemDeletedConsumer(IRepository<CatalogueItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogueItemDeleted> context)
        {
            var message = context.Message;

            var item = await repository.GetAsync(message.ItemId);
            if (item == null)
            {
                return;
            }
            else
            {
                await repository.RemoveAsync(message.ItemId);
            }
        }
    }
}