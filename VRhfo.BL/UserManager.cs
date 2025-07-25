﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRhfo.BL.Models;
using VRhfo.PL;
using static VRhfo.BL.Models.User;

namespace VRhfo.BL
{
    public static class UserManager
    {         
        public class LoginFailureException : Exception
        {
            public LoginFailureException() : base("Invalid Username or Password.")
            {
            }

            public LoginFailureException(string message) : base(message)
            {
            }
        }

        public static async Task<int> UpdatePassword(User user, string newPassword)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    int result;
                    var tblUser = await dc.tblUsers.FirstOrDefaultAsync(u => u.Id == user.Id);
                    if (tblUser == null)
                        throw new Exception("User not found.");

                    // Hash the new password
                    string hashedPassword = newPassword;

                    // Update the password
                    tblUser.PasswordHash = hashedPassword;

                    // Clear the reset token and expiration if they exist
                    tblUser.PasswordResetToken = null;
                    tblUser.PasswordResetTokenExpiration = null;

                    result = await dc.SaveChangesAsync();
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            try
            {
                // Generate a secure random token
                byte[] tokenBytes = new byte[32];
                using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                {
                    rng.GetBytes(tokenBytes);
                }
                string token = Convert.ToBase64String(tokenBytes);

                // Set expiration (1 hour from now)
                DateTime expiration = DateTime.UtcNow.AddHours(1);

                // Update the user in the database with the token and expiration
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    var tblUser = await dc.tblUsers.FirstOrDefaultAsync(u => u.Id == user.Id);
                    if (tblUser == null)
                        throw new Exception("User not found.");

                    tblUser.PasswordResetToken = token;
                    tblUser.PasswordResetTokenExpiration = expiration;
                    await dc.SaveChangesAsync();
                }

                return token;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<User> FindByEmailAsync(ResetPassword rpModel)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    tblUser row = await dc.tblUsers.FirstOrDefaultAsync(u => u.Email == rpModel.Email);
                    if (row == null)
                        return null;

                    return new User
                    {
                        Username = row.Username,
                        Id = row.Id,
                        Password = row.PasswordHash,
                        Email = row.Email,
                        FirstVisit = row.FirstVisit,
                        SubscriptionStartDate = (DateTime)row.SubscribedDate,
                        Auth0UserId = row.Auth0UserId,
                        Subscription_Tier = row.SubscriptionTier,
                        NextRenewalDueDate = (DateTime)row.NextRenewalDueDate,
                        GoonScore = (int)row.GoonScore
                    };
                }
            }
            catch (Exception)
            {
                throw;

            }
       }

        public static async Task<User> LoadByIdAsync(Guid id)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    tblUser tblUser = await dc.tblUsers.FirstOrDefaultAsync(x => x.Id == id);
                    if (tblUser == null)
                        return null;

                    User user = new User
                    {
                        Id = id,
                        SubscriptionStartDate = (DateTime)tblUser.SubscribedDate,
                        Username = tblUser.Username,
                        Email = tblUser.Email,
                        Auth0UserId = tblUser.Auth0UserId,
                        IsSubscribed = tblUser.IsSubscribed,
                        FirstVisit = tblUser.FirstVisit,
                        Password = tblUser.PasswordHash,
                        Subscription_Tier = tblUser.SubscriptionTier,
                        GoonScore = (int)tblUser.GoonScore,
                        PasswordResetToken = tblUser.PasswordResetToken,
                        PasswordResetTokenExpiration = tblUser.PasswordResetTokenExpiration
                    };

                    return user;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async static Task<User> GetFreeUserByGuid(string guid)
        {
            using (VRhfoEntities dc = new VRhfoEntities())
            {
                Guid parsedGuid;
                if (!Guid.TryParse(guid, out parsedGuid))
                    return null; // invalid GUID passed

                var tbUser = await dc.tblUsers
                    .FirstOrDefaultAsync(u => u.Id == parsedGuid && u.IsSubscribed == false);

                if (tbUser == null)
                    return null;

                // Convert tblUser to User (assuming you have a mapping like this)
                User user = new User
                {
                     Id = tbUser.Id,
                     FirstVisit = tbUser.FirstVisit,
                     IPAdress = tbUser.IPaddress
                    // Add more fields if needed
                };

                return user;
            }
        }

        //TODO: This needs to be changed to "UpgradeToSubscriber", insert free exists,
        //only option is to upgrade (update the free)
        public static async Task<int> UpgradeToPremium(User user, bool rollback = false)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    // Grab the existing user row by ID
                    var existingUser = await dc.tblUsers.FirstOrDefaultAsync(x => x.Id == user.Id);


                    // if user email exists, send back error
                    var existingEmail = await dc.tblUsers.FirstOrDefaultAsync(x => x.Email == user.Email);
                    if (existingEmail != null)
                    {
                        return 0;

                    }
                    if (existingUser != null)
                    {
                        // Update existing user's fields
                        existingUser.Email = user.Email;
                        existingUser.Auth0UserId = user.Auth0UserId;
                        existingUser.IsSubscribed = user.IsSubscribed;
                        existingUser.FirstVisit = user.FirstVisit;
                        existingUser.Username = user.Username;
                        existingUser.SubscribedDate = user.SubscriptionStartDate;
                        existingUser.PasswordHash = user.Password;
                        existingUser.SubscriptionTier = user.Subscription_Tier;
                        existingUser.NextRenewalDueDate = user.NextRenewalDueDate;
                        existingUser.GoonScore = user.GoonScore;
                    }
                    else
                    {
                        // Optional: handle the case where user does not exist (should not happen if upgrading)
                        // Throw exception or return error code
                        throw new Exception("User not found for upgrade.");
                    }

                    return await dc.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                throw; // Rethrow to caller, or return error code if preferred
            }
        }

        public static async Task<bool> LoginAsync(User user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.Password))
                {
                    using (VRhfoEntities db = new VRhfoEntities())
                    {
                        tblUser row = await db.tblUsers.FirstOrDefaultAsync(u => u.Email == user.Email);
                        if (row == null)
                            return false;

                        // Check password
                        if (row.PasswordHash == GetHash(user.Password) || row.PasswordHash == user.Password)
                        {
                            user.SubscriptionStartDate = (DateTime)row.SubscribedDate;
                            user.Subscription_Tier = row.SubscriptionTier;
                            user.NextRenewalDueDate = (DateTime)row.NextRenewalDueDate;
                            user.FirstVisit = row.FirstVisit;
                            user.Username = row.Username;
                            user.IsSubscribed = row.IsSubscribed;
                            user.Email = row.Email;
                            user.Password = row.PasswordHash == user.Password ? user.Password : GetHash(user.Password);
                            user.GoonScore = (int)row.GoonScore;
                           
                            return true;
                        }
                        return false;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string GetHash(string password)
        {
            using (var hasher = new System.Security.Cryptography.SHA1Managed())
            {
                var hashbytes = System.Text.Encoding.UTF8.GetBytes(password);
                return Convert.ToBase64String(hasher.ComputeHash(hashbytes));
            }
        }

        public static async Task<User> LoadByUsernameAsync(string username)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    tblUser row = await dc.tblUsers.FirstOrDefaultAsync(u => u.Username == username);
                    if (row == null)
                        return null;

                    return new User
                    {
                        Username = row.Username,
                        Id = row.Id,
                        Password = row.PasswordHash,
                        Email = row.Email,
                        FirstVisit = row.FirstVisit,
                        SubscriptionStartDate = (DateTime)row.SubscribedDate,
                        Auth0UserId = row.Auth0UserId,
                        Subscription_Tier = row.SubscriptionTier,
                        NextRenewalDueDate = (DateTime)row.NextRenewalDueDate,
                        GoonScore = (int)row.GoonScore
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



        public static async Task<int> LoadTodaysWatchTimeAsync(Guid userId)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    var today = DateTime.UtcNow.Date;
                    var todaysWatchTime = await dc.tblWatchEntries
                        .Where(w => w.UserId == userId && w.LastDateWatched >= today)
                        .SumAsync(w => (int?)w.WatchDurationTicks) ?? 0;

                    return todaysWatchTime;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<int> LoadWeeklyWatchTimeAsync(Guid userId)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    var lastWeek = DateTime.UtcNow.Date.AddDays(-7);
                    var weeklyWatchTime = await dc.tblWatchEntries
                        .Where(w => w.UserId == userId && w.LastDateWatched >= lastWeek)
                        .SumAsync(w => (int?)w.WatchDurationTicks) ?? 0;

                    return weeklyWatchTime;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<int> LoadLifetimeWatchTimeAsync(Guid userId)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    var lifetimeWatchTime = await dc.tblWatchEntries
                        .Where(w => w.UserId == userId)
                        .SumAsync(w => (int?)w.WatchDurationTicks) ?? 0;

                    return lifetimeWatchTime;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<int> LoadMonthlyWatchTimeAsync(Guid userId)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    var lastMonth = DateTime.UtcNow.Date.AddDays(-30);
                    var monthlyWatchTime = await dc.tblWatchEntries
                        .Where(w => w.UserId == userId && w.LastDateWatched >= lastMonth)
                        .SumAsync(w => (int?)w.WatchDurationTicks) ?? 0;

                    return monthlyWatchTime;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<int> InsertFreeAccountGUIDAsync(User guid)
        {
            try
            {
                
                // Check if it already exists (very unlikely with GUID)
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                        tblUser freeUser = new tblUser
                        {
                            Id = guid.Id,
                            Auth0UserId = "free-user",
                            Username = "Free User",
                            Email = $"{Guid.NewGuid()}",
                            PasswordHash = "FreeUserPassword123",
                            FirstVisit = DateTime.UtcNow,
                            SubscribedDate = DateTime.UtcNow,
                            IsSubscribed = false,
                            NextRenewalDueDate = DateTime.UtcNow.AddYears(1),
                            SubscriptionTier = "Free",
                            GoonScore = 0,
                            PasswordResetToken = null,
                            IPaddress = guid.IPAdress 
                            
                        };
                        dc.tblUsers.Add(freeUser);
                                       
                    return await dc.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<User> LoadWatchedVideosAsync(string username)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    var user = await dc.tblUsers.FirstOrDefaultAsync(u => u.Username == username);
                    if (user == null)
                        return null;

                    var today = DateTime.Today;
                    var weekAgo = today.AddDays(-7);

                    var watchEntries = await dc.tblWatchEntries
                        .Where(we => we.UserId == user.Id)
                        .Select(we => new
                        {
                            we.VideoId,
                            we.LastDateWatched
                        })
                        .ToListAsync();

                    var videosWatchedToday = watchEntries
                        .Where(we => we.LastDateWatched >= today)
                        .Select(we => we.VideoId)
                        .Distinct()
                        .ToList();

                    var videosWatchedPastWeek = watchEntries
                        .Where(we => we.LastDateWatched >= weekAgo && we.LastDateWatched < today)
                        .Select(we => we.VideoId)
                        .Distinct()
                        .ToList();

                    var restOfVideosWatched = watchEntries
                        .Where(we => we.LastDateWatched < weekAgo)
                        .Select(we => we.VideoId)
                        .Distinct()
                        .ToList();

                    var videosToday = await dc.tblVideos
                        .Where(v => videosWatchedToday.Contains(v.Id))
                        .Select(v => new Video
                        {
                            Id = v.Id,
                            Title = v.Title,
                            ThumbnailUrl = v.ThumbnailUrl,
                            Duration = v.Duration,
                            VideoUrl = v.VideoUrl
                        })
                        .ToListAsync();

                    var videosPastWeek = await dc.tblVideos
                        .Where(v => videosWatchedPastWeek.Contains(v.Id))
                        .Select(v => new Video
                        {
                            Id = v.Id,
                            Title = v.Title,
                            ThumbnailUrl = v.ThumbnailUrl,
                            Duration = v.Duration,
                            VideoUrl = v.VideoUrl
                        })
                        .ToListAsync();

                    var videosRest = await dc.tblVideos
                        .Where(v => restOfVideosWatched.Contains(v.Id))
                        .Select(v => new Video
                        {
                            Id = v.Id,
                            Title = v.Title,
                            ThumbnailUrl = v.ThumbnailUrl,
                            Duration = v.Duration,
                            VideoUrl = v.VideoUrl
                        })
                        .ToListAsync();

                    return new User
                    {
                        VideosWatchedToday = videosToday,
                        VideosWatchedPastWeek = videosPastWeek,
                        RestOfVideosWatched = videosRest
                    };
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading watched videos.", ex);
            }
        }

        public static bool SaveSessionData(Guid userId, string ip, SessionDto data)
        {
            throw new NotImplementedException();
        }

        public static User GetFreeUserByIP(string ip)
        {

            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    // Query the database for the user with the specified IP address
                    tblUser freeUser = dc.tblUsers.FirstOrDefault(u => u.IPaddress == ip);

                    if (freeUser != null)
                    {
                        // Map tblUser to User entity (assuming User is your domain model)
                        User user = new User
                        {
                            Id = freeUser.Id,
                               // Map other properties as needed
                            FirstVisit = freeUser.FirstVisit

                        };

                        return user;
                    }

                    return null; // Return null if no user found with the given IP
                }
            }
            catch (Exception)
            {
                throw; // Handle exceptions according to your application's error handling strategy
            }
        }

        public static bool VerifyPassword(object hashedPassword, string password)
        {
            throw new NotImplementedException();
        }
    }
}