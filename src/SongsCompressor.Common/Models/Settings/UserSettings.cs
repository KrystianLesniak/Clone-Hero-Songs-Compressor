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
                OptionsEnum.ConvertAudioToOpus,
                OptionsEnum.ConvertAudioFromMp3,
                OptionsEnum.ConvertAudioFromOgg
            };
            Directories = new List<DirectoryInfo>();
        }

        public IList<OptionsEnum> Options { get; set; }
        public IList<DirectoryInfo> Directories { get; set; }
    }
}
