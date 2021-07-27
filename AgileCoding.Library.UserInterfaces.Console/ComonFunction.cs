namespace AgileCoding.Library.UserInterfaces.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ComonFunction
    {
        public static List<string> PopulateHelpKeysWithPrefixAdded(string[] HelpKeys, char?[] PreFixChars)
        {
            List<string> KeysWithPrefixAdded = new List<string>();
            for (int i = 0; i < HelpKeys.Length; i++)
            {
                for (int x = 0; x < PreFixChars.Length; x++)
                {
                    KeysWithPrefixAdded.Add(PreFixChars[x] + HelpKeys[i]);
                }
            }

            return KeysWithPrefixAdded;
        }

        public static Dictionary<string, string> ToParamtersDictionary(this string[] args, List<char?> preFixChars)
        {
            Dictionary<string, string> parametersList = new Dictionary<string, string>();
            if (args != null)
            {

                foreach (var param in args)
                {
                    string newParam = param;
                    if (preFixChars.Any(x => x !=null && param.StartsWith(x.ToString())))
                    {
                        newParam = param.Substring(1);
                    }

                    if (!newParam.Contains("="))
                    {
                        parametersList.Add(newParam, null);
                    }

                    string[] keyValue = newParam.Split('=');
                    parametersList.Add(keyValue[0], keyValue[1]);
                }
            }
            return parametersList;
        }
    }
}
