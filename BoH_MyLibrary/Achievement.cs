namespace BoH_MyLibrary
{
    public struct Achievement
    {
        private string _id;
        public string Id() => _id;

        private string _category;
        public string Category() => _category;

        private string _iconUnlocked;
        public string IconUnlocked() => _iconUnlocked;

        private bool _singleDescription;
        public bool SingleDescription() => _singleDescription;

        private bool _validateOnStorefront;
        public bool ValidateOnStoreFront() => _validateOnStorefront;

        private string _label;
        public string Label() => _label;

        private string _descriptionUnlocked;
        public string DescriptionUnlocked() => _descriptionUnlocked;

        public Achievement(JsonObject obj)
        {
            _id = obj.GetField("id");
            _category = obj.GetField("category");
            _iconUnlocked = obj.GetField("iconUnlocked");
            _label = obj.GetField("label");
            _descriptionUnlocked = obj.GetField("descriptionunlocked");
            _validateOnStorefront = obj.GetField("validateOnStorefront") == "true";
            _singleDescription = obj.GetField("singleDescription") == "true";
        }

        public string GetByField(Field field)
        {
            switch (field)
            {
                case Field.Id: return _id;
                case Field.Category: return _category;
                case Field.SingleDescription:
                    {
                        if (_singleDescription) return "true";
                        return "false";
                    }
                case Field.Label: return _label;
                case Field.DescriptionUnlocked: return _descriptionUnlocked;
                case Field.IconUnlocked: return _iconUnlocked;
                case Field.ValidateOnStoreFront:
                    {
                        if (_validateOnStorefront) return "true";
                        return "false";
                    }
                default: return "";
            }
        }
    }
}
