using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerWeb : MonoBehaviour
{
    public string videoFileName = "Han-To Go.mp4";
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (source) => source.Play();
    }
}
