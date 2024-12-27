using JarvisWeb.Domain;
using JarvisWeb.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Services
{
    public class ApiKeyService(
        ILogger<ApiKeyService> logger, 
        JarvisWebDbContext context): BaseService<ApiKey>(context, logger, context.ApiKeys)
    {
        
    }
}
