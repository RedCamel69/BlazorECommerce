using BlazorEcommerce.Server.Services.AuthService;
using Microsoft.EntityFrameworkCore;
using System.Timers;

namespace BlazorEcommerce.Server.Services.AdressService
{
    public class AddressService : IAddressService
    {
        private readonly DataContext _context;
        private readonly IAuthService _authService;

        public AddressService(DataContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address)
        {
            var response = new ServiceResponse<Address>();

            var dbAddress = (await GetAddress()).Data;

            if (dbAddress == null)
            {
                //create
                address.UserId = _authService.GetUserId();
                _context.Addresses.Add(address);
                response.Data = address;
            }
            else
            {
                //update 
                dbAddress.FirstName = address.FirstName;
                dbAddress.LastName = address.LastName;
                dbAddress.State = address.State;
                dbAddress.Street = address.Street;
                dbAddress.City= address.City;
                dbAddress.Country = address.Country;
                dbAddress.Zip= address.Zip; ;
                response.Data = dbAddress;  
            }

            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<ServiceResponse<Address>> GetAddress()
        {
            var userId = _authService.GetUserId();
            var address = await _context.Addresses.Where(x=>x.UserId == userId).FirstOrDefaultAsync();

            return new ServiceResponse<Address>
            {
                Data = address
            };
        }
    }
}
