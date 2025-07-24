namespace VRhfo.BL.Models
{
    public class Video
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string ThumbnailUrl { get; set; }
        public string PreviewVideoURL { get; set; }
        public string VideoUrl { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public DateTime UploadDate { get; set; }
        public TimeSpan Duration { get; set; }
        public List<string> Tags { get; set; }
        public List<Comment> Comments { get; set; }
        public int Views { get; set; }
        public int RatingCount { get; set; }
        public bool IsPublic { get; set; }  //For full length vids that are free, but not previews
        public bool IsPreview { get; set; }// If True, returns 5 min preview, if false must be paid.

        public DateTime? LastDateWatched { get; set; }
        public User user { get; set; }

        public string TruncatedTitle
        {
            get{                
                return Title.Length <= 60 ? Title : Title.Substring(0, 60) + "...";
            }
                
        }


        public string FormattedUploadDateDMY
        {
            get
            {
                return UploadDate.ToString("MMM dd, yyyy");
            }
        }

        public string FormattedDuration
        {
            get
            {
                if (Duration.TotalHours >= 1)
                {
                    return $"{Duration.Hours}h {Duration.Minutes}m";
                }
                else
                {
                    return $"{Duration.Minutes}m";
                }
            }
        }

        public string FormattedLikes
        {
            get
            {
                if (Likes < 1000)
                {
                    return Likes.ToString();
                }
                else if (Likes < 1000000)
                {
                    return (Likes / 1000.0).ToString("F1") + "k";
                }
                else
                {
                    return (Likes / 1000000.0).ToString("F1") + "m";
                }
            }
        }

        public string FormattedDislikes
        {
            get
            {
                if (Dislikes < 1000)
                {
                    return Dislikes.ToString();
                }
                else if (Dislikes < 1000000)
                {
                    return (Dislikes / 1000.0).ToString("F1") + "k";
                }
                else
                {
                    return (Dislikes / 1000000.0).ToString("F1") + "m";
                }
            }
        }

        //Think IceCube, "EXPLICIT CONTENT" sold more NWA albums. Viewers of my site are degenerates, like me, adding "WARNING: STRONG HYPNOSIS", or "WARNING: STRONG HYPNOTIC CONTENT"  adds to the experience.
        public string ContentWarning { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }

        public double AverageRating
        {
            get
            {
                int totalVotes = Likes + Dislikes;
                return totalVotes > 0 ? (int)((double)Likes / totalVotes * 100) : 0;
            }
        }

        public string FormattedViews
        {
            get
            {
                if (Views < 1000)
                {
                    return Views.ToString();
                }
                else if (Views < 1000000)
                {
                    double value = Views / 1000.0;
                    return (value == Math.Floor(value) ? ((int)value).ToString() : value.ToString("0.#")) + "k";
                }
                else
                {
                    double value = Views / 1000000.0;
                    return (value == Math.Floor(value) ? ((int)value).ToString() : value.ToString("0.#")) + "M";
                }
            }
        }




        public string FormattedUploadDate
        {
            get
            {
                var currentDate = DateTime.UtcNow;
                var dateDifference = currentDate - UploadDate;
                if (dateDifference.TotalDays < 1)
                {
                    return "Today";
                }
                else if (dateDifference.TotalDays < 2)
                {
                    return "1 day ago";
                }
                else if (dateDifference.TotalDays < 7)
                {
                    return $"{(int)dateDifference.TotalDays} days ago";
                }
                else if (dateDifference.TotalDays < 14)
                {
                    return "1 week ago";
                }
                else if (dateDifference.TotalDays < 30)
                {
                    return $"{(int)(dateDifference.TotalDays / 7)} weeks ago";
                }
                else if (dateDifference.TotalDays < 60)
                {
                    return "1 month ago";
                }
                else if (dateDifference.TotalDays < 365)
                {
                    return $"{(int)(dateDifference.TotalDays / 30)} months ago";
                }
                else if (dateDifference.TotalDays < 730)
                {
                    return "1 year ago";
                }
                else
                {
                    return $"{(int)(dateDifference.TotalDays / 365)} years ago";
                }
            }
        }


    }
}