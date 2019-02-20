namespace AppLocalizationUtil.Data.Destinations
{
    public class DestinationValueFormatUtils
    {
        public static string PrepareValueFormats(string value, string sourceType, string destinationType, int increaseIndex)
        {            
            value = value
                .Replace($"%{sourceType}", $"%{destinationType}");

            for (var i = 9 - 1; i >= 0; i--)
            {
                value = value.Replace($"%{i}${sourceType}", $"%{i + increaseIndex}${destinationType}");
            }

            return value;
        }

    }
}