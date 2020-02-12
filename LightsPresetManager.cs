using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;


namespace Daniels.Lighting
{
    public static class LightsPresetManager
    {

        public enum ePresetInitializationSuccessFailureReasons
        {
            PresetsDirectoryNotFound = -1,
            Success = 0
        }

        public static bool Ready = false;
        public static readonly string PresetsDirectoryName = @"NVRAM\LightPresets";
        public static readonly string ConfigFileName = @"LightsConfig.json";

        public static ePresetInitializationSuccessFailureReasons Initialize()
        {
            CreateResources();
            if (PresetsDirectoryExists)
            {
                //ReadConfig();
                Ready = true;
                return ePresetInitializationSuccessFailureReasons.Success;
            }
            else
                return ePresetInitializationSuccessFailureReasons.PresetsDirectoryNotFound;
        }

        private static void CreateResources()
        {
            try
            {
                if (Directory.Exists(PresetsDirectoryName) != true)
                {
                    Directory.CreateDirectory(PresetsDirectoryName);
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Failed to create directory: `{0}` - Reason = {1}", PresetsDirectoryExists, e.Message);
            }
        }

        public static bool PresetsDirectoryExists
        {
            get { return Directory.Exists(PresetsDirectoryName); }
        }

        public static string PresetFilePath(string presetName, LightGroup lightGroup)
        {
            return String.Format("{0}\\{1}-{2}.json", PresetsDirectoryName, lightGroup.Name, presetName);
        }

        public static bool PresetFileExists(string presetName, LightGroup lightGroup)
        {
            return File.Exists(PresetFilePath(presetName, lightGroup));
        }

        public static void Save(string name, LightGroup lightGroup)
        {
            string presetFilePath = PresetFilePath(name, lightGroup);
            try
            {
                using (StreamWriter writer = new StreamWriter(presetFilePath))
                {
                    writer.WriteLine(lightGroup.SavePreset());
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Failed to write the preset file `{0}` - Reason = {1}", presetFilePath, e.Message);
            }
            finally { }

        }

        public static void Load(string name, LightGroup lightGroup)
        {
            string presetFilePath = PresetFilePath(name, lightGroup);
            string presetData = String.Empty;

            try
            {

                using (StreamReader reader = new StreamReader(presetFilePath))
                {
                    presetData = reader.ReadToEnd();
                }

                if (String.IsNullOrEmpty(presetData) != true)
                {
                    lightGroup.ApplyPreset(presetData);
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Failed to read the preset file `{0}` - Reason: {1}", presetFilePath, e.Message);
            }
            finally { }

        }

        public static void Delete(string name, LightGroup lightGroup)
        {
            string presetFilePath = PresetFilePath(name, lightGroup);

            try
            {
                File.Delete(presetFilePath);
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Failed to delete the preset file `{0}` - Reason: {1}", presetFilePath, e.Message);
            }
            finally { }

        }

        public static string[] List(LightGroup lightGroup)
        {
            string[] files = Directory.GetFiles(PresetsDirectoryName, lightGroup.Name + "-*.json");
            string[] presets = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                presets[i] = Path.GetFileNameWithoutExtension(files[i]).Substring(lightGroup.Name.Length+1);
            }

            return presets;
        }

    }
}