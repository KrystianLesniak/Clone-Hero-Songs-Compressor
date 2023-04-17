﻿using FFMpegCore;
using SongsCompressor.Common.Enums;
using SongsCompressor.Common.Interfaces;
using SongsCompressor.Common.Models;
using System.Reflection;

namespace Engine.FFmpegProvider
{
    //This engine may seem dull but in case of future this engine will be very important.
    //For things like providing FFmpeg paltform-wise or auto-downloading one
    public class ProvideFFmpegEngine : ICompressionEngine
    {
        public int ExecutionOrder => 10;

        public static ICompressionEngine? Create(IList<OptionsEnum> options)
        {
            if (options.Contains(OptionsEnum.ConvertAudioToOpus))
                return new ProvideFFmpegEngine();

            return null;
        }

        public Task<EngineProgressStatus> GetCurrentProgress()
        {
            return Task.FromResult(new EngineProgressStatus
            {
                WorkDescription = "Preparing FFmpeg"
            });
        }

        public Task Start()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

            if (File.Exists(Path.Combine(assemblyDirectory, "ffmpeg.exe")))
            {
                GlobalFFOptions.Configure(options =>
                    options.BinaryFolder = assemblyDirectory
                );
            }

            //TODO: need to handle it somehow if ffmpeg is not present in main directory
            return Task.CompletedTask;
        }
    }
}