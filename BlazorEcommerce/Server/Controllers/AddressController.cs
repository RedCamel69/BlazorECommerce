using BlazorEcommerce.Server.Services.AdressService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<Address>>> AddorUpdateAddress(Address address)
        {
            var result = await _addressService.AddOrUpdateAddress(address);
            //return result;
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<Address>>> GetAddress()
        {
            var result = await _addressService.GetAddress();
            return Ok(result);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
