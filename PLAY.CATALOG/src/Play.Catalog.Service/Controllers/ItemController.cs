using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers;

[Route("items")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IItemRepository itemRepository;
    public ItemController(IItemRepository itemRepository)
    {
        this.itemRepository = itemRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<ItemDto>> Get()
    {
        var items = (await itemRepository.GetAllAsync())
        .Select(item => item.AsDto());
        return items;
    }


    [HttpGet("{id}")]
    public async Task<ItemDto> GetById(Guid id)
    {
        var item = (await itemRepository.GetItemAsync(id)).AsDto();
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostItem(CreateItemDTO createItemDTO)
    {
        var item = new Item
        {
            Name = createItemDTO.Name,
            Description = createItemDTO.Description,
            Price = createItemDTO.Price,
            CreateDate = DateTimeOffset.UtcNow
        };

        await itemRepository.CreateAsync(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item.AsDto());
    }

    [HttpPut("{id}")]

    public async Task<ActionResult> PutAsync(Guid id, UpdateItemDTO updateItemDTO)
    {

        var existingItem = await itemRepository.GetItemAsync(id);
        if (existingItem == null)
        {
            return NotFound();
        }

        existingItem.Name = updateItemDTO.Name;
        existingItem.Price = updateItemDTO.Price;
        existingItem.Description = updateItemDTO.Description;
        await itemRepository.UpdateAsync(existingItem);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async  Task<IActionResult> Delete(Guid id)
    {
        var existingItem = await itemRepository.GetItemAsync(id);
             if (existingItem == null)
        {
            return NotFound();
        }
        await itemRepository.RemoveAsync(id);
         return NoContent();
    }

}