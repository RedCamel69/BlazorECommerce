﻿using Blazored.LocalStorage;

namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        public event Action OnChange;
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;

        public CartService(ILocalStorageService localStorage, 
            HttpClient httpClient,
            AuthenticationStateProvider authStateProvider )
        {
            _localStorage=localStorage;
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        public async Task AddToCart(CartItem cartItem)
        {
            if (await IsUserAuthenticated())
            {
                await _httpClient.PostAsJsonAsync("api/cart/add", cartItem);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    cart = new List<CartItem>();
                }

                var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId &&
                    x.ProductTypeId == cartItem.ProductTypeId);
                if (sameItem == null)
                {
                    cart.Add(cartItem);
                }
                else
                {
                    sameItem.Quantity += cartItem.Quantity;
                }

                await _localStorage.SetItemAsync("cart", cart);
            }
            await GetCartItemsCount();
        }


        public Task RemoveFromCart(CartItem cartItem)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            if(await IsUserAuthenticated())
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<CartProductResponse>>>("api/cart");
                return response.Data;
            }
            else
            {
                var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cartItems == null)
                {
                    return new List<CartProductResponse>();
                }
                var response = await _httpClient.PostAsJsonAsync("api/cart/products", cartItems);

                var cartProducts =
                    await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

                return cartProducts.Data;

            }

        }

     

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {

            if (await IsUserAuthenticated())
            {
                await _httpClient.DeleteAsync($"api/cart/{productId}/{productTypeId}");
            }

            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

                if (cart == null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == productId
                        && x.ProductTypeId == productTypeId);

                if (cartItem != null)
                {
                    cart.Remove(cartItem);
                    await _localStorage.SetItemAsync("cart", cart);
                   
                }

            }

           // await GetCartItemsCount();

        }

        public async Task UpdateQuantity(CartProductResponse product)
        {
            if (await IsUserAuthenticated())
            {
                var request = new CartItem
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    ProductTypeId = product.ProductTypeId
                };

                await _httpClient.PutAsJsonAsync("api/cart/update-quantity", request);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

                if (cart == null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == product.ProductId
                        && x.ProductTypeId == product.ProductTypeId);

                if (cartItem != null)
                {
                    cartItem.Quantity = product.Quantity;
                    await _localStorage.SetItemAsync("cart", cart);
                }
            }


           
        }

        public async Task StoreCartItems(bool emptyLocalCart)
        {
            var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (localCart == null)
            {
                return;
            }

            await _httpClient.PostAsJsonAsync("api/cart", localCart);

            if (emptyLocalCart)
            {
                await _localStorage.RemoveItemAsync("cart");
            }
        }


        private async Task<bool> IsUserAuthenticated()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }

        public async Task GetCartItemsCount()
        {
            if (await IsUserAuthenticated())
            {
                var result = await _httpClient.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = result.Data;

                await _localStorage.SetItemAsync<int>("cartItemsCount", count);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                await _localStorage.SetItemAsync<int>("cartItemsCount", cart != null ? cart.Count : 0);
            }

            OnChange.Invoke();
        }
    }
}