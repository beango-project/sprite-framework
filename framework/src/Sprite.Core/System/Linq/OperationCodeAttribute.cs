namespace System.Linq
{
    public class OperationCodeAttribute
    {
        public OperationCodeAttribute(string code)
        {
            Code = code;
        }

        public OperationCodeAttribute(string code, string name) : this(code)
        {
            Name = name;
        }

        /// <summary>
        ///     获取 属性名称
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        ///     前台名称
        /// </summary>
        public string Name { get; private set; }
    }
}