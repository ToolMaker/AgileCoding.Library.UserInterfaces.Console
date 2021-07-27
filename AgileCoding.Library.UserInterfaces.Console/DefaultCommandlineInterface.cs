namespace AgileCoding.Library.UserInterfaces.Console
{
    using AgileCoding.Extentions.Loggers;
    using AgileCoding.Library.Interfaces.Logging;
    using AgileCoding.Library.Interfaces.UserInterfaces.Console;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DefaultCommandlineInterface : ICommandLineUserInterface
    {
        private ILogger logger = null;

        public List<string> HelpKeys { get; set; }

        public Func<string> HelpDescriptionStringFunction { get; set; } = () => string.Empty;

        public List<char?> PreFixChars { get; set; } = new List<char?>();

        public string InterfaceName { get; set; }

        public IOptionList Parameters { get; set; }

        public Func<bool, List<IOption>> ValidateParameters { get; set; }

        public StringComparison FunctionNameCaseComparrer { get; set; } = StringComparison.OrdinalIgnoreCase;

        public StringComparison HelpKeyNameCaseComparrer { get; set; } = StringComparison.OrdinalIgnoreCase;

        public Action<ILogger> DoWork { get; set; }

        public List<string> HelpKeysWithPrefixAdded { get; set; }
        public StringComparison InterfaceCaseComparrer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public DefaultCommandlineInterface(ILogger functionOptionLogger, string[] HelpKeys, char?[] PreFixChars)
        {
            this.logger = functionOptionLogger;
            this.HelpKeys = HelpKeys.ToList();
            HelpKeysWithPrefixAdded = ComonFunction.PopulateHelpKeysWithPrefixAdded(HelpKeys, PreFixChars);
            this.PreFixChars.AddRange(PreFixChars);
        }

        public void ProcessCommandLineArgs(string[] args)
        {
            if (Parameters.Any(x => x.Keys.Count == 0))
            {
                var paramsWithNoKeys = Parameters.Where(x => x.Keys.Count == 0).ToList();

                foreach (var paramsWithNoKey in paramsWithNoKeys)
                {
                    logger.WriteInformation($"Paramater given with no key(s). Please specify atleast one parameter key.");
                }
            }

            IOptionList requiredParameters = new DefaultOptionList();
            Parameters.
                Where(x => x.IsRequired).
                ToList().
                ForEach((x) => requiredParameters.Add(x));
            IOptionList optionalParameters = new DefaultOptionList();
            Parameters.Where(x => !x.IsRequired).ToList().ForEach((x) => optionalParameters.Add(x)); ;
            if (args.Length > 1)
            {
                if (HelpKeysWithPrefixAdded.Any(x => x.Equals(args[1], HelpKeyNameCaseComparrer)))
                {
                    StringBuilder paramStringAndSample = new StringBuilder($"Function Option : " + InterfaceName + Environment.NewLine + Environment.NewLine);
                    paramStringAndSample.AppendLine($"Parameter Perfix(es): '{string.Join("','", PreFixChars)}'" + Environment.NewLine);
                    int counter;

                    paramStringAndSample.Append(Environment.NewLine + $"Required Parameters: ");
                    CreateOptionalParamsMessage(requiredParameters, ref paramStringAndSample, out counter);

                    paramStringAndSample.Append(Environment.NewLine + $"Optional Parameters: ");
                    CreateOptionalParamsMessage(optionalParameters, ref paramStringAndSample, out counter);
                    paramStringAndSample.AppendLine();
                    CreateSampleValues(paramStringAndSample);

                    logger.WriteInformation(HelpDescriptionStringFunction() + Environment.NewLine + paramStringAndSample.ToString());
                    return;
                }
                else
                {
                    PopulateParamerters(args);
                }
            }


            if (ValidateRequiredParamerters(requiredParameters) && CustumValidation())
            {
                logger.WriteInformation($"Ready to start function '{InterfaceName}'");
                DoWork(this.logger);
                logger.WriteInformation($"Done doing work on function '{InterfaceName}'");
            }
        }

        private bool CustumValidation()
        {
            if (Parameters.Any((t) => !t.ValidationFunction()))
            {
                logger.WriteInformation(Parameters.ValidationErrorMessage.ToString());
                return false;
            }

            return true;
        }

        private bool ValidateRequiredParamerters(IOptionList requiredParameters)
        {
            bool validationResult = true; ;
            foreach (var param in requiredParameters)
            {
                if (string.IsNullOrEmpty(param.Value))
                {
                    logger.WriteInformation($"Validation of required option failed : '{param.Keys[param.KeyIndex]}'");

                    if (validationResult)
                    {
                        validationResult = false;
                    }
                }
            }

            if (!validationResult)
            {
                logger.WriteInformation($"{Environment.NewLine}Please fix the errors above.");
            }

            return validationResult;
        }

        private void PopulateParamerters(string[] paramertersList)
        {
            int counter = 0;
            foreach (var param in paramertersList)
            {
                if (counter++ > 0)
                {
                    foreach (IOption option in Parameters)
                    {
                        string[] keyVal = param.Split('=');
                        if (option.KeysWithPrefixAdded.Contains(keyVal[0]))
                        {
                            if (keyVal.Length == 1)
                            {
                                if (option.IsFlag)
                                {
                                    option.Value = "true";
                                }
                                else
                                {
                                    logger.WriteInformation($"It seems we specified no value for option '{option.Keys[option.KeyIndex]}'");
                                }
                            }
                            else if (keyVal.Length == 2)
                            {
                                if (option.IsFlag)
                                {
                                    logger.WriteInformation($"It seems we specified a vlaue for a flag. No need to specify a value for option flag '{option.Keys[option.KeyIndex]}'");
                                }
                                else
                                {
                                    option.Value = keyVal[1];
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException($"It seem the Option was not constructed correctly. Should be of 'Key=Value' however I have picked up more than one '=' sign.");
                            }
                        }
                    }

                }
            }
        }

        private void CreateSampleValues(StringBuilder paramStringAndSample)
        {
            paramStringAndSample.Append("TODO: Add sample string functionality");
        }

        private static void CreateRequiredParamsMessage(List<IOption> requiredParameters, StringBuilder paramStirng, out int counter)
        {
            counter = 1;
            if (requiredParameters.Count > 0)
            {
                foreach (var param in requiredParameters)
                {
                    paramStirng.AppendLine($"Param {counter++} possible Key(s) : {string.Join(",", param.Keys.ToArray())}, to get help use : {string.Join(",", param.HelpKeys.ToArray())}");
                }
            }
            else
            {
                paramStirng.AppendLine("None");
            }
        }

        private static void CreateOptionalParamsMessage(IOptionList optionalParameters, ref StringBuilder paramStirng, out int counter)
        {
            counter = 1;
            if (optionalParameters.Count > 0)
            {
                paramStirng.AppendLine();
                foreach (var param in optionalParameters)
                {
                    paramStirng.AppendLine($"Param {counter++} possible Key Name(s) : [{string.Join("],[", param.Keys.ToArray())}], \t\t to get help use : [{string.Join("],[", param.HelpKeysWithPrefixAdded.ToArray())}]");
                }
            }
            else
            {
                paramStirng.AppendLine("None");
            }
        }

        public void InitializeParameters()
        {
            throw new NotImplementedException();
        }
    }
}
