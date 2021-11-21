namespace System.Linq
{
    /// <summary>
    /// 过滤操作
    /// </summary>
    public enum FilterOperation
    {
        /// <summary>
        /// Data value is equal to selected value.
        /// </summary>
        Equal,

        /// <summary>
        /// Data value is not equal to selected value.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Data value is less than selected value.
        /// </summary>
        LessThan,

        /// <summary>
        /// Data value is less than or equal to selected value.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Data value is greater than selected value.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Data value is greater than or equal to selected value.
        /// </summary>
        GreaterThanOrEqual,

        And,

        Or,

        /// <summary>
        /// Data string contains selected value.
        /// </summary>
        Contains,

        /// <summary>
        /// Data string does not contain selected value.
        /// </summary>
        NotContains,

        /// <summary>
        /// Data string starts with selected value.
        /// </summary>
        StartsWith,

        /// <summary>
        /// Data string does not start with selected value.
        /// </summary>
        NotStartsWith,

        /// <summary>
        /// Data string ends with selected value.
        /// </summary>
        EndsWith,

        /// <summary>
        /// Data string does not end with selected value.
        /// </summary>
        NotEndsWith
    }
}