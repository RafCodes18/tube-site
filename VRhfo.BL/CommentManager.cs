using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System.ComponentModel.Design;
using VRhfo.BL.Models;
using VRhfo.PL;

namespace VRhfo.BL
{
    public class CommentManager
    {
        public static CommentLikes CheckForExistingLikeEntry(Guid commentId, Guid userId)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    tblCommentLike cl = dc.tblCommentLikes.FirstOrDefault(cl => cl.UserId == userId && cl.CommentId == commentId);
                    
                    if (cl != null)
                    {
                        CommentLikes clm = new CommentLikes()
                        {
                            UserId = cl.UserId,
                            CommentId = cl.CommentId,
                            CreatedAt = (DateTime)cl.CreatedAt,
                            IsLike = cl.IsLike,
                            Id = cl.Id
                        };
                        return clm;
                    }          
                    return null;                   
                }
            }
            catch (Exception)
            {

                throw;
            }
            throw new NotImplementedException();
        }

        public static List<Comment> GetCommentsByVideoId(int videoId)
        {
            using (VRhfoEntities db = new VRhfoEntities())
            {
                List<tblComment> tblComments = db.tblComments
                    .Where(c => c.VideoId == videoId)
                    .Include(c => c.User)
                    .OrderByDescending(c => c.DatePosted)
                    .ToList();

                List<Comment> comments = new List<Comment>();

                foreach (var tblComment in tblComments)
                {
                    comments.Add(new Comment
                    {
                        Content = tblComment.Content,
                        DatePosted = tblComment.DatePosted,
                        UserId = tblComment.UserId,
                        Id = tblComment.Id,
                        User = new User
                        {
                            Username = tblComment.User.Username,
                            Subscription_Tier = tblComment.User.SubscriptionTier
                        },                        
                    });
                }

                return comments;
            }
        }

        public static int InsertComment(Comment newComment)
        {
            try
            {
                int result;
                using(VRhfoEntities dc = new VRhfoEntities())
                {
                    tblComment newRow = new tblComment();
                    newRow.Content = newComment.Content;
                    newRow.DatePosted = newComment.DatePosted;  
                    newRow.UserId = newComment.UserId;
                    newRow.VideoId = newComment.VideoId;
                    newRow.Id = Guid.NewGuid();
                   /* newRow.DislikesCount = 0;
                    newRow.LikesCount = 0;
*/
                    dc.tblComments.Add(newRow);

                    result = dc.SaveChanges();  
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static int InsertLikeDislikeEntry(CommentLikes newLike)
        {
            try
            {
                int results = 0;

                using(VRhfoEntities dc = new VRhfoEntities())
                {
                    tblCommentLike cl = new tblCommentLike();
                    cl.CreatedAt = newLike.CreatedAt;
                    cl.UserId = newLike.UserId;
                    cl.CommentId = newLike.CommentId;
                    cl.IsLike = newLike.IsLike; 

                    dc.tblCommentLikes.Add(cl);
                    return dc.SaveChanges();   

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static int InsertReply(Reply newReply)
        {
            try
            {
                int result = 0;

                using(VRhfoEntities dc = new VRhfoEntities())
                {
                    tblReply newRow = new tblReply();
                    newRow.Content = newReply.Content;
                    newRow.DatePosted = newReply.DatePosted;
                    newRow.LikesCount = newReply.LikesCount;
                    newRow.DislikesCount = newReply.DislikesCount;
                    newRow.Id = newReply.Id;
                    newRow.UserId = newReply.UserId;
                    newRow.CommentId = newReply.CommentId;
                   
                    dc.tblReplies.Add(newRow);

                    result = dc.SaveChanges();
                }

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static int LoadDislikeCount(Guid commentId)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    // Count the number of dislikes for the given commentId
                    int dislikeCount = dc.tblCommentLikes
                        .Where(cl => cl.CommentId == commentId && cl.IsLike == false)
                        .Count();

                    return dislikeCount;
                }
            }
            catch (Exception ex)
            {
                throw ex; // You can log the exception before throwing if necessary
            }
        }

        public static int LoadLikeCount(Guid commentId)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                {
                    // Count the number of likes for the given commentId
                    int likeCount = dc.tblCommentLikes
                        .Where(cl => cl.CommentId == commentId && cl.IsLike == true)
                        .Count();                    
                    return likeCount;
                }
            }
            catch (Exception ex)
            {
                throw ex; // You can log the exception before throwing if necessary
            }
        }

        public static List<Reply> LoadListOfReplies(Guid id)
        {
            try
            {
                using (VRhfoEntities dc = new VRhfoEntities())
                { // Retrieve all replies associated with the given commentId
                    var replies = dc.tblReplies
                                    .Where(r => r.CommentId == id)
                                    .OrderBy(r => r.DatePosted) // Optionally, you can order by the date posted
                                    .Select(r => new Reply // Project each tblReply to a Reply object
                                    {
                                        Id = r.Id,
                                        Content = r.Content,
                                        DatePosted = r.DatePosted,
                                        UserId = r.UserId,
                                        CommentId = r.CommentId,
                                        LikesCount = r.LikesCount,
                                        DislikesCount = r.DislikesCount
                                    })
                                    .ToList();

                    return replies;
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static int UpdateLikeDislikeEntry(CommentLikes existingLike)
        {
            try
            {
                int results = 0;

                using(VRhfoEntities dc = new VRhfoEntities())
                {
                    var row = dc.tblCommentLikes.FirstOrDefault(s => s.Id == existingLike.Id);

                    if (row != null)
                    {
                        row.IsLike = existingLike.IsLike;
                        return dc.SaveChanges();
                    }
                    else
                    {
                        return 0; // Indicating no rows were affected
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
