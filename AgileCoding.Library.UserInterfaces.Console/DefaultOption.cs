namespace AgileCoding.Library.UserInterfaces.Console
{
    using AgileCoding.Library.Interfaces.UserInterfaces.Console;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DefaultOption : IOption
    {
        public DefaultOption(string name, char? preFixChar, string helpArgumentKey = null, Func<string> helpStringFucntion = null)
        {
            ValidateKey(name);

            Keys.Add(name);
            InitSingleValues(preFixChar, helpArgumentKey, helpStringFucntion);
            InitHelpPrefixKeys(helpArgumentKey, preFixChar);
            InitArumentKeyWithPrefixKey(name, preFixChar);
        }

        public DefaultOption(string[] names, char? preFixChar, string helpArgumentKey = null, Func<string> helpStringFucntion = null)
        {
            ValidateKeys(names);

            List<KeyValuePair<string, string>> namesPair = new List<KeyValuePair<string, string>>();
            names.ToList().ForEach((x) => namesPair.Add(new KeyValuePair<string, string>(x, null)));

            InitSingleValues(preFixChar, helpArgumentKey, helpStringFucntion);
            InitHelpPrefixKeys(helpArgumentKey, preFixChar);
            InitArumentKeyWithPrefixKey(names, preFixChar);
        }

        private void InitArumentKeyWithPrefixKey(string[] names, char? preFixChar)
        {
            KeysWithPrefixAdded = ComonFunction.PopulateHelpKeysWithPrefixAdded(names, new char?[] { preFixChar });
        }

        public DefaultOption(string[] names, char?[] preFixChars, string helpArgumentKey = null, Func<string> helpStringFucntion = null)
        {
            if (names.Any(x => string.IsNullOrEmpty(x)))
            {
                throw new InvalidOperationException("Tried to initialize a arugment with no name specified.");
            }

            List<KeyValuePair<string, string>> namesPair = new List<KeyValuePair<string, string>>();
            names.ToList().ForEach((x) => namesPair.Add(new KeyValuePair<string, string>(x, null)));

            InitChars(preFixChars, helpArgumentKey, helpStringFucntion);
            InitHelpPrefixKeys(helpArgumentKey, preFixChars);
            InitArumentKeyWithPrefixKeys(names, preFixChars);
        }

        private void InitArumentKeyWithPrefixKeys(string[] names, char?[] preFixChars)
        {
            KeysWithPrefixAdded = ComonFunction.PopulateHelpKeysWithPrefixAdded(names, preFixChars);
        }

        public DefaultOption(string[] names, char?[] preFixChars, string[] helpArgumentKeys = null, Func<string> helpStringFucntion = null)
        {
            if (names.Any(x => string.IsNullOrEmpty(x)))
            {
                throw new InvalidOperationException("Tried to initialize a arugment with no name specified.");
            }

            List<KeyValuePair<string, string>> namesPair = new List<KeyValuePair<string, string>>();
            names.ToList().ForEach((x) => namesPair.Add(new KeyValuePair<string, string>(x, null)));

            InitMultipleValues(preFixChars, helpArgumentKeys, helpStringFucntion);
            InitHelpPrefixKeys(helpArgumentKeys, preFixChars);
            InitArumentKeysWithPrefixKeys(names, preFixChars);
        }

        private void InitArumentKeysWithPrefixKeys(string[] names, char?[] preFixChars)
        {
            KeysWithPrefixAdded = ComonFunction.PopulateHelpKeysWithPrefixAdded(names, preFixChars);
        }

        private void InitMultipleValues(char?[] preFixChars, string[] helpArgumentKeys, Func<string> helpStringFucntion)
        {
            InitPrefixChars(preFixChars);
            InitHelpKeys(helpArgumentKeys);

            HelpDescriptionStringFunction = helpStringFucntion;
            InitHelpPrefixKeys(helpArgumentKeys, preFixChars);
        }

        private void InitChars(char?[] preFixChars, string helpArgumentKey, Func<string> helpStringFucntion)
        {
            InitPrefixChars(preFixChars);
            InitHelpKey(helpArgumentKey);

            HelpDescriptionStringFunction = helpStringFucntion;
            InitHelpPrefixKeys(helpArgumentKey, preFixChars);
        }

        private void InitHelpPrefixKeys(string helpArgumentKey, char?[] preFixChars)
        {
            HelpKeysWithPrefixAdded = ComonFunction.PopulateHelpKeysWithPrefixAdded(new string[] { helpArgumentKey }, preFixChars);
        }

        private void InitSingleValues(char? preFixChar, string helpArgumentKey, Func<string> helpStringFucntion)
        {
            InitPrefixChar(preFixChar);
            InitHelpKey(helpArgumentKey);

            HelpDescriptionStringFunction = helpStringFucntion;
            InitHelpPrefixKeys(helpArgumentKey, preFixChar);
        }

        private void InitHelpPrefixKeys(string helpArgumentKey, char? preFixChar)
        {
            HelpKeysWithPrefixAdded = ComonFunction.PopulateHelpKeysWithPrefixAdded(new string[] { helpArgumentKey }, new char?[] { preFixChar });
        }

        private void InitArumentKeyWithPrefixKey(string argumentKey, char? preFixChar)
        {
            KeysWithPrefixAdded = ComonFunction.PopulateHelpKeysWithPrefixAdded(new string[] { argumentKey }, new char?[] { preFixChar });
        }

        private void InitPrefixChar(char? preFixChar)
        {
            PreFixChars = new List<char?>();
            PreFixChars.Add(preFixChar);
        }

        private void InitPrefixChars(char?[] preFixChars)
        {
            PreFixChars = new List<char?>();
            PreFixChars.AddRange(preFixChars);
        }

        private void InitHelpKeys(string[] helpArgumentKeys)
        {
            HelpKeys = new List<string>();
            HelpKeys.AddRange(helpArgumentKeys);
        }

        private void InitHelpPrefixKeys(string[] helpArgumentKeys, char?[] preFixChars)
        {
            HelpKeysWithPrefixAdded = ComonFunction.PopulateHelpKeysWithPrefixAdded(helpArgumentKeys, preFixChars);
        }

        private void InitHelpKey(string helpArgumentKey)
        {
            HelpKeys = new List<string>();
            HelpKeys.Add(helpArgumentKey);
        }

        public static void ValidateKeys(string[] names)
        {
            if (names.Any(x => string.IsNullOrEmpty(x)))
            {
                throw new InvalidOperationException("Tried to initialize a arugment with no name specified.");
            }
        }

        private void ValidateKey(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("Tried to initialize a arugment with no name specified.");
            }
        }

        public List<string> Keys { get; set; } = new List<string>();

        public List<string> HelpKeys { get; set; }

        public Func<string> HelpDescriptionStringFunction { get; set; }

        public List<char?> PreFixChars { get; set; }

        public bool IsRequired { get; set; }

        public StringComparison HelpKeyNameCaseComparrer { get; set; }

        public List<string> SampleValues { get; set; }

        public Func<bool> ValidationFunction { get; set; }

        public bool IsFlag { get; set; }

        public List<string> HelpKeysWithPrefixAdded { get; set; }

        public List<string> KeysWithPrefixAdded { get; set; }

        public int KeyIndex { get; set; }

        public string Value { get; set; }

        public string ValidationErrorMessage { get; set; }

        public StringComparison OptionNameCaseComparrer { get; set; }
    }
}
