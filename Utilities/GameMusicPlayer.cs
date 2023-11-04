using AnotherLib;
using Microsoft.Xna.Framework.Audio;

namespace GameTemplate.Utilities
{
    public class GameMusicPlayer
    {
        public static SoundEffectInstance mainTheme;

        public static void Update()
        {
            if (mainTheme.State != SoundState.Playing)
            {
                mainTheme.Volume = GameData.MusicVolume;
                mainTheme.Play();
            }
        }

        public static void StopMusic()
        {
            mainTheme.Stop();
        }
    }
}
