using Fulfillment.Core.Entities;
using Fulfillment.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fulfillment.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DevSeedController : ControllerBase
{
    private readonly FulfillmentDbContext _context;

    public DevSeedController(FulfillmentDbContext context)
    {
        _context = context;
    }
}