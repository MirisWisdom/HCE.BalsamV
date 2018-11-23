﻿using System;
using System.IO;
using Atarashii.Common;
using Atarashii.Modules.Profile;

namespace Atarashii.CLI.Commands
{
    /// <inheritdoc />
    /// <summary>
    ///     CLI front-end for loading a lastprof.txt file.
    /// </summary>
    internal class Profile : Command
    {
        public Profile(Output output) : base(output)
        {
        }

        public override void Initialise(string[] commands)
        {
            ExitIfNone(commands);
            var args = GetArguments(commands);

            switch (commands[0])
            {
                case nameof(Resolve):
                    Resolve(args);
                    break;
                case nameof(Detect):
                    Detect();
                    break;
                case nameof(Parse):
                    Parse(args);
                    break;
                default:
                    Fail("Invoked an invalid Profile command.", ExitType.IncorrectArguments);
                    break;
            }
        }

        private void Detect()
        {
            Info("Invoked the Profile.Detect command.");

            try
            {
                var result = LastprofFactory.Get(LastprofFactory.Type.Detect, Output);
                Console.WriteLine(result.Parse());
            }
            catch (FileNotFoundException e)
            {
                Fail(e.Message, ExitType.Exception);
            }
            catch (ProfileException e)
            {
                Fail(e.Message, ExitType.Exception);
            }
        }

        private void Resolve(string[] args)
        {
            Info("Invoked the Profile.Resolve command.");
            ExitIfNone(args);

            try
            {
                Console.WriteLine(new Lastprof(File.ReadAllText(args[0])).Parse());
            }
            catch (ProfileException e)
            {
                Fail(e.Message, ExitType.Exception);
            }
        }

        private void Parse(string[] args)
        {
            Info("Invoked the Profile.Parse command.");
            ExitIfNone(args);

            try
            {
                var configuration = ConfigurationFactory.GetFromStream(File.Open(args[0], FileMode.Open));
                Console.WriteLine();
                Dump("+ Profile ------------------------------------------------------");
                Dump($"  - Name                : {configuration.Name}");
                Dump($"  - Colour              : {configuration.Colour.ToString()}");
                Dump("+ Mouse --------------------------------------------------------");
                Dump("  + Sensitivity");
                Dump($"    - Horizontal        : {configuration.Mouse.Sensitivity.Horizontal}");
                Dump($"    - Vertical          : {configuration.Mouse.Sensitivity.Vertical}");
                Dump($"  - InvertVerticalAxis  : {configuration.Mouse.InvertVerticalAxis}");
                Dump("+ Audio --------------------------------------------------------");
                Dump("  + Volume");
                Dump($"    - Master            : {configuration.Audio.Volume.Master}");
                Dump($"    - Effects           : {configuration.Audio.Volume.Effects}");
                Dump($"    - Music             : {configuration.Audio.Volume.Music}");
                Dump($"  - Quality             : {configuration.Audio.Quality.ToString()}");
                Dump($"  - Variety             : {configuration.Audio.Variety.ToString()}");
                Dump("+ Video --------------------------------------------------------");
                Dump("  + Resolution");
                Dump($"    - Width             : {configuration.Video.Resolution.Width}");
                Dump($"    - Height            : {configuration.Video.Resolution.Height}");
                Dump($"  - RefreshRate         : {configuration.Video.RefreshRate.Value}");
                Dump($"  - FrameRate           : {configuration.Video.FrameRate.ToString()}");
                Dump("  + Effects");
                Dump($"    - Specular          : {configuration.Video.Effects.Specular}");
                Dump($"    - Shadows           : {configuration.Video.Effects.Shadows}");
                Dump($"    - Decals            : {configuration.Video.Effects.Decals}");
                Dump($"  - Particles           : {configuration.Video.Particles.ToString()}");
                Dump($"  - Quality             : {configuration.Video.Quality.ToString()}");
                Dump("+ Network -------------------------------------------------------");
                Dump($"  - Connection          : {configuration.Network.Connection.ToString()}");
                Dump("  + Port");
                Dump($"    - Server            : {configuration.Network.Port.Server}");
                Dump($"    - Client            : {configuration.Network.Port.Client}");
                Console.WriteLine();
                Pass("Successfully parsed data from the provided blam.sav binary.");
            }
            catch (Exception e)
            {
                Fail(e.Message, ExitType.Exception);
            }
        }
    }
}