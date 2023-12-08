using Montreal.Core.Crosscutting.Common.Enum;

namespace Montreal.Core.Crosscutting.Common.Data
{
    public class Filter
    {
        public string Property { get; set; }
        public Condition Condition { get; set; }
        public string Value { get; set; }

        public Filter(string property, Condition condition, string value)
        {
            Property = property;
            Condition = condition;
            Value = value;
        }
    }
}