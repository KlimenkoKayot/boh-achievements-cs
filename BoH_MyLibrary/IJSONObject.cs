namespace BoH_MyLibrary
{
    internal interface IJSONObject
    {
        public IEnumerable<string> GetAllFields();
        public string? GetField(string fieldName);
        public void SetField(string fieldName, string value);
    }
}
