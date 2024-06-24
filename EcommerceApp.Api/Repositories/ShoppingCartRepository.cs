
using Microsoft.EntityFrameworkCore;
using EcommerceApp.Api.Data;
using EcommerceApp.Api.Entities;
using EcommerceApp.Api.Repositories.Contracts;
using EcommerceApp.Models.Dtos;

namespace EcommerceApp.Api.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly EcommerceAppDbContext dbContext;

        public ShoppingCartRepository(EcommerceAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private async Task<bool> CartItemExists(int cartId, int productId)
        {
            return await this.dbContext.CartItems.AnyAsync(c => c.CartId == cartId &&
                                                                     c.ProductId == productId);

        }
        public async Task<CartItem> AddItem(CartItemToAddDto cartItemToAddDto)
        {
            if (await CartItemExists(cartItemToAddDto.CartId, cartItemToAddDto.ProductId) == false)
            {
                var item = await (from product in this.dbContext.Products
                                  where product.Id == cartItemToAddDto.ProductId
                                  select new CartItem
                                  {
                                      CartId = cartItemToAddDto.CartId,
                                      ProductId = product.Id,
                                      Qty = cartItemToAddDto.Qty
                                  }).SingleOrDefaultAsync();

                if (item != null)
                {
                    var result = await this.dbContext.CartItems.AddAsync(item);
                    await this.dbContext.SaveChangesAsync();
                    return result.Entity;
                }
            }

            return null;

        }

        public async Task<CartItem> DeleteItem(int id)
        {
            var item = await this.dbContext.CartItems.FindAsync(id);

            if (item != null)
            {
                this.dbContext.CartItems.Remove(item);
                await this.dbContext.SaveChangesAsync();
            }
            
            return item;

        }

        public async Task<CartItem> GetItem(int id)
        {
            return await (from cart in this.dbContext.Carts
                          join cartItem in this.dbContext.CartItems
                          on cart.Id equals cartItem.CartId
                          where cartItem.Id == id
                          select new CartItem
                          {
                              Id = cartItem.Id,
                              ProductId = cartItem.ProductId,
                              Qty = cartItem.Qty,
                              CartId = cartItem.CartId
                          }).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<CartItem>> GetItems(int userId)
        {
            return await (from cart in this.dbContext.Carts
                          join cartItem in this.dbContext.CartItems
                          on cart.Id equals cartItem.CartId
                          where cart.UserId == userId
                          select new CartItem
                          {
                              Id = cartItem.Id,
                              ProductId = cartItem.ProductId,
                              Qty = cartItem.Qty,
                              CartId = cartItem.CartId
                          }).ToListAsync();
        }

        public async Task<CartItem> UpdateQty(int id, CartItemQtyUpdateDto cartItemQtyUpdateDto)
        {
            var item = await this.dbContext.CartItems.FindAsync(id);

            if (item != null)
            {
                item.Qty = cartItemQtyUpdateDto.Qty;
                await this.dbContext.SaveChangesAsync();
                return item;
            }

            return null;
        }
    }
}
