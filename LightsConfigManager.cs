using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Newtonsoft.Json;

namespace Daniels.Lighting
{
    public static class LightsConfigManager
    {
        public enum eConfigInitializationSuccessFailureReasons
        {
            ConfigFileNotFound = -1,
            Success = 0
        }

        public static bool Ready = false;
        public static readonly string ConfigDirectoryName = @"NVRAM";
        public static readonly string ConfigFileName = @"LightsConfig.json";
        private static LightsConfig _lightsConfig = new LightsConfig();


        public static eConfigInitializationSuccessFailureReasons Initialize()
        {
            if (ConfigFileExists)
            {
                ReadConfig();
                Ready = true;
                return eConfigInitializationSuccessFailureReasons.Success;
            }
            else
                return eConfigInitializationSuccessFailureReasons.ConfigFileNotFound;
        }

        public static string ConfigFilePath
        {
            get { return String.Format("{0}\\{1}", ConfigDirectoryName, ConfigFileName); }
        }

        public static bool ConfigFileExists
        {
            get { return File.Exists(ConfigFilePath); }
        }

        public static void CreateNewConfig(LightsConfig lightsConfig)
        {
            _lightsConfig = lightsConfig;
            WriteConfig();
            ReloadConfig();
        }

        public static void ReloadConfig()
        {
            //var originalConfig = _shadeConfig.Clone() as CrestronShadeConfig;
            ReadConfig();
        }

        private static void ReadConfig()
        {
            string configData = String.Empty;

            try
            {

                using (StreamReader reader = new StreamReader(ConfigFilePath))
                {
                    configData = reader.ReadToEnd();
                }

                if (String.IsNullOrEmpty(configData) != true)
                {
                    _lightsConfig = JsonConvert.DeserializeObject<LightsConfig>(configData);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Error("Failed to read the config file `{0}` - Reason: {1}", ConfigFilePath, e.Message);
                CrestronConsole.PrintLine("Failed to read the config file `{0}` - Reason: {1}", ConfigFilePath, e.Message);
            }
            finally { }
        }

        private static void WriteConfig()
        {
            try
            {
                if (Directory.Exists(ConfigDirectoryName) != true)
                {
                    Directory.CreateDirectory(ConfigDirectoryName);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Error("Failed to create directory: `{0}` - Reason = {1}", ConfigDirectoryName, e.Message);
                CrestronConsole.PrintLine("Failed to create directory: `{0}` - Reason = {1}", ConfigDirectoryName, e.Message);
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(ConfigFilePath))
                {
                    writer.WriteLine(CreateConfigData());

                }
            }
            catch (Exception e)
            {
                ErrorLog.Error("Failed to write the config file `{0}` - Reason = {1}", ConfigFilePath, e.Message);
                CrestronConsole.PrintLine("Failed to write the config file `{0}` - Reason = {1}", ConfigFilePath, e.Message);
            }
            finally { }
        }

        public static string CreateConfigData()
        {
            /*ErrorLog.Notice("Serializing config...");

            string s = JsonConvert.SerializeObject(_lightsConfig, Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            //JsonSerializer jsonSerializer = new JsonSerializer();
            
            //jsonSerializer.Serialize(

            ErrorLog.Notice("Serializing config... done:\r\n{0}", s);
            return s;*/
            return JsonConvert.SerializeObject(_lightsConfig, Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public static string[] Groups
        {
            get { return _lightsConfig.Lights.Keys.ToArray<String>(); }
        }

        public static List<DMXFixtureConfig> GroupFixtures(string group)
        {
            return _lightsConfig.Lights[group];
        }
    }
}