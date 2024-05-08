﻿CREATE TABLE tblVideosLiked (
    UserID INT,
    VideoID INT NOT NULL,
    LikedDate DATETIME NOT NULL,
    FOREIGN KEY (UserID) REFERENCES tblUser(Id) ON DELETE CASCADE,
    FOREIGN KEY (VideoID) REFERENCES tblVideo(Id),
    PRIMARY KEY (UserID, VideoID)
);