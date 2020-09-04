using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using Newtonsoft.Json;

namespace Daniels.Lighting
{
    public class LightsControl : IEnumerable<LightGroup>
    {
        private static LightsControl _instance;

        private BasicTriList _transport;
        internal List<LightGroup> _lightGroups = new List<LightGroup>();

        private LightsControl(BasicTriList transport)
        {
            _transport = transport;
        }

        public static LightsControl GetInstance(BasicTriList transport)
        {
            if (_instance == null)
            {
                _instance = new LightsControl(transport);
            }
            return _instance;
        }

        public static LightsControl GetInstance()
        {
            if (_instance == null)
            {
                throw new NullReferenceException("LightsControl is not initialized");
            }
            return _instance;
        }

        public void Initialize()
        {
            try
            {
                if (LightsConfigManager.Initialize() != LightsConfigManager.eConfigInitializationSuccessFailureReasons.Success)
                {
                    var configObj = new LightsConfig();

                    configObj.Profiles = new Dictionary<string,DMXFixtureProfileConfig>(2)
                    {
                        {"ETC Source Four LED Studio HD", new DMXFixtureProfileConfig()
                            {
                                Id = 1, Name = "ETC Source Four LED Studio HD", DMXChannels = new Dictionary<DMXChannel,ushort>()
                                {
                                    {DMXChannel.Intensity, 1},
                                    {DMXChannel.WhitePoint, 2},
                                }
                            }
                        },
                        {"Elation Satura Profile Basic", new DMXFixtureProfileConfig()
                            {
                                Id = 2, Name = "Elation Satura Profile Basic", DMXChannels = new Dictionary<DMXChannel,ushort>()
                                {
                                    {DMXChannel.Intensity, 15},
                                    {DMXChannel.Pan, 1},
                                    {DMXChannel.Tilt, 2},
                                    {DMXChannel.Zoom, 20},
                                    {DMXChannel.Iris, 18},
                                    {DMXChannel.Focus, 19},
                                    {DMXChannel.Blade1, 21},
                                    {DMXChannel.Blade1Rotate, 22},
                                    {DMXChannel.Blade2, 23},
                                    {DMXChannel.Blade2Rotate, 24},
                                    {DMXChannel.Blade3, 25},
                                    {DMXChannel.Blade3Rotate, 26},
                                    {DMXChannel.Blade4, 27},
                                    {DMXChannel.Blade4Rotate, 28},
                                },
                            }
                        },
                        {"Elation Satura Profile Standard", new DMXFixtureProfileConfig()
                            {
                                Id = 3, Name = "Elation Satura Profile Standard", DMXChannels = new Dictionary<DMXChannel,ushort>()
                                {
                                    {DMXChannel.Intensity, 17},
                                    {DMXChannel.Pan, 1},
                                    {DMXChannel.PanFine, 2},
                                    {DMXChannel.Tilt, 3},
                                    {DMXChannel.TiltFine, 4},
                                    {DMXChannel.Zoom, 22},
                                    {DMXChannel.Iris, 20},
                                    {DMXChannel.Focus, 21},
                                    {DMXChannel.Blade1, 23},
                                    {DMXChannel.Blade1Rotate, 24},
                                    {DMXChannel.Blade2, 25},
                                    {DMXChannel.Blade2Rotate, 26},
                                    {DMXChannel.Blade3, 27},
                                    {DMXChannel.Blade3Rotate, 28},
                                    {DMXChannel.Blade4, 29},
                                    {DMXChannel.Blade4Rotate, 30},
                                },
                            }
                        },
                    };

                    configObj.Lights = new Dictionary<string, List<DMXFixtureConfig>>(3)
                    {
                        { "Podium", new List<DMXFixtureConfig>(2)
                            {
                                new DMXFixtureConfig(){ Id = 1, Name = "South #1", Profile = configObj.Profiles["ETC Source Four LED Studio HD"], BaseDMXChannel = 11 },
                                new DMXFixtureConfig(){ Id = 2, Name = "North #4", Profile = configObj.Profiles["ETC Source Four LED Studio HD"], BaseDMXChannel = 41 },
                            }
                        },
                        { "Stage", new List<DMXFixtureConfig>(4)
                            {
                                new DMXFixtureConfig(){ Id = 3, Name = "South #2", Profile = configObj.Profiles["ETC Source Four LED Studio HD"], BaseDMXChannel = 21 },
                                new DMXFixtureConfig(){ Id = 4, Name = "South #3", Profile = configObj.Profiles["ETC Source Four LED Studio HD"], BaseDMXChannel = 31 },
                                new DMXFixtureConfig(){ Id = 5, Name = "North #5", Profile = configObj.Profiles["ETC Source Four LED Studio HD"], BaseDMXChannel = 51 },
                                new DMXFixtureConfig(){ Id = 6, Name = "North #6", Profile = configObj.Profiles["ETC Source Four LED Studio HD"], BaseDMXChannel = 61 },
                            }
                        },
                        { "PTZ", new List<DMXFixtureConfig>(2)
                            {
                                new DMXFixtureConfig(){ Id = 7, Name = "North", Profile = configObj.Profiles["Elation Satura Profile Standard"], BaseDMXChannel = 256,},
                                new DMXFixtureConfig(){ Id = 8, Name = "South", Profile = configObj.Profiles["Elation Satura Profile Standard"], BaseDMXChannel = 310,},
                            }
                        },
                    };

                    LightsConfigManager.CreateNewConfig(configObj);
                }

                if (LightsPresetManager.Initialize() != LightsPresetManager.ePresetInitializationSuccessFailureReasons.Success)
                {
                    ErrorLog.Error("Failed to initialize presets");
                }

                foreach (string group in LightsConfigManager.Groups)
                {
                    //List<DMXFixture> groupDmxFixtures = new List<DMXFixture>(LightsConfigManager.GroupFixtures(group).Count);

                    LightGroup lightGroup = new LightGroup((uint)(_lightGroups.Count+1), group);
                    _lightGroups.Add(lightGroup);

                    foreach (DMXFixtureConfig dmxFixtureConfig in LightsConfigManager.GroupFixtures(group))
                    {
                        DMXFixture dmxFixture;
                        switch (dmxFixtureConfig.Profile.Name)
                        {
                            case "ETC Source Four LED Studio HD":
                                dmxFixture = new DMXFixture(lightGroup, dmxFixtureConfig, _transport);
                                break;
                            case "Elation Satura Profile Standard":
                                dmxFixture = new DMXPTZFixture(lightGroup, dmxFixtureConfig, _transport);
                                break;
                            default:
                                dmxFixture = new DMXFixture(lightGroup, dmxFixtureConfig, _transport);
                                break;
                        }
                    }
                }

                CrestronConsole.AddNewConsoleCommand(ConsoleCommandLightGroups, "lightgroups", "List light groups. Use \"LIGHTGROUPS ?\" for more info.", ConsoleAccessLevelEnum.AccessOperator);
                CrestronConsole.AddNewConsoleCommand(ConsoleCommandLightGroup, "lightgroup", "Light master commands. Use \"LIGHTGROUP ?\" for more info.", ConsoleAccessLevelEnum.AccessOperator);
                CrestronConsole.AddNewConsoleCommand(ConsoleCommandLights, "lights", "Lighting commands. Use \"LIGHTS ?\" for more info.", ConsoleAccessLevelEnum.AccessOperator);
                CrestronConsole.AddNewConsoleCommand(ConsoleCommandDMXTransport, "dmx", "DMX Transport commands. Use \"DMX ?\" for more info.", ConsoleAccessLevelEnum.AccessOperator);
                CrestronConsole.AddNewConsoleCommand(ConsoleCommandTest, "test", "TEST commands. Use \"TEST ?\" for more info.", ConsoleAccessLevelEnum.AccessOperator);

                ErrorLog.Notice(">>> LightsControl: initialized successfully");
            }
            catch (Exception e)
            {
                ErrorLog.Error(">>> LightControl: Error in InitializeSystem: {0}\r\n{1}", e.Message, e.StackTrace);
            }

        }

        public LightGroup[] Masters
        {
            get { return _lightGroups.ToArray(); }
        }

        public LightGroup Find(Predicate<LightGroup> match)
        {
            return _lightGroups.Find(match);
        }

        #region IEnumerable<LightGroup> Implementation

        public IEnumerator<LightGroup> GetEnumerator()
        {
            return _lightGroups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable<LightGroup> Implementation


        #region Console Commands

        /// <summary>
        /// LIGHTGROUPS helper console function.
        /// </summary>
        /// <param _name="cmd">command _name</param>
        private void ConsoleCommandLightGroups(string cmd)
        {
            const string re = @"\G(""((""""|[^""])+)""|(\S+)) *";
            const string usage = "Usage:\r\n\t lightgroups";

            var ms = Regex.Matches(cmd, re);
            string[] args = ms.Cast<Match>().Select(m => Regex.Replace(m.Groups[2].Success ? m.Groups[2].Value : m.Groups[4].Value, @"""""", @"""")).ToArray();

            if (args.Length > 0)
            {
                CrestronConsole.ConsoleCommandResponse(usage);
                return;
            }

            StringBuilder response = new StringBuilder();
            response.AppendLine("LightGroups:");

            foreach (LightGroup lightGroup in _lightGroups)
            {
                response.AppendFormat("{0}: {1}\t\t{2}\r\n", lightGroup.Id, lightGroup.Name, (lightGroup.Muted) ? "ON" : "OFF");
            }
            CrestronConsole.ConsoleCommandResponse(response.ToString());
        }


        /// <summary>
        /// LIGHT helper console function.
        /// </summary>
        /// <param _name="cmd">command _name</param>
        private void ConsoleCommandLightGroup(string cmd)
        {
            const string re = @"\G(""((""""|[^""])+)""|(\S+)) *";
            const string usage = "Usage:\r\n\t lightgroup <id>|<name> [<ON|OFF|<intensity>>|save <name>|load <name>|list]";

            var ms = Regex.Matches(cmd, re);
            string[] args = ms.Cast<Match>().Select(m => Regex.Replace(m.Groups[2].Success ? m.Groups[2].Value : m.Groups[4].Value, @"""""", @"""")).ToArray();
            
            if (args.Length >= 1)
            {
                LightGroup lightGroup = _lightGroups.Find(g => g.Name == args[0]);
                if (lightGroup == null)
                {
                    try
                    {
                        uint id = Convert.ToUInt32(args[0]);
                        lightGroup = _lightGroups.Find(g => g.Id == id);
                        if (lightGroup == null)
                            throw new ArgumentOutOfRangeException("args[0]");
                    }
                    catch (Exception)
                    {
                        CrestronConsole.ConsoleCommandResponse("LightGroup with id or name \"{0}\" can't be found", args[0]);
                        return;
                    }
                }


                if (args.Length >= 2)
                {
                    switch (args[1])
                    {
                        case "ON":
                            lightGroup.Muted = false;
                            break;
                        case "OFF":
                            lightGroup.Muted = true;
                            break;
                        case "save":
                            //preset = lightMaster.SavePreset();
                            //CrestronConsole.ConsoleCommandResponse(preset);
                            if (args.Length >= 3 && !String.IsNullOrEmpty(args[2]))
                            {
                                string presetName = args[2];
                                LightsPresetManager.Save(presetName, lightGroup);
                            }
                            else
                                CrestronConsole.ConsoleCommandResponse("ERROR: Preset name is invalid");
                            break;
                        case "load":
                            if (args.Length >= 3 && !String.IsNullOrEmpty(args[2])&& LightsPresetManager.PresetFileExists(args[2], lightGroup))
                            {
                                string presetName = args[2];
                                LightsPresetManager.Load(presetName, lightGroup);
                            }
                            else
                                CrestronConsole.ConsoleCommandResponse("ERROR: Preset could not be found");
                            break;
                        case "list":
                            StringBuilder response = new StringBuilder("Presets for " + lightGroup.Name+"\r\n");
                            foreach (string presetName in LightsPresetManager.List(lightGroup))
                            {
                                response.AppendLine(presetName);
                            }
                            CrestronConsole.ConsoleCommandResponse(response.ToString());
                            break;
                        default:
                            try
                            {
                                ushort intensity = Convert.ToUInt16(args[1]);
                                lightGroup.Intensity = intensity;
                            }
                            catch (Exception)
                            {
                                CrestronConsole.ConsoleCommandResponse("\"{0}\" can't be converted to ether ON, OFF or Intensity value.", args[1]);
                                return;
                            }

                            break;
                    }

                }
                else // args.Length == 1
                {
                    StringBuilder response = new StringBuilder();
                    response.AppendLine("LightGroup:");
                    response.AppendLine(lightGroup.ToString());
                    CrestronConsole.ConsoleCommandResponse(response.ToString());
                }
            }
            else
                CrestronConsole.ConsoleCommandResponse(usage);
        }

        /// <summary>
        /// LIGHT helper console function.
        /// </summary>
        /// <param _name="cmd">command _name</param>
        private void ConsoleCommandLights(string cmd)
        {
            const string re = @"\G(""((""""|[^""])+)""|(\S+)) *";
            const string usage = "Usage:\r\n\t lights <id> [ON|OFF|<intensity>|preset <presetId> <newPresetName>]";

            var ms = Regex.Matches(cmd, re);
            string[] args = ms.Cast<Match>().Select(m => Regex.Replace(m.Groups[2].Success ? m.Groups[2].Value : m.Groups[4].Value, @"""""", @"""")).ToArray();
            
            if (args.Length >= 1)
            {
                LightFixture lightFixture;
                try
                {
                    uint id = Convert.ToUInt32(args[0]);
                    lightFixture = GetLightFixtureById(id);
                    if (lightFixture == null)
                        throw new ArgumentOutOfRangeException("args[0]");
                }
                catch (Exception)
                {
                    CrestronConsole.ConsoleCommandResponse("LightFixture can't be found by id \"{0}\"", args[0]);
                    return;
                }

                if (args.Length >= 2)
                {
                    switch (args[1])
                    {
                        case "ON":
                            lightFixture.Muted = false;
                            break;
                        case "OFF":
                            lightFixture.Muted = true;
                            break;
                        case "preset":
                            CrestronConsole.ConsoleCommandResponse(lightFixture.SavePreset());
                            break;
                        case "load":
                            break;
                        default:
                            try
                            {
                                ushort intensity = Convert.ToUInt16(args[1]);
                                lightFixture.Intensity = intensity;
                            }
                            catch (Exception)
                            {
                                CrestronConsole.ConsoleCommandResponse("\"{0}\" can't be converted to ether ON, OFF or Intensity value.", args[1]);
                                return;
                            }

                            break;
                    }

                }
                else // args.Length == 1
                {
                    StringBuilder response = new StringBuilder();
                    response.AppendLine("LightFixture:");
                    response.AppendLine(lightFixture.ToString());
                    CrestronConsole.ConsoleCommandResponse(response.ToString());
                }
            }
            else
                CrestronConsole.ConsoleCommandResponse(usage);
        }

        /// <summary>
        /// LIGHT helper console function.
        /// </summary>
        /// <param _name="cmd">command _name</param>
        private void ConsoleCommandDMXTransport(string cmd)
        {
            const string re = @"\G(""((""""|[^""])+)""|(\S+)) *";
            const string usage = "Usage:\r\n\t dmx <channel> <value>";

            var ms = Regex.Matches(cmd, re);
            string[] args = ms.Cast<Match>().Select(m => Regex.Replace(m.Groups[2].Success ? m.Groups[2].Value : m.Groups[4].Value, @"""""", @"""")).ToArray();

            if (args.Length == 2)
            {
                uint channelNumber = 0;
                try
                {
                    channelNumber = Convert.ToUInt32(args[0]);
                    if (channelNumber < 1 || channelNumber > 512)
                        throw new ArgumentOutOfRangeException("args[0]");
                }
                catch (Exception)
                {
                    CrestronConsole.ConsoleCommandResponse("DMX transport channel can't be converted \"{0}\"", args[0]);
                    return;
                }
                int channelValue = -1;
                try
                {
                    channelValue = Convert.ToInt32(args[1]);
                    if (channelValue < 0 || channelValue > 255)
                        throw new ArgumentOutOfRangeException("args[1]");
                }
                catch (Exception)
                {
                    CrestronConsole.ConsoleCommandResponse("DMX transport channel value can't be converted \"{0}\"", args[1]);
                    return;
                }

                _transport.UShortInput[channelNumber].UShortValue = (ushort)channelValue;
                CrestronConsole.ConsoleCommandResponse("DMX channel {0} set to {1}", channelNumber, channelValue);
            }
            else
                CrestronConsole.ConsoleCommandResponse(usage);
        }



        /// <summary>
        /// Test helper console function.
        /// </summary>
        /// <param _name="cmd">command _name</param>
        private void ConsoleCommandTest(string cmd)
        {
            //string s = LightsConfigManager.CreateConfigData();

            List<string> l = new List<string>(2);
            l.Add("test1");
            l.Add("test2");
            string s = JsonConvert.SerializeObject(l, Formatting.Indented);

            CrestronConsole.ConsoleCommandResponse(s);
        }

        #endregion Console Commands

        private LightFixture GetLightFixtureById(uint id)
        {
            //return _lightGroups.SelectMany((g, i)=> g[i]).FirstOrDefault(l => l.Id == id);
            //return _lightGroups.SelectMany(g => g.Lights).FirstOrDefault(l => l.Id == id);
            foreach (var lightGroup in _lightGroups)
            {
                foreach (var light in lightGroup)
                {
                    if (light.Id == id)
                        return light;
                }
            }
            return null;
        }
    }
}