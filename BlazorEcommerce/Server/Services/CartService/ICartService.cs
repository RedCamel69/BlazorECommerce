﻿namespace BlazorEcommerce.Server.Services.CartService
{
    public interface ICartService
    {
        Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems);

        Task<ServiceResponse<List<CartProductResponse>>> StoreCartProducts(List<CartItem> cartItems);

        Task<ServiceResponse<int>> GetCartItemsCount();

        Task<ServiceResponse<List<CartProductResponse>>> GetDbCartProducts(int? userId = null);

        Task<ServiceResponse<bool>> AddToCart(CartItem cartItem);

        Task<ServiceResponse<bool>> RemoveItemFromCart(CartItem cartItem);

        Task<ServiceResponse<bool>> UpdateQuantity(CartItem cartItem);        


    }
}
