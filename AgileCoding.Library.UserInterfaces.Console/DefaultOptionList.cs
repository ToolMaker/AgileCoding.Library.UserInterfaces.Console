namespace AgileCoding.Library.UserInterfaces.Console
{
    using AgileCoding.Library.Interfaces.UserInterfaces.Console;
    using System.Collections.Generic;
    using System.Text;

    public class DefaultOptionList : List<IOption>, IOptionList
    {
        public StringBuilder ValidationErrorMessage { get; set; }
    }
}
