using System.Collections.Generic;

namespace System.Linq
{
    public class FilterGroup
    {
        private FilterOperation _operation;

        public FilterGroup()
        {
        }

        public FilterOperation Operation
        {
            get => _operation;
            set
            {
                if (value != FilterOperation.And && value != FilterOperation.Or)
                {
                    throw new InvalidOperationException("The operation type in the query condition group is wrong, and it can only be And or Or.");
                }

                _operation = value;
            }
        }

        public List<FilterRule> Rules { get; set; }

        public List<FilterGroup> Groups { get; set; }
    }
}