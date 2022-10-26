using BlazorEcommerce.Shared;
using System.Net.Http;

namespace BlazorEcommerce.Client.Services
{
    public class CategoryService : ICategoryService
    {

        private readonly HttpClient _http;

        public event Action OnChange;

        public List<Category> Categories { get; set; }
        public List<Category> AdminCategories { get; set;}

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

        public async Task GetAdminCategories()
        {
            var res =
 await _http.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/Category/admin");

            if (res != null && res.Data != null)
                AdminCategories = res.Data;
        }

        public async Task AddCategory(Category category)
        {
            
             var result=    await _http.PostAsJsonAsync("api/category/admin", category);
            AdminCategories = result.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>().Result.Data;
            await GetCategories();
            OnChange.Invoke();
        }

        public async Task UpdateCategory(Category category)
        {
            var response = await _http.PutAsJsonAsync("api/Category/admin", category);
            AdminCategories = (await response.Content
                .ReadFromJsonAsync<ServiceResponse<List<Category>>>()).Data;
            await GetCategories();
            OnChange.Invoke();
        }

        public async Task DeleteCategory(int categoryId)
        {
            var response = await _http.DeleteAsync($"api/Category/admin/{categoryId}");
            AdminCategories = (await response.Content
                .ReadFromJsonAsync<ServiceResponse<List<Category>>>()).Data;
            await GetCategories();
            OnChange.Invoke();
        }

        public Category CreateNewCategory()
        {
            var newCategory = new Category { IsNew = true, Editing = true }; //strange bug fix
            AdminCategories.Add(newCategory);
            OnChange.Invoke();
            return newCategory;
        }
    }
}
