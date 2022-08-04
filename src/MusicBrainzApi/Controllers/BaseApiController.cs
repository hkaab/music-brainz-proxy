using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace MusicBrainzApi.Controllers
{
    public class BaseApiController : ControllerBase
    {
        private readonly ILogger<BaseApiController> _logger;

        public BaseApiController(ILogger<BaseApiController> logger)
        {
            _logger = logger;
        }
        protected IActionResult HandleException(Exception ex)
        {
            _logger.LogError(ex,ex.Message);
            return ex switch
            {
                ArgumentException => StatusCode(StatusCodes.Status400BadRequest, ex.Message),
                _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
            };
        }

    }
}
