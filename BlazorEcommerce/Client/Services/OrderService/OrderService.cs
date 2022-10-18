using Microsoft.AspNetCore.Components;

namespace BlazorEcommerce.Client.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _http;
        private readonly IAuthService _authService;
        private readonly NavigationManager _navigationManager;
      
        public OrderService(HttpClient httpClient,
            IAuthService authService,
            NavigationManager navigationManager)
        {
            _http = httpClient;
            _authService = authService;
            _navigationManager = navigationManager;
        }

        public AuthenticationStateProvider AuthStateProvider { get; }

        public  async Task<List<OrderOverviewResponse>> GetOrders()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<OrderOverviewResponse>>>("api/order");
            return result.Data;
        }


        public async Task<OrderDetailsResponse> GetOrderDetails(int orderId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<OrderDetailsResponse>>("api/order/" + orderId);
            return result.Data;
        }

        public async Task PlaceOrder()
        {
            if (await _authService.IsUserAuthenticated())
            {
                await _http.PostAsync("api/order", null);
            }
            else
            {
                _navigationManager.NavigateTo("login");
            }
        }

      
    }
}
