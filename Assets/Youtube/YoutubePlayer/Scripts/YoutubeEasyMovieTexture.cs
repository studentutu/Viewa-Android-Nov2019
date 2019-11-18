using UnityEngine;
using System.Collections;

public class YoutubeEasyMovieTexture : MonoBehaviour
{

    public bool playLowQualityAudioInBackground;

    public void LoadYoutubeInTexture(string youtubeVideoIdOrUrl)
    {
        /*
         *  IF YOU HAVE EASY MOVIE TEXTURE, ADD THIS SCRIPT IN THE SAME GAME OBJECT AS THE "MediaPlayerCtrl" ARE
         *  Then uncomment these lines below.
         */
        this.gameObject.GetComponent<MediaPlayerCtrl>().m_strFileName = YoutubeVideo.Instance.RequestVideo(youtubeVideoIdOrUrl,720);


        ////THIS IS JUST A WIP ||||| Below lines Not working yet
        //if (playLowQualityAudioInBackground)
        //{
        //    this.gameObject.GetComponent<MediaPlayerCtrl>().m_strFileName = YoutubeVideo.Instance.RequestVideo(youtubeVideoIdOrUrl, 360);
        //}else
        //{
        //    this.gameObject.GetComponent<MediaPlayerCtrl>().m_strFileName = YoutubeVideo.Instance.RequestVideo(youtubeVideoIdOrUrl, 1080);
        //}
    }
}
