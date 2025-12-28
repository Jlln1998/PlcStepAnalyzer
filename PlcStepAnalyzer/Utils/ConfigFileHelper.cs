using PlcStepAnalyzer.Config;
using PlcStepAnalyzer.Model;
using Serilog;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlcStepAnalyzer.Utils
{
    public class ConfigFileHelper
    {
        private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppDatas", "Configs.json");

        public static OpResult SaveConfig(DataConfig config)
        {
            var result = new OpResult();
            try
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    PropertyNameCaseInsensitive = true
                };
                string jsonString = JsonSerializer.Serialize(config, jsonOptions);
                File.WriteAllText(ConfigFilePath, jsonString, System.Text.Encoding.UTF8);
                result.IsSuccess = true;
                result.Message = "配置保存成功";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "配置保存失败" + ex.Message;
                Log.Error("保存配置文件失败：{ErrorMessage}", ex.Message);
            }
            return result;
        }

        public static DataConfig? LoadConfig()
        {
            try
            {
                if (!File.Exists(ConfigFilePath))
                {
                    return new DataConfig(); 
                }
                string jsonString = File.ReadAllText(ConfigFilePath, System.Text.Encoding.UTF8);
                var config = JsonSerializer.Deserialize<DataConfig>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return config;
            }
            catch (Exception ex)
            {
                Log.Error("加载配置文件失败：{ErrorMessage}", ex.Message);
                return null;
            }
        }
    }

}
