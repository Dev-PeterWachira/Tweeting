namespace Contracts.V1
{

public static class ApiRoutes
{
    public const string Root = "api";

    public const string Version = "V1";

    public const string Base = "api/V1";

    public static class Posts
    {
        public const string GetAll = Base + "/posts";

        public const string Get = Base + "/posts/{postId}";

        public const string Update = Base + "/posts/{postId}";

        public const string Delete = Base + "/posts/{postId}";

        public const string Create = Base + "/posts";
    }

    public static class Identity
        {
            public const string Register = Base + "/identity/register";

            public const string Login = Base + "/identity/Login";

            public const string Refresh = Base + "/identity/Refresh";
        }

        public class Tags
        {
            public const string GetAll = "tags";

            public const string Get = "tags/{tagName}";

            public const string Create = "tags";

            public const string Update = "tags/{tagName}";

            public const string Delete = "tags/{tagName}";
        }
}

}