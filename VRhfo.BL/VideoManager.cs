﻿using System.Collections.Generic;
using VRhfo.BL.Models;
using VRhfo.PL;

namespace VRhfo.BL
{
    public static class VideoManager
    {
        public static bool HasUserViewedRecently(Guid userId, int videoId, TimeSpan interval)
        {
            using (VRhfoEntities dc = new VRhfoEntities())
            {
                var since = DateTime.Now - interval;
                return dc.tblVideoViews.Any(v =>
                    v.UserId == userId &&
                    v.VideoId == videoId &&
                    v.ViewTime > since);
            }
        }
        public static bool HasIpViewedRecently(string ip, int videoId, TimeSpan interval)
        {
            using (VRhfoEntities dc = new VRhfoEntities())
            {
                var since = DateTime.Now - interval;
                return dc.tblVideoViews.Any(v =>
                    v.IPAdress == ip &&
                    v.VideoId == videoId &&
                    v.ViewTime > since);
            }
        }
        public static void LogUserView(Guid userId, int videoId)
        {
            using (VRhfoEntities dc = new VRhfoEntities())
            {
                var entry = new tblVideoView
                {
                    VideoId = videoId,
                    UserId = userId,
                    ViewTime = DateTime.Now
                };

                dc.tblVideoViews.Add(entry);
                dc.SaveChanges();
            }
        }
        public static void LogIpView(string ip, int videoId)
        {
            using (VRhfoEntities dc = new VRhfoEntities())
            {
                var entry = new tblVideoView
                {
                    VideoId = videoId,
                    IPAdress = ip,
                    ViewTime = DateTime.Now
                };

                dc.tblVideoViews.Add(entry);
                dc.SaveChanges();
            }
        }

        public static void IncrementView(int videoId)
        {
            using (VRhfoEntities dc = new VRhfoEntities())
            {
                var video = dc.tblVideos.FirstOrDefault(v => v.Id == videoId);
                if (video != null)
                {
                    video.Views += 1;
                    dc.SaveChanges();
                }
            }
        }

        public static List<Video> GetUsersLikedVideos(Guid userId)
        {
            List<Video> likedVids = new List<Video>();
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    var likedVideoIds = dc.tblVideosLikes
                                          .Where(vl => vl.UserID == userId)
                                          .Select(vl => vl.VideoID)
                                          .ToList();

                    likedVids = dc.tblVideos
                                  .Where(v => likedVideoIds.Contains(v.Id))
                                  .Select(v => new Video
                                  {
                                      Id = v.Id,
                                      UserId = userId,
                                      VideoUrl = v.VideoUrl,
                                      Category = v.Category,
                                      Likes = v.Likes,
                                      UploadDate = v.UploadDate,
                                      Description = v.Description,
                                      Dislikes = v.Dislikes,
                                      Views = v.Views,
                                      Title = v.Title,
                                      Genre = v.Genre,
                                      Duration = v.Duration,
                                      RatingCount = v.RatingCount,
                                      ThumbnailUrl = v.ThumbnailUrl,
                                  })
                                  .ToList();
                };
            }
            catch (Exception)
            {

                throw;
            }

            return likedVids;
        }
        public static List<Video> GetSuggestedVideos(int maxSuggestions, string watchedVideoTitle)
        {
            using (VRhfoEntities dc = new VRhfoEntities())
            {
                var random = new Random();
                var unwatchedVideos = dc.tblVideos.Where(v => v.Title != watchedVideoTitle).ToList();
                var shuffledVideos = unwatchedVideos.OrderBy(v => random.Next()).ToList();

                if (shuffledVideos.Count <= maxSuggestions)
                {
                    return shuffledVideos.Select(v => new Video
                    {
                        Id = v.Id,
                        UserId = v.UserId,
                        Title = v.Title,
                        Category = v.Category,
                        ThumbnailUrl = v.ThumbnailUrl,
                        VideoUrl = v.VideoUrl,
                        Description = v.Description,
                        Genre = v.Genre,
                        UploadDate = v.UploadDate,
                        Duration = v.Duration,
                        Views = v.Views,
                        RatingCount = v.RatingCount,
                        IsPublic = v.IsPublic == 1,
                        IsPreview = v.IsPreview == 1,
                        ContentWarning = v.ContentWarning,
                        Likes = v.Likes,
                        Dislikes = v.Dislikes
                    }).ToList();
                }
                else
                {
                    return shuffledVideos.Take(maxSuggestions).Select(v => new Video
                    {
                        Id = v.Id,
                        UserId = v.UserId,
                        Title = v.Title,
                        Category = v.Category,
                        ThumbnailUrl = v.ThumbnailUrl,
                        VideoUrl = v.VideoUrl,
                        Description = v.Description,
                        Genre = v.Genre,
                        UploadDate = v.UploadDate,
                        Duration = v.Duration,
                        Views = v.Views,
                        RatingCount = v.RatingCount,
                        IsPublic = v.IsPublic == 1,
                        IsPreview = v.IsPreview == 1,
                        ContentWarning = v.ContentWarning,
                        Likes = v.Likes,
                        Dislikes = v.Dislikes
                    }).ToList();
                }
            }
        }

        public static Video LoadByTitle(string title)
        {
            using (VRhfoEntities dc = new VRhfoEntities())
            {
                var videoEntity = dc.tblVideos.FirstOrDefault(v => v.Title == title);

                if (videoEntity != null)
                {
                    return new Video
                    {
                        Id = videoEntity.Id,
                        UserId = videoEntity.UserId,
                        Title = videoEntity.Title,
                        Category = videoEntity.Category,
                        ThumbnailUrl = videoEntity.ThumbnailUrl,
                        VideoUrl = videoEntity.VideoUrl,
                        Description = videoEntity.Description,
                        Genre = videoEntity.Genre,
                        UploadDate = videoEntity.UploadDate,
                        Duration = videoEntity.Duration,
                        Views = videoEntity.Views,
                        RatingCount = videoEntity.RatingCount,
                        IsPublic = videoEntity.IsPublic == 1,
                        IsPreview = videoEntity.IsPreview == 1,
                        ContentWarning = videoEntity.ContentWarning,
                        Likes = videoEntity.Likes,
                        Dislikes = videoEntity.Dislikes
                    };
                }
                else
                {
                    return null;
                }
            }
        }
        public static List<Video> LoadAll()
        {
            List<Video> list = new List<Video>();

            using (VRhfoEntities dc = new VRhfoEntities())
            {
                (from v in dc.tblVideos
                 select new
                 {
                     v.Id,
                     v.UserId,
                     v.Title,
                     v.Category,
                     v.ThumbnailUrl,
                     v.VideoUrl,
                     v.Description,
                     v.Genre,
                     v.UploadDate,
                     v.Duration,
                     v.Views,
                     v.RatingCount,
                     v.IsPublic,
                     v.IsPreview,
                     v.ContentWarning,
                     v.Likes,
                     v.Dislikes,
                     v.PreviewVideoURL
                 }).ToList().ForEach(video => list.Add(new Video
                 {
                     Id = video.Id,
                     UserId = video.UserId,
                     Title = video.Title,
                     Category = video.Category,
                     ThumbnailUrl = video.ThumbnailUrl,
                     VideoUrl = video.VideoUrl,
                     Description = video.Description,
                     Genre = video.Genre,
                     UploadDate = video.UploadDate,
                     Duration = video.Duration,
                     Views = video.Views,
                     RatingCount = video.RatingCount,
                     IsPublic = video.IsPublic == 1,
                     IsPreview = video.IsPreview == 1,
                     ContentWarning = video.ContentWarning,
                     Likes = video.Likes,
                     Dislikes = video.Dislikes,
                     PreviewVideoURL = video.PreviewVideoURL
                 }));

                return list;
            }
        }


        public static Video LoadById(int id)
        {
            using (VRhfoEntities dc = new VRhfoEntities())
            {
                var videoEntity = dc.tblVideos.FirstOrDefault(v => v.Id == id);

                if (videoEntity != null)
                {
                    return new Video
                    {
                        Id = videoEntity.Id,
                        UserId = videoEntity.UserId,
                        Title = videoEntity.Title,
                        Category = videoEntity.Category,
                        ThumbnailUrl = videoEntity.ThumbnailUrl,
                        VideoUrl = videoEntity.VideoUrl,
                        Description = videoEntity.Description,
                        Genre = videoEntity.Genre,
                        UploadDate = videoEntity.UploadDate,
                        Duration = videoEntity.Duration,
                        Views = videoEntity.Views,
                        RatingCount = videoEntity.RatingCount,
                        IsPublic = videoEntity.IsPublic == 1,
                        IsPreview = videoEntity.IsPreview == 1,
                        ContentWarning = videoEntity.ContentWarning,
                        Likes = videoEntity.Likes,
                        Dislikes = videoEntity.Dislikes
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public static int Insert(Video video, bool rollback = false)
        {
            try
            {
                int results = 0;
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    tblVideo row = new tblVideo();
                    row.Id = video.Id;
                    row.UserId = video.UserId;
                    row.Title = video.Title;
                    row.Category = video.Category;
                    row.ThumbnailUrl = video.ThumbnailUrl;
                    row.VideoUrl = video.VideoUrl;
                    row.ContentWarning = video.ContentWarning;

                    //Todo
                    row.Likes = video.Likes;
                }
                return results;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static int Delete(Video video, bool rollback = false)
        {
            try
            {
                int results = 0;
                return results;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<Video> LoadByUserId(Guid userId)
        {
            List<Video> list = new List<Video>();

            using (VRhfoEntities dc = new VRhfoEntities())
            {
                var videosForUser = (from v in dc.tblVideos
                                     where v.UserId == userId
                                     select new
                                     {
                                         v.Id,
                                         v.UserId,
                                         v.Title,
                                         v.Category,
                                         v.ThumbnailUrl,
                                         v.VideoUrl,
                                         v.Description,
                                         v.Genre,
                                         v.UploadDate,
                                         v.Duration,
                                         v.Views,
                                         v.RatingCount,
                                         v.IsPublic,
                                         v.IsPreview,
                                         v.ContentWarning,
                                         v.Likes,
                                         v.Dislikes
                                     }).ToList();

                foreach (var video in videosForUser)
                {
                    list.Add(new Video
                    {
                        Id = video.Id,
                        UserId = video.UserId,
                        Title = video.Title,
                        Category = video.Category,
                        ThumbnailUrl = video.ThumbnailUrl,
                        VideoUrl = video.VideoUrl,
                        Description = video.Description,
                        Genre = video.Genre,
                        UploadDate = video.UploadDate,
                        Duration = video.Duration,
                        Views = video.Views,
                        RatingCount = video.RatingCount,
                        IsPublic = video.IsPublic == 1,
                        IsPreview = video.IsPreview == 1,
                        ContentWarning = video.ContentWarning,
                        Likes = video.Likes,
                        Dislikes = video.Dislikes
                    });
                }
            }

            return list;
        }

        public static string checkIfCurrentVideoLiked(Video video, User currentUser)
        {
            using(VRhfoEntities dc = new VRhfoEntities())
            {
                 var isLiked = dc.tblVideosLikes
                              .Any(vl => vl.VideoID == video.Id && vl.UserID == currentUser.Id);
                return isLiked ? "like-yes" : "like-noL";
            }
        }

        public static int InsertWatchEntry(WatchEntry userWatchedVid)
        {

            int result = 0;
            if (userWatchedVid == null) { return 0; }

            using(VRhfoEntities dc = new VRhfoEntities())
            {

                if(dc.tblWatchEntries.Any(s => s.Id == userWatchedVid.Id && s.VideoId == userWatchedVid.VideoId))
                {
                    return 0;
                }
                tblWatchEntry tblRow = new tblWatchEntry
                {
                    UserId = userWatchedVid.UserId,
                    VideoId = userWatchedVid.VideoId,
                    WatchDurationTicks = userWatchedVid.WatchDurationTicks,
                    Completed = userWatchedVid.Completed,
                    LastDateWatched = userWatchedVid.LastDateWatched,
                    FirstViewed = userWatchedVid.FirstViewed,
                    TimesViewed = userWatchedVid.TimesViewed
                };
                dc.tblWatchEntries.Add(tblRow);
                result = dc.SaveChanges();
            }

            return result;
        }

        public static bool UpdateWatchEntry(WatchEntry userWatchedVid)
        {
            using(VRhfoEntities db = new VRhfoEntities())
            {
                var tblRow = db.tblWatchEntries.FirstOrDefault(row => row.UserId == userWatchedVid.UserId && row.VideoId == userWatchedVid.VideoId);

                if (tblRow != null)
                {
                                            //tracking total lifetime watch duration, not per session and resetting to 0
                    tblRow.WatchDurationTicks +=  userWatchedVid.WatchDurationTicks;
                    tblRow.LastDateWatched = userWatchedVid.LastDateWatched;
                    tblRow.Completed = userWatchedVid.Completed;
                    

                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public static bool CheckIfWatchEntry(WatchEntry userWatchedVid)
        {
            using(VRhfoEntities dc = new VRhfoEntities())
            {
                return dc.tblWatchEntries.Any(row => row.UserId == userWatchedVid.UserId && row.VideoId == userWatchedVid.VideoId);
            }
        }
    }
}
