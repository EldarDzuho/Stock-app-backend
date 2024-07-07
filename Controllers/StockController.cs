using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IStockRepository _stockRepo;
        public StockController(ApplicationDBContext context, IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = await _stockRepo.GetAllAsync();

            var stockDto = stocks.Select(s => s.ToStockDto());

            return Ok(stocks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var stock = await _context.Stocks.FindAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
            
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            var stockModel = stockDto.ToStockFromCreateDto();
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id}, stockModel.ToStockDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            var StockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (StockModel == null)
            {
                return NotFound();
            }

            StockModel.Symbol = updateDto.Symbol;
            StockModel.CompanyName = updateDto.CompanyName;
            StockModel.Purchase = updateDto.Purchase;
            StockModel.LastDiv = updateDto.LastDiv;
            StockModel.Indusrty = updateDto.Indusrty;
            StockModel.MarketCap = updateDto.MarketCap;

            await _context.SaveChangesAsync();

            return Ok(StockModel.ToStockDto());

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var StockModel = await _context.Stocks.FirstOrDefaultAsync(X => X.Id == id);

            if(StockModel == null)
            {
                return NotFound();
            }

            _context.Stocks.Remove(StockModel);

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}