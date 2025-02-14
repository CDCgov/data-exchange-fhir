﻿using Microsoft.AspNetCore.Mvc;

namespace OneCDPFHIRFacade.Controllers
{
    // #####################################################
    // Define a health check endpoint at /health
    // #####################################################

    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet(Name = "Health")]
        public IResult Get()
        {
            return Results.Json(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow.ToString(""), // ISO 8601 format for compatibility
                description = "API is running and healthy"
            });
        }
    }
}