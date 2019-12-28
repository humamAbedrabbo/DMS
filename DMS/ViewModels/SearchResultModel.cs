namespace DAS.ViewModels
{
    public class SearchResultModel
    {
        public string TypeName { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon
        {
            get
            {
                switch(TypeName)
                {
                    case "Repository":
                        return "oi oi-box mx-1";
                    case "Folder":
                        return "oi oi-folder mx-1";
                    default:
                        return "oi oi-file mx-1";
                }
            }
        }
        public string Url
        {
            get
            {
                switch (TypeName)
                {
                    case "Repository":
                        return $"repo/{Id}";
                    case "Folder":
                        return $"folders/{Id}";
                    default:
                        return $"doc/details/{Id}";
                }
            }
        }
    }
}
