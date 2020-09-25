using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.API.Middlewares;
using PaymentGateway.Domain.DomainObjects;
using PaymentGateway.Domain.DTOs;
using PaymentGateway.Repository;

namespace PaymentGateway.API.Controllers
{
    /// <summary>
    /// Just for testing purpose (Authorization key needed for this too, use developer secret_key "key_uCLUxBlfJBkN9fmmrvWPVlG3s1OcRvpCasTBMQ9vjhE=" )
    /// </summary>
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class MasterdataController : ControllerBase
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IRequestInfo _request_info;
        public MasterdataController(IMerchantRepository merchantRepository, ICurrencyRepository currencyRepository,IRequestInfo request_info)
        {
            _merchantRepository = merchantRepository;
            _currencyRepository = currencyRepository;
            _request_info = request_info;
        }

        /// <summary>
        /// Gets all the supported currencies
        /// </summary>
        /// <returns></returns>
        [HttpGet("currencies")]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<Currency>> GetAllCurrencies()
        {
            return Ok(_currencyRepository.QueryAll());
        }

        /// <summary>
        /// Gets all the resgistered Merchants and gets their decrypted secret_keys
        /// </summary>
        /// <returns></returns>
        [HttpGet("merchants")]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<Merchant>> GetAllMerchants()
        {
            return Ok(_merchantRepository.GetAll());
        }

        /// <summary>
        /// Adds a new merchant and returns the merchant Id and secret_key to be used in further requests. secret_key is encyrpted  in database
        /// </summary>
        /// <param name="merchant_name"></param>
        /// <returns></returns>
        [HttpPost("merchant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Merchant>> AddMerchant([FromQuery] string merchant_name)
        {
            Merchant merchant = new Merchant();
            merchant.Name = merchant_name;
            var resp = await _merchantRepository.AddAsync(merchant);
            return Ok(resp);
        }
    }
}
