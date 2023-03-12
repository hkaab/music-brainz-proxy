namespace MusicBrainzApi.Extensions
{
    public class LowerCaseString
    {
        private readonly string _value;

        public LowerCaseString(string value)
        {
            _value = (value ?? string.Empty).ToLower();
        }

        public static implicit operator LowerCaseString(string value)
        {
            return new LowerCaseString(value);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}