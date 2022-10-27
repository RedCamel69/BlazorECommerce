using BlazorEcommerce.Shared;

namespace BlazorEcommerce.Server.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly DataContext _context;

        public ProductTypeService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<ProductType>>> AddProductType(ProductType productType)
        {
            productType.Editing = productType.IsNew = false;
            _context.ProductTypes.Add(productType);
            await _context.SaveChangesAsync();
            return await GetProductTypes();
        }

        public async Task<ServiceResponse<List<ProductType>>> DeleteProductType(int id)
        {
            ProductType? productType = _context.ProductTypes.FirstOrDefault(x => x.Id == id);
            if (productType == null)
            {
                return new ServiceResponse<List<ProductType>>
                {
                    Success = false,
                    Message = "Category not found."
                };
            }

           _context.ProductTypes.Remove(productType);
            await _context.SaveChangesAsync();
            return await GetProductTypes();
        }

        public async Task<ServiceResponse<List<ProductType>>> GetProductTypes()
        {

            var productTypes = await _context.ProductTypes
               .ToListAsync();
            return new ServiceResponse<List<ProductType>>()
            {
                Data = productTypes
            };
        }

        public async Task<ServiceResponse<List<ProductType>>> UpdateProductType(ProductType productType)
        {
            var dbProductType = _context.ProductTypes.FirstOrDefault(_context => _context.Id == productType.Id);
            if (dbProductType == null)
            {
                return new ServiceResponse<List<ProductType>>
                {
                    Success = false,
                    Message = "Product Type not found."
                };
            }

            dbProductType.Name = productType.Name;

            await _context.SaveChangesAsync();

            return await GetProductTypes();
        }
    }
}
