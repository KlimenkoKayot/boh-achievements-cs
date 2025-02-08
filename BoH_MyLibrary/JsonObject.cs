namespace BoH_MyLibrary
{
    public class JsonObject : IJSONObject
    {
        private Dictionary<string, string> _fields;
        public JsonObject(string json)
        {
            _fields = JsonParser.Deserialize(json);
        }
        public JsonObject(Dictionary<string, string> data) => _fields = data;

        public IEnumerable<string> GetAllFields()
        {
            return _fields.Keys;
        }

        public void SetField(string fieldName, string value)
        {
            if (!_fields.ContainsKey(fieldName))
            {
                throw new KeyNotFoundException($"key with name: {fieldName} not found");
            }
            _fields[fieldName] = value;
        }

        public string? GetField(string fieldName)
        {
            if (!_fields.ContainsKey(fieldName))
            {
                return null;
            }
            return _fields[fieldName];
        }

        public IEnumerable<JsonObject>? GetArrayField(string fieldName)
        {
            if (!_fields.ContainsKey(fieldName))
            {
                return null;
            }

            var listWithDict = JsonParser.DeserializeArray(_fields[fieldName]);
            IEnumerable<JsonObject> result = listWithDict.AsEnumerable();
            return result;
        }
    }
}
