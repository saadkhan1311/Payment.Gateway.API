using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.API.Middlewares;
using PaymentGateway.Business;
using PaymentGateway.Domain;
using PaymentGateway.Domain.DTOs;

namespace PaymentGateway.API.Controllers
{
    /// <summary>
    /// Process and Retrieve Card Payments
    /// </summary>
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IRequestInfo _request_info;
        private readonly IPaymentsService _paymentsService;
        private readonly IValidator<PaymentRequestDTO> _validator;
        private IMapper _mapper;

        public PaymentsController(IRequestInfo request_info, IPaymentsService paymentsService, IValidator<PaymentRequestDTO> validator, IMapper mapper)
        {
            _request_info = request_info;
            _paymentsService = paymentsService;
            _validator = validator;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the payment details by the acquirer_reference_id
        /// </summary>
        /// <param name="acquirer_reference_id"></param>
        /// <returns>Returns the payment response</returns>
        /// <response code="200">Returns the payment response</response>
        /// <response code="404">No payment response found for the provided acquirer_reference_id</response> 
        /// <response code="400">Either the acquirer_reference_id was null or empty guid</response> 
        [HttpGet("{acquirer_reference_id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PaymentResponseDTO>> GetPaymentByUid(Guid acquirer_reference_id)
        {
            if (acquirer_reference_id == Guid.Empty) return BadRequest("acquirer_reference_id was null");
            var result = await _paymentsService.GetPaymentByAcquirerRefrenceIdAsync(acquirer_reference_id);
            if (result == null)
                return NotFound("No transaction found for the specified acquirer_reference_id");
            return Ok(result);
        }


        /// <summary>
        /// Validates the Payment Request, Stores(Encrypts) /Retrieves(Decrypts) Card Info and forwards to the Acquiring Bank for completing the transaction
        /// </summary>
        /// <param name="paymentRequest"></param>
        /// <returns>Payment Response either. Check Status and Errors fields in response</returns>
        /// <response code="201">Returns the payment response</response>
        /// <response code="400">Either the payment request was null or an unexpected error occurred in the gateway</response> 
        /// <response code="401">Access was denied to the api</response> 
        /// <response code="422">The payment request was invalid</response> 
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        public async Task<ActionResult<PaymentResponseDTO>> ProcessPayment([FromBody] PaymentRequestDTO paymentRequest)
        {
            try
            {
                if (paymentRequest == null) return BadRequest("Request was null");

                //Check if the request was made by the merchant who's secret_key was used
                if (paymentRequest.merchant_id != _request_info.User_Id)
                {
                    var invalidPaymentResponse = _mapper.Map<PaymentResponseDTO>(paymentRequest);
                    invalidPaymentResponse.Transaction_Reference_Id = _request_info.Transaction_Reference;
                    invalidPaymentResponse.Errors = new List<string>() { StringConstants.MERCHANT_SECRET_KEY_MISMATCH_MESSAGE };
                    invalidPaymentResponse.Status = Domain.DTOs.StatusCode.INVALID_DATA;

                    return Unauthorized(invalidPaymentResponse);
                }

                //Run the Fluent Validation Rules on the Model
                var validationResult = _validator.Validate(paymentRequest);

                if (validationResult.IsValid)
                {
                    var result = await _paymentsService.ProcessPaymentAsync(_request_info.Transaction_Reference, paymentRequest);

                    //If Payment Response had errors it measns some business validations must have failed, return InvalidData Response
                    if (result.Errors != null && result.Errors.Count() > 0)
                    {
                        return UnprocessableEntity(result);
                    }
                    else
                    {
                        //Payment was processed successfully
                        if (result.Status == Domain.DTOs.StatusCode.APPROVED)
                            return CreatedAtAction(nameof(GetPaymentByUid),new { acquirer_reference_id = result.Acquirer_Reference_Id} ,result);
                        //Payment was not processed successfully
                        else
                            return UnprocessableEntity(result);
                    }
                }
                else
                {
                    var invalidPaymentResponse = _mapper.Map<PaymentResponseDTO>(paymentRequest);
                    invalidPaymentResponse.Transaction_Reference_Id = _request_info.Transaction_Reference;
                    invalidPaymentResponse.Errors = validationResult.Errors.Select(x => x.ErrorMessage);
                    invalidPaymentResponse.Status = Domain.DTOs.StatusCode.INVALID_DATA;

                    return UnprocessableEntity(invalidPaymentResponse);
                }
            }
            catch (Exception ex)
            {
                //Log the actual exception
                var errorPaymentResponse = _mapper.Map<PaymentResponseDTO>(paymentRequest);
                errorPaymentResponse.Transaction_Reference_Id = _request_info.Transaction_Reference;
                errorPaymentResponse.Errors = new List<string>() { StringConstants.UNEXPECTED_ERROR_MESSAGE };
                errorPaymentResponse.Status = Domain.DTOs.StatusCode.UNKNOWN;
                return BadRequest(errorPaymentResponse);
            }
        }
    }
}
