using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SaveYouTubeVideosStatistics
{
    [StorageAccount("BlobConnectionString")]
    public class Function1
    {
        string youtubeDataApiKey = "AIzaSyD_6EqgjekzS4JEd14GgYxHRoySIVMmzw4";

        [FunctionName("SaveYouTubeVideosStatistics")]
        public void Run([TimerTrigger("0 0 0/4 * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log,
            [Blob("youtube-data-json//videolist.json", System.IO.FileAccess.ReadWrite)] BlockBlobClient blobClient)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var videos = GetVideosFromPlaylist("PLln3PF6nQloueB1D4nhqg3j4Yl-ggS9vo").Result;

            List<VideoStatisticsSimpleModel> videoModels = new List<VideoStatisticsSimpleModel>();
            for (int i = 0; i < videos.items.Count; i++)
            {
                var video = videos.items[i];
                var videoModelServer = GetVideoStatistics(video.contentDetails.videoId).Result;
                var videoModel = ParseModel(videoModelServer);
                videoModels.Add(videoModel);
            }

            AppendVideosData(videoModels, blobClient);

            log.LogInformation($"VideoModels: {videoModels.Count}");
        }

        public void AppendVideosData(List<VideoStatisticsSimpleModel> videoModels, BlockBlobClient blobClient)
        {
            Azure.Response<Azure.Storage.Blobs.Models.BlobDownloadResult> videolistFile = blobClient.DownloadContent();
            //System.IO.Stream streamFile = blobClient.OpenRead();
            var stringFile = Encoding.ASCII.GetString(videolistFile.Value.Content);

            var youTubeStatisticsModels = JsonConvert.DeserializeObject<List<VideoStatisticsSimpleModel>>(stringFile) ?? new List<VideoStatisticsSimpleModel>();
            youTubeStatisticsModels.AddRange(videoModels);

            string serializedVideoList = JsonConvert.SerializeObject(youTubeStatisticsModels);
            var content = Encoding.UTF8.GetBytes(serializedVideoList);
            using (var memoryStream = new MemoryStream(content))
            {
                blobClient.Upload(memoryStream);
            }
        }

        private async Task<YouTubePlaylistModel> GetVideosFromPlaylist(string playlistId)
        {
            var parameter = new Dictionary<string, string>
            {
                ["key"] = youtubeDataApiKey,
                ["playlistId"] = playlistId,
                ["part"] = "contentDetails",
                ["maxResults"] = "3"
            };

            var baseUrl = "https://www.googleapis.com/youtube/v3/playlistItems?";
            var fullUrl = MakeURLFromQuery(baseUrl, parameter);

            var result = await new HttpClient().GetStringAsync(fullUrl);

            if (!string.IsNullOrEmpty(result))
            {
                return JsonConvert.DeserializeObject<YouTubePlaylistModel>(result);
            }

            return null;
        }

        private async Task<YouTubeStatisticsSnippetModel?> GetVideoStatistics(string videoId)
        {
            var parameter = new Dictionary<string, string>
            {
                ["key"] = youtubeDataApiKey,
                ["part"] = "statistics&part=snippet",
                ["id"] = videoId,
            };

            var baseUrl = "https://youtube.googleapis.com/youtube/v3/videos?";
            var fullUrl = MakeURLFromQuery(baseUrl, parameter);

            var result = await new HttpClient().GetStringAsync(fullUrl);

            if (!string.IsNullOrEmpty(result))
            {
                return JsonConvert.DeserializeObject<YouTubeStatisticsSnippetModel>(result);
            }

            return null;
        }

        private string MakeURLFromQuery(string baseUrl, Dictionary<string, string> parameter)
        {
            return parameter.Aggregate(baseUrl, (accumulated, par) => string.Format($"{accumulated}{par.Key}={par.Value}&"));
        }

        private VideoStatisticsSimpleModel ParseModel(YouTubeStatisticsSnippetModel statisticsSnippetModel)
        {
            var result = new VideoStatisticsSimpleModel()
            {
                Timestamp = DateTime.Now,
                statistics = statisticsSnippetModel.items[0].statistics,
                channelId = statisticsSnippetModel.items[0].snippet.channelId,
                title = statisticsSnippetModel.items[0].snippet.title,
                channelTitle = statisticsSnippetModel.items[0].snippet.channelTitle,
                videoId = statisticsSnippetModel.items[0].id,
            };

            return result;
        }
    }


}
