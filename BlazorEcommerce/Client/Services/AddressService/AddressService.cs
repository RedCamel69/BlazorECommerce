using BlazorEcommerce.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using static System.Net.WebRequestMethods;

namespace BlazorEcommerce.Client.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly HttpClient _http;
        private readonly IAuthService _authService;
        private readonly NavigationManager _navigationManager;

        public AddressService(HttpClient httpClient,
            IAuthService authService,
            NavigationManager navigationManager)
        {
            _http = httpClient;
            _authService = authService;
            _navigationManager = navigationManager;
        }

        public async Task<Address> AddOrUpdateAddress(Address address)
        {
          
            var result = await _http.PostAsJsonAsync("api/address",address);
            var response = result.Content.ReadFromJsonAsync<ServiceResponse<Address>>().Result.Data;
            return response;
        }

        public async Task<Address> GetAddress()
        {

            var result = await _http.GetFromJsonAsync<ServiceResponse<Address>>("api/address");
            return result.Data;
        }
    }
}
