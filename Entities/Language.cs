namespace AppLocalizationUtil.Entities
{
    public class Language
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}