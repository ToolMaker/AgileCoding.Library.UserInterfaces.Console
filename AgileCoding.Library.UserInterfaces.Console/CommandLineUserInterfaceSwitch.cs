namespace AgileCoding.Library.UserInterfaces.Console
{
    using AgileCoding.Extentions.Loggers;
    using AgileCoding.Library.Interfaces.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AgileCoding.Library.Interfaces.UserInterfaces.Console;

    public class CommandLineUserInterfaceSwitch : ICommandLineUserInterfaceSwitch
    {
        private ILogger logger;

        public CommandLineUserInterfaceSwitch(ILogger logger, string[] helpKeys, char?[] preFixChars, Func<string> helpStringFunction)
        {
            this.logger = logger;
            HelpKeys = helpKeys.ToList();
            HelpDescriptionStringFunction = helpStringFunction;
            PreFixChars = preFixChars.ToList();
            HelpKeysWithPrefixAdded = ComonFunction.PopulateHelpKeysWithPrefixAdded(helpKeys, preFixChars);
        }

        public List<ICommandLineUserInterface> UserInterfaces { get; set; } = new List<ICommandLineUserInterface>();

        public List<string> HelpKeys { get; set; }

        public Func<string> HelpDescriptionStringFunction { get; set; }

        public StringComparison HelpKeyNameCaseComparrer { get; set; }

        public List<char?> PreFixChars { get; set; }

        public List<string> HelpKeysWithPrefixAdded { get; set; }

        public void ProcessCommandLineArgs(string[] args)
        {
            if (UserInterfaces.Count <= 0)
            {
                throw new InvalidOperationException("It seems no Command line userinterfaces was registered yet.");
            }

            if (args == null || args.Length == 0)
            {
                var character = PreFixChars.ToList().Where(x => x != null);
                string preFIxString = $"[{string.Join("|", character)}]";

                if (PreFixChars.Any(x => x == null))
                {
                    preFIxString = $"!{preFIxString}";
                }

                logger.WriteInformation($"It seems no userinterface was select. Please use any of the following help command(s):{Environment.NewLine}");
                logger.WriteInformation($"{AppDomain.CurrentDomain.FriendlyName} {preFIxString}[{string.Join("|", HelpKeys.ToArray())}]");

                return;
            }

            if (HelpKeysWithPrefixAdded.Any(x => x.Equals(args[0], HelpKeyNameCaseComparrer)))
            {
                logger.WriteInformation(HelpDescriptionStringFunction());
                return;
            }


            if (!UserInterfaces.Any(x => x.InterfaceName.ToLower().Equals(args[0], x.InterfaceCaseComparrer)))
            {
                logger.WriteInformation($"No userinterface defined with the name '{args[0]}'");
                return;
            }

            var selectedUserInterface = UserInterfaces.Where(x => x.InterfaceName.Equals(args[0], x.InterfaceCaseComparrer)).SingleOrDefault();

            if (selectedUserInterface != null)
            {
                selectedUserInterface.ProcessCommandLineArgs(args);
                selectedUserInterface.DoWork(logger);
            }
            else
            {
                logger.WriteError("I tested earlier and found a function operation but now I dont anymore. I was not expecting this error to happen at all.");
            }
        }

        public void RegisterUserInterface(ICommandLineUserInterface interfaceToRegister)
        {
            this.UserInterfaces.Add(interfaceToRegister);
        }

        public void RegisterUserInterfaces(ICommandLineUserInterface[] interfacesToRegister)
        {
            this.UserInterfaces.AddRange(interfacesToRegister);
        }
    }
}
