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
    }
}

}