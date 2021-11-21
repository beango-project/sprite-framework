namespace System.Linq
{
    public class FilterRule
    {
        /// <summary>
        /// 获取或设置 属性名称
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 获取或设置 属性值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 获取或设置 操作类型
        /// </summary>
        public FilterOperation Operation { get; set; }


        /// <summary>
        /// 初始化一个<see cref="FilterRule"/>的新实例
        /// </summary>
        public FilterRule() : this(null, null)
        {
        }

        // /// <summary>
        // /// 初始化一个<see cref="FilterRule"/>类型的新实例
        // /// </summary>
        // /// <param name="field">数据名称</param>
        // /// <param name="value">数据值</param>
        // /// <param name="operateCode">操作方式的前台码</param>
        // public FilterRule(string field, object value, string operateCode)
        //     : this(field, value, FilterHelper.GetFilterOperate(operateCode))
        // { }

        /// <summary>
        /// 使用指定数据名称，数据值及操作方式初始化一个<see cref="FilterRule"/>的新实例
        /// </summary>
        /// <param name="field">数据名称</param>
        /// <param name="value">数据值</param>
        /// <param name="operation">操作方式</param>
        public FilterRule(string field, object value, FilterOperation operation = FilterOperation.Equal)
        {
            Field = field;
            Value = value;
            Operation = operation;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is FilterRule rule))
            {
                return false;
            }

            return rule.Field == Field && rule.Value == Value && rule.Operation == Operation;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Field, Value, Operation);
        }
    }
}