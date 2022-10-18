namespace BlazorEcommerce.Client.Services
{
    public class CategoryService : ICategoryService
    {

        private readonly HttpClient _http;

        public List<Category> Categories { get; set; }


        public CategoryService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task GetCategories()
        {
            var res =
             await _http.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/Category");

            if (res != null && res.Data != null)
                Categories = res.Data;
        }


    }
}
