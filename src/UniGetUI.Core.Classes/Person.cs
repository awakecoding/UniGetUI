namespace UniGetUI.Core.Classes
{
    public readonly struct Person
    {
        // Properties for Avalonia compiled binding support
        public string Name { get; init; }
        public Uri? ProfilePicture { get; init; }
        public Uri? GitHubUrl { get; init; }
        public bool HasPicture { get; init; }
        public bool HasGitHubProfile { get; init; }
        public string Language { get; init; }

        public Person(string Name, Uri? ProfilePicture = null, Uri? GitHubUrl = null, string Language = "")
        {
            this.Name = Name;
            this.ProfilePicture = ProfilePicture;
            this.GitHubUrl = GitHubUrl;
            HasPicture = ProfilePicture is not null;
            HasGitHubProfile = GitHubUrl is not null;
            this.Language = Language;
        }
    }
}
