# SaveYouTubeVideosStatistics
Hi, I am Mykola Lehkyi and I am Software Engineer.
This repository contains a C# Azure Function that retrieves YouTube video statistics and stores them in Azure Blob Storage. The function is triggered every four hours and updates a JSON file with the latest video statistics.

Azure part: https://github.com/mykolalehkyi/SaveYouTubeVideoStatistics/
Web Api part: https://github.com/mykolalehkyi/YT_Data_Api
Angular client part: https://github.com/mykolalehkyi/epz-youtube-api 

## Prerequisites
Before you begin, ensure you have the following:
- An active Azure account.
- Azure Storage Account and a Blob Container.
- YouTube Data API key.
- .NET SDK installed on your local machine.

## Configuration
Azure Storage Account: Update the local.settings.json file with your Azure Storage Account connection string.
Copy code
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "<Your Blob Connection String>",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "BlobConnectionString": "<Your Blob Connection String>"
  }
}
```
YouTube Data API Key: Update the youtubeDataApiKey variable in the Function1 class with your YouTube Data API key.
```csharp
string youtubeDataApiKey = "YOUR_YOUTUBE_DATA_API_KEY";
```
The function is set to trigger every four hours. It retrieves the latest video statistics from a specified YouTube playlist and appends the data to a JSON file stored in Azure Blob Storage.

## Code Overview
- Namespace: SaveYouTubeVideosStatistics
- Class: Function1
- youtubeDataApiKey: Stores the YouTube Data API key.
- Run: The main function triggered by a TimerTrigger every four hours. It retrieves video statistics and appends them to a JSON file in Azure Blob Storage.
- AppendVideosData: Appends the retrieved video statistics to the existing JSON file in Azure Blob Storage.
- GetVideosFromPlaylist: Retrieves videos from a specified YouTube playlist.
- GetVideoStatistics: Retrieves statistics for a specific YouTube video.
- MakeURLFromQuery: Constructs a URL from the base URL and query parameters.
- ParseModel: Parses the YouTube statistics and snippet model into a simple model.
## Models
- YouTubePlaylistModel: Represents the response model for a YouTube playlist.
- YouTubeStatisticsSnippetModel: Represents the response model for YouTube video statistics and snippet.
- VideoStatisticsSimpleModel: A simplified model for storing video statistics.
