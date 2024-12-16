using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using I2.Loc;
using PhoenixPoint.Modding;

namespace LootMod
{
    /// <summary>
    /// creates an empty localization.csv.
    /// expects that other classes tell it to write lines in to the csv with AddLine()
    /// expects that AddLocalizationFromCSV() is called after all lines are written to add the localization to the main game. 
    /// </summary>
    internal class LocalizationHandler
    {
        private string localizationFile;

        internal LocalizationHandler() {
            localizationFile = Path.Combine(ModHandler.modInstance.Instance.Entry.Directory, "Assets", "Localization", "LootMod.csv");
            CreateLocalizationFile();
        }

        // Ensures the CSV file exists with the correct headers
        private void CreateLocalizationFile()
        {
            using (StreamWriter writer = new StreamWriter(localizationFile, false, Encoding.UTF8))
            {
                writer.WriteLine("Key,Type,Desc,English,Chinese (Simplified),French,German,Italian,Polish,Russian,,UI_Tester    ");
            }
        }

        // Adds a line to the CSV file
        public void AddLine(string key, string text)
        {
            using (StreamWriter writer = new StreamWriter(localizationFile, true, Encoding.UTF8))
            {
                string line = $"{key}, Text,,\"{text}\",,,,,,,,";
                writer.WriteLine(line);
            }
        }

        // Read localization from CSV file
        public void AddLocalizationFromCSV(string Category = null)
        {
            if (File.Exists(localizationFile))
            {
                try
                {
                    string CSVstring = File.ReadAllText(localizationFile);
                    if (!CSVstring.EndsWith("\n"))
                    {
                        CSVstring += "\n";
                    }
                    LanguageSourceData SourceToChange = Category == null ? // if category is not given
                        LocalizationManager.Sources[0] :                   // use fist language source
                        LocalizationManager.Sources.First(source => source.GetCategories().Contains(Category));
                    if (SourceToChange != null)
                    {
                        int numBefore = SourceToChange.mTerms.Count;
                        _ = SourceToChange.Import_CSV(string.Empty, CSVstring, eSpreadsheetUpdateMode.AddNewTerms, ',');
                        LocalizationManager.LocalizeAll(true);    // Force localing all enabled labels/sprites with the new data
                        int numAfter = SourceToChange.mTerms.Count;
                        ModHandler.modInstance.Logger.LogInfo($"Added {numAfter - numBefore} terms from {localizationFile} in localization source {SourceToChange}, category: {Category}");
                    }
                    else
                    {
                        ModHandler.modInstance.Logger.LogInfo($"No language source with category {Category} found!");
                    }
                }
                catch (Exception e)
                {
                    ModHandler.modInstance.Logger.LogInfo($"Localisation Import failed: {e}");
                }
            }
            else
            {
                ModHandler.modInstance.Logger.LogInfo($"couldnt find localozation file {localizationFile}");
            }

            
        }
    }
}
