using System;
using System.IO;
using System.Reflection;
using PhoenixPoint.Modding;

namespace LootMod
{
    internal class Helper
    {


        public static void PrintPropertiesAndFields(object obj, ModMain instance, string prefix = "")
        {
            Type type = obj.GetType();

            PropertyInfo[] properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            AppendToFile($"{prefix}Properties of {type.Name}:");
            foreach (var property in properties)
            {
                try
                {
                    string name = property.Name;
                    object value = property.GetValue(obj);
                    AppendToFile($"{prefix}{name}: {value}");
                }
                catch (Exception ex)
                {
                    AppendToFile($"{prefix}Error accessing property {property.Name}: {ex.Message}");
                }
            }


            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            AppendToFile($"{prefix}Fields of {type.Name}:");
            foreach (var field in fields)
            {
                try
                {
                    string name = field.Name;
                    object value = field.GetValue(obj);
                    AppendToFile($"{prefix}{name}: {value}");
                }
                catch (Exception ex)
                {
                    AppendToFile($"{prefix}Error accessing field {field.Name}: {ex.Message}");
                }
            }
        }

        public static void AppendToFile(string content, string filename = "LootModLog.txt")
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", filename);
            File.AppendAllText(filePath, content + "\n");
        }

        public static void DeleteFile(string filename = "LootModLog.txt")
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", filename);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                ModHandler.modInstance.Logger.LogInfo($"File '{filePath}' deleted successfully.");
            }
            else
            {
                ModHandler.modInstance.Logger.LogInfo($"File '{filePath}' not found.");
            }
        }

        public static void CopyFieldsByReflection(object src, object dst, BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Type srcType = src.GetType();
            foreach (FieldInfo dstFieldInfo in dst.GetType().GetFields(bindFlags))
            {
                FieldInfo srcField = srcType.GetField(dstFieldInfo.Name, bindFlags);
                if (srcField != null && srcField.Name != "Guid")
                {
                    dstFieldInfo.SetValue(dst, srcField.GetValue(src));
                }
            }
        }

    }
}
