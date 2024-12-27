using JarvisWeb.Domain;
using JarvisWeb.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Utilities
{
    public static class UserUtilities
    {
        public static async Task<User?> GetUserFromBearerToken(string bearerToken, JarvisWebDbContext dbContext)
        {
            var jwtTokenParser = new JwtSecurityTokenHandler();
            var token = jwtTokenParser.ReadJwtToken(bearerToken);
            var identityId = token.Claims.First(c => c.Type == "sub").Value;
            if (identityId == null)
            {
                return null;
            }
            var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.ExternalId == identityId);
            if (user == null)
            {
                user = new User
                {
                    ExternalId = identityId,
                    Name = token.Claims.First(c => c.Type == "name").Value,
                    Nicknames = token.Claims.First(c => c.Type == "name").Value,
                };
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }
            return user;
        }
    }
}
