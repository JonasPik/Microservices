using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private const string AdminRole = "Admin";

        private readonly IRepository<InventoryItem> inventoryItemsRepository;
        private readonly IRepository<CatalogueItem> catalogueItemsRepository;

        public ItemsController(IRepository<InventoryItem> itemsRepository,
        IRepository<CatalogueItem> catalogueItemsRepository)
        {
            this.inventoryItemsRepository = itemsRepository;
            this.catalogueItemsRepository = catalogueItemsRepository;
        }

        // GET /items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var currentUserId = User.FindFirstValue("sub");
            if (Guid.Parse(currentUserId) != userId)
            {
                if (!User.IsInRole(AdminRole))
                {
                    return Unauthorized();
                }
            }

            var inventoryItemEntities = await inventoryItemsRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItemEntities.Select(item => item.CatalogueItemId);
            var catalogueItemEntities = await catalogueItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
            {
                var catalogueItem = catalogueItemEntities.Single(catalogueItem => catalogueItem.Id == inventoryItem.CatalogueItemId);
                return inventoryItem.AsDto(catalogueItem.Name, catalogueItem.Description);
            });

            return Ok(inventoryItemDtos);
        }

        // POST /items
        [HttpPost]
        [Authorize(Roles = AdminRole)]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await inventoryItemsRepository.GetAsync(item => item.UserId == grantItemsDto.UserId
                && item.CatalogueItemId == grantItemsDto.CatalogueItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem()
                {
                    CatalogueItemId = grantItemsDto.CatalogueItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await inventoryItemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await inventoryItemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}