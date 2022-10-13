using Microsoft.AspNetCore.Components;

namespace BlazorEcommerce.Client.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public OrderService(HttpClient httpClient,
            AuthenticationStateProvider authStateProvider,
            NavigationManager navigationManager)
        {
            _http = httpClient;
            _authenticationStateProvider = authStateProvider;
            _navigationManager = navigationManager;
        }

        public AuthenticationStateProvider AuthStateProvider { get; }

        public async Task PlaceOrder()
        {
           if (await IsUserAuthenticated())
            {
                await _http.PostAsync("api/order", null);
            }
            else
            {
                _navigationManager.NavigateTo("login");
            }
        }

        private async Task<bool> IsUserAuthenticated()
        {
            return (await _authenticationStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }
    }
}
