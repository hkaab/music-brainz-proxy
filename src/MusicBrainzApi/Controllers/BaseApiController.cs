using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;

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
            IActionResult ret;
            if (ex is ArgumentException)
                ret = StatusCode(StatusCodes.Status400BadRequest,ex.Message);
            else 
                ret = StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            return ret;
        }
        
    }
}
