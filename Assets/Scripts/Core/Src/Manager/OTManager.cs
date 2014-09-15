using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Core.Src.Manager
{
    public class OTManager
    {
        private Dictionary<string, string> _text = new Dictionary<string, string>();
        private static OTManager _instance = new OTManager();
        public static OTManager instance { get { return _instance; } }
        public OTManager()
        {
            if (_instance != null)
            {
                throw new UnityException("Error: Please use instance to get OTManager.");
            }
        }

        public bool HasOT(string textID)
        {
            return _text.ContainsKey(textID);
        }

        public void AddOT(string textID,string textContent)
        {
            if (!_text.ContainsKey(textID))
            {
                _text.Add(textID, textContent);
            }
            else
            {
                Log.Error("Error: Duplicate " + textID + " in OT files");
            }
        }

        public void AddOT(string text)
        {
            string[] noComment = Regex.Split(text, @"\s*//.*\s*");
            foreach (string item in noComment)
            {
                if (string.IsNullOrEmpty(item)) continue;
                MatchCollection matches = Regex.Matches(item, @"\w+=");
                string[] values = Regex.Split(item, @"\w+=");
                for (int i = 0; i < matches.Count; i++)
                {
                    string key = matches[i].Value;
                    key = key.Substring(0, key.Length - 1);
                    if (_text.ContainsKey(key))
                    {
                        Log.Error("Error: Duplicate " + key + " in OT files");
                        continue;
                    }
                    string value = values[i + 1];
                    value = Regex.Replace(value, @"[\t\r\n]+", "");//去掉TAB,回车符
                    value = Regex.Replace(value, @"\\n", "\n");//将\n替换成换行符
                    _text.Add(key, value);
                }
            }
        }

        public static string Get(string key, params string[] args)
        {
            return _instance.GetOT(key, args);
        }

        public string GetOT(string key, params string[] args)
        {
            if (_text.ContainsKey(key))
            {
                if (args.Length > 0)
                {
                    return Regex.Replace(_text[key], @"#\d", (match) =>
                    {
                        int index = int.Parse(match.Groups[0].Value.Substring(1));
                        return args[index - 1];
                    });
                }
                else
                {
                    return _text[key];
                }
            }
            else
            {
                return "";
            }
        }
    }
}