using System;
using System.Windows.Media;

namespace Snake
{
    public static class Audio
    {
        public readonly static MediaPlayer GameOver = LoadAudio("Fortunate Son.mp3");
        public readonly static MediaPlayer Background = LoadAudio("Vietnam Music.mp3");
        public readonly static MediaPlayer Start = LoadAudio("Good Morning.mp3");

        private static MediaPlayer LoadAudio(string filename, double volume = 1, bool repeat = false, bool autoReset = true)
        {
            MediaPlayer player = new();
            player.Volume = volume;
            player.Open(new Uri($"Assets/{filename}", UriKind.Relative));

            if (repeat)
            {
                player.MediaEnded += PlayerRepeat_MediaEnded;
            }

            if (autoReset)
            {
                player.MediaEnded += Player_MediaEnded;
            }

            return player;
        }

        private static void Player_MediaEnded(object sender, EventArgs e)
        {
            MediaPlayer m = sender as MediaPlayer;
            m.Position = new TimeSpan(0);
        }

        private static void PlayerRepeat_MediaEnded(object sender, EventArgs e)
        {
            MediaPlayer m = sender as MediaPlayer;
            m.Stop();
            m.Position = new TimeSpan(0);
            m.Play();
        }
    }
}
