using JarvisWeb.Domain;
using JarvisWeb.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Utilities
{
    public static class UserUtilities
    {
        public static async Task<Guid> GetUserGuidFromIdentityId(string identityId, JarvisWebDbContext context)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.ExternalId == identityId);
            if (user == null)
            {
                context.Users.Add(new User { ExternalId = identityId });
                context.SaveChanges();
                user = await context.Users.FirstOrDefaultAsync(u => u.ExternalId == identityId);
            }
            return user!.Id;
        }
    }
}
