using System.Collections.Generic;
using System;

namespace SaveYouTubeVideosStatistics
{
    public class VideoStatisticsSimpleModel
    {
        public DateTime Timestamp { get; set; }
        public Statistics statistics { get; set; }
        public string channelTitle { get; set; }
        public string channelId { get; set; }
        public string videoId { get; set; }
        public string title { get; set; }
    }
}
