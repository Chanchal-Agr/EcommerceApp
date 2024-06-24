using Microsoft.EntityFrameworkCore;
using EcommerceApp.Api.Data;
using EcommerceApp.Api.Entities;
using EcommerceApp.Api.Repositories.Contracts;

namespace EcommerceApp.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceAppDbContext dbContext;

        public ProductRepository(EcommerceAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IEnumerable<ProductCategory>> GetCategories()
        {
            var categories = await this.dbContext.ProductCategories.ToListAsync();
           
            return categories; 
        
        }

        public async Task<ProductCategory> GetCategory(int id)
        {
            var category = await dbContext.ProductCategories.SingleOrDefaultAsync(c => c.Id == id);
            return category;
        }

        public async Task<Product> GetItem(int id)
        {
            var product = await dbContext.Products
                                .Include(p => p.ProductCategory)
                                .SingleOrDefaultAsync(p => p.Id == id);
            return product;
        }

        public async Task<IEnumerable<Product>> GetItems()
        {
            var products = await this.dbContext.Products
                                     .Include(p => p.ProductCategory).ToListAsync();

            return products;
        
        }

        public async Task<IEnumerable<Product>> GetItemsByCategory(int id)
        {
            var products = await this.dbContext.Products
                                     .Include(p => p.ProductCategory)
                                     .Where(p => p.CategoryId == id).ToListAsync();
            return products;
        }
    }
}
