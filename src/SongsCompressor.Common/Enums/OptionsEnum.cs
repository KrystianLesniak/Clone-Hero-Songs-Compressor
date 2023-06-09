﻿namespace SongsCompressor.Common.Enums
{
    public enum OptionsEnum
    {
        None = 0,
        CreateBackup = 100,
        ConvertPngToJpg = 200,
        ResizeAlbum = 210,
        ConvertAudioToOpus = 300,
        ConvertAudioFromMp3 = 310,
        ConvertAudioFromOgg = 320,
    }

    public static class OptionsEnumLists
    {
        public static readonly OptionsEnum[] MainOptions =
        {
            OptionsEnum.ConvertPngToJpg,
            OptionsEnum.ConvertAudioToOpus,
        };
    }
}
