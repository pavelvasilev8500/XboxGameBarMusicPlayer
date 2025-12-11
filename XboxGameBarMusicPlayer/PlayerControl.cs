namespace XboxGameBarMusicPlayer
{
    internal static class PlayerControl
    {
        public static void Play() => Init.Player.Play();

        public static void Pause() => Init.Player.Pause();

        public static void Repeate()
        {
            Init.Player.IsLoopingEnabled = !Init.Player.IsLoopingEnabled;
        }
    }
}
