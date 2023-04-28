using SongsCompressor.Common.Enums;
using System.Collections.ObjectModel;

namespace SongsCompressor.Common.Models
{
    public class UserSettings
    {
        public UserSettings()
        {
            Options = new List<OptionsEnum>()
            {
                //Put default options here
                OptionsEnum.ConvertPngToJpg,
                OptionsEnum.ResizeAlbum,
                OptionsEnum.ConvertAudioToOpus,
                OptionsEnum.ConvertAudioFromMp3,
                OptionsEnum.ConvertAudioFromOgg
            };
            Directories = new List<string>();
        }

        public IList<OptionsEnum> Options { get; set; }
        public IList<string> Directories { get; set; }
    }
}
