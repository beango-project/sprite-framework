namespace System.Reflection
{
    public static class ActivateFactory
    {
        public static T CreateInstance<T>()
        {
            var instance = New<int>.Instance();
            return default;
        }
    }
}