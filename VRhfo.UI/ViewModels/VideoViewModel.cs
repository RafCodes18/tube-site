using VRhfo.BL.Models;

namespace VRhfo.UI.Views.ViewModels
{
    public class VideoViewModel
    {
        public Video video { get; set; }

        public List<Video> suggestedVideos { get; set; }
        public User user{ get; set; }

        public string likeState { get; set; } 
        
        public Guid LoggedInUserId {  get; set; }
        public Guid notLoggedInUserId {  get; set; }

    }
}
