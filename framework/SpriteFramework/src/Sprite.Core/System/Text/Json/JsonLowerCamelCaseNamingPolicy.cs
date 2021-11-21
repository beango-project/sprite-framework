namespace System.Text.Json
{
    public class JsonLowerCamelCaseNamingPolicy : JsonNamingPolicy

    {
        public override string ConvertName(string name)
        {
            var str = name.ToUpper();
            return str;
        }
    }
}