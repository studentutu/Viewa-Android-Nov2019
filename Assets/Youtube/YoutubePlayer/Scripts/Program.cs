using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using YoutubeExtractor;

public class Program : MonoBehaviour
{
    public string url;
    public int quality;

    void Start()
    {
        Run();
    }

    public async void Run()
    {
        IEnumerable<VideoInfo> videoInfos = await DownloadUrlResolver.GetDownloadUrlsAsync(url);
        VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == quality);  

        if(video.RequiresDecryption)
        {
            DownloadUrlResolver.DecryptDownloadUrl(video);
        }

        GetComponent<MediaPlayerCtrl>().m_strFileName = video.DownloadUrl;
    }
}
