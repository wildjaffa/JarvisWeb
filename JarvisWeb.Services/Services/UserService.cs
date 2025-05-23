﻿using JarvisWeb.Domain;
using JarvisWeb.Domain.Models;
using JarvisWeb.Services.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Services
{
    public class UserService(
        ILogger<UserService> logger,
        JarvisWebDbContext context,
        GlobalStateService globalStateService)
    {
        ILogger<UserService> _logger = logger;
        JarvisWebDbContext _context = context;
        GlobalStateService _globalStateService = globalStateService;

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            _logger.LogInformation($"Getting user with id {id}");
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByApiKey(string apiKey)
        {
            return await _context.Users.
                FirstOrDefaultAsync(u => u.ApiKeys.Any(a => a.IsActive && a.Key == apiKey));
        }

        public async Task<User?> GetUserFromJwtToken(string jwtToken)
        {
            _logger.LogInformation($"Getting user from JWT token");
            var user = await UserUtilities.GetUserFromBearerToken(jwtToken, _context);
            return user;
        }

        public async Task<User> Update(User user)
        {
            var dbUser = await _context.Users.FindAsync(user.Id);
            if (dbUser == null)
            {
                return user;
            }
            dbUser.LastSeenDailySummaryId = user.LastSeenDailySummaryId;
            dbUser.IsInOffice = user.IsInOffice;
            dbUser.ExternalId = user.ExternalId;
            dbUser.Name = user.Name;
            dbUser.Nicknames = user.Nicknames;
            _context.Users.Update(dbUser);
            await _context.SaveChangesAsync();
            return dbUser;
        }

        public async Task UpdateUserIsInOffice(Guid userId, bool isInOffice)
        {
            var dbUser = await _context.Users.FindAsync(userId);
            if (dbUser == null)
            {
                return;
            }
            dbUser.IsInOffice = isInOffice;
            _context.Update(dbUser);
            await _context.SaveChangesAsync();
            _globalStateService.StateHasChanged(userId);
            if (!isInOffice) return;
            await Task.Delay(10 * 1000);
            await BashUtilities.RunCommandWithBash("/mnt/sda5/ai-assistant/scripts/map-touchscreen.sh", _logger);
        }
    }
}
