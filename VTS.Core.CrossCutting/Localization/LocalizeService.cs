using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace VTS.Core.CrossCutting
{
    public class LocalizeService : ILocalizeService
    {
        private ILocalizer _localizer;
        private IConfiguration _configuration;
        private const int rus = 1;
        private const int en = 2;
        const string localizeFile = "localization.tsv";          
        private Dictionary<String, String> data;
        public LocalizeService(ILocalizer localizer, IConfiguration configuration)
        {            
           _localizer = localizer;
            _configuration = configuration;
        }
        public void LoadLocalization(string language = null)
        {
            var config = new Configuration();   
            if (language == null)
            {
                if (_configuration.GetDefaultLanguage == DefaultLanguage.System)
                {
                            language = _localizer.GetCurrentCultureInfo();
                }
                else
                {
                    language = _configuration.GetDefaultLanguage.ToString();
                }               
            }

            if (language == DefaultLanguage.System.ToString())
            {
                language = _localizer.GetCurrentCultureInfo();
            }

            try
            {
                switch (language)
                {
                    case "en":
                        {
                            data = GetLocalizeDictionary(en);
                            break;
                        }
                    case "ru":
                        {
                            data =  GetLocalizeDictionary(rus);                            
                            break;
                        }
                    default: {

                            data = GetLocalizeDictionary(en);                       
                            break;
                    }
                }
            }
            catch
            {   
            }
        }

        public string Localize(string key)
        {
            string result = @"N/A";
            if (data == null || data.Count == 0)
            {
                LoadLocalization();
            }

            if (data != null && data.Count != 0)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    key = key.ToLower();
                    if (data.ContainsKey(key))
                    {
                        result = data[key];
                    }
                }
            }
            return result;
        }
        private Dictionary<string, string> GetLocalizeDictionary(int langNumber)
        {
            var dictionaryLocalize = new Dictionary<string, string>();
            try
            {
                Type classType = typeof(LocalizeService);
                var assembly = classType.GetTypeInfo().Assembly;
                var nameSpace =  classType.Namespace;
                string embeddedresource = string.Format("{0}.{1}", nameSpace, localizeFile);                
                Stream stream = assembly.GetManifestResourceStream(embeddedresource);                
                using (TextReader tr = new StreamReader(stream))
                {
                    string line;
                    while ((line = tr.ReadLine()) != null)
                    {
                        string[] items = line.Split('\t');
                        if (items.Length > 2)
                        {
                            try
                            {
                                dictionaryLocalize.Add(items[0].ToLower(), items[langNumber]);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            return dictionaryLocalize;
        }        
    }
}