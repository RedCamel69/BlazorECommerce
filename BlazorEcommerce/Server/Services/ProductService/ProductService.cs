namespace BlazorEcommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;
        public ProductService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
              .Where(x => x.Featured == true && !x.Deleted && x.Visible)
              .Include(p => p.Variants.Where(v=>!v.Deleted && v.Visible))
              .ToListAsync()
            };

            return response;
        }



        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();
            //var product = await _context.Products.FindAsync(productId);
            var product = await _context.Products
                .Include(p => p.Variants.Where(v=>v.Visible && !v.Deleted))
                .ThenInclude(v => v.ProductType)
                .FirstOrDefaultAsync(p => p.Id == productId && !p.Deleted && p.Visible);

            if (product == null)
            {
                response.Success = false;
                response.Message = "Sorry this product does not exist";
            }
            else
            {
                response.Success = true;
                response.Data = product;
            }

            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProducts()
        {

            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                    .Where(p=>!p.Deleted && p.Visible)
                    .Include(_ => _.Variants.Where(v=>!v.Deleted && v.Visible))
                    .ToListAsync()

            };

            return response;


        }

        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                .Where(p => p.Category.Url.ToLower() == categoryUrl.ToLower() && !p.Deleted && p.Visible)
                 .Include(_ => _.Variants.Where(v => !v.Deleted && v.Visible))
                .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
        {
            var products = await FindProductsBySearchText(searchText);

            List<string> result = new();

            foreach (var product in products)
            {
                if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product.Title);
                }

                if (product.Description != null)
                {
                    var punctuation = product.Description
                        .Where(char.IsPunctuation)
                        .Distinct().ToArray();
                    var words = product.Description.Split()
                        .Select(s => s.Trim(punctuation));

                    foreach (var word in words)
                    {
                        if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            && !result.Contains(word))
                        {
                            result.Add(word);
                        }
                    }
                }
            }

            return new ServiceResponse<List<string>>
            {
                Data = result
            };
        }

        public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(string searchText, int page)
        {

            var pageResults = 2f;
            var pageCount = Math.Ceiling((await FindProductsBySearchText(searchText)).Count / pageResults);

            var products = await _context.Products
                            .Where(p => p.Title.ToLower().Contains(searchText.ToLower())
                            ||
                            p.Description.ToLower().Contains(searchText.ToLower())
                            && !p.Deleted && p.Visible)
                            .Include(p => p.Variants)
                            .Skip((page - 1) * (int)pageResults)
                            .Take((int)pageResults)
                            .ToListAsync();

            var response = new ServiceResponse<ProductSearchResult>
            {
                Data = new ProductSearchResult
                {
                    Products = products,
                    CurrentPage = page,
                    Pages = (int)pageCount
                }
            };

            return response;
        }

        private async Task<List<Product>> FindProductsBySearchText(string searchText)
        {
            return await _context.Products
                            .Where(p => p.Title.ToLower().Contains(searchText.ToLower())
                                ||
                                p.Description.ToLower().Contains(searchText.ToLower())
                                && !p.Deleted && p.Visible
                                )
                            .Include(p => p.Variants)
                            .ToListAsync();
        }
    }
}
