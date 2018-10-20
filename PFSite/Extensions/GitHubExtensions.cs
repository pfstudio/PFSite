namespace Microsoft.Extensions.DependencyInjection
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.OAuth;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using PFSite.OAuth.GitHub;
    using System;
    using System.Threading.Tasks;

    public static class GitHubExtensions
    {
        private static DateTimeOffset? Datetime;

        public static AuthenticationBuilder AddGitHub(this AuthenticationBuilder builder,
            string clientId, string clientSecret, string callbackPath = "/signin-github")
        {
            builder.AddOAuth(GitHubAuthenticationDefaults.AuthenticationScheme, options =>
            {
                // 配置OAuth
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.CallbackPath = new PathString(callbackPath);

                // GitHub Endpoints
                options.AuthorizationEndpoint = GitHubAuthenticationDefaults.AuthorizationEndpoint;
                options.TokenEndpoint = GitHubAuthenticationDefaults.TokenEndpoint;
                options.UserInformationEndpoint = GitHubAuthenticationDefaults.UserInformationEndpoint;

                // 配置claim映射
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey(GitHubClaimTypes.Id, "id");
                options.ClaimActions.MapJsonKey(GitHubClaimTypes.Login, "login");
                options.ClaimActions.MapJsonKey(GitHubClaimTypes.Url, "html_url");
                options.ClaimActions.MapJsonKey(GitHubClaimTypes.Avatar, "avatar_url");

                // 添加创建事件
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                        context.RunClaimActions(user);
                    },
                    OnTicketReceived = context =>
                    {
                        context.Properties.IsPersistent = true;
                        context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);

                        return Task.CompletedTask;
                    }
                };
            });

            return builder;
        }
    }
}

namespace PFSite.OAuth.GitHub
{
    using System.Security.Claims;

    public static class GitHubAuthenticationDefaults
    {
        public const string AuthenticationScheme = "GitHub";

        public static readonly string AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        public static readonly string TokenEndpoint = "https://github.com/login/oauth/access_token";
        public static readonly string UserInformationEndpoint = "https://api.github.com/user";
    }

    public static class GitHubClaimTypes
    {
        public const string Id = "github:id";
        public const string Login = "github:login";
        public const string Url = "github:url";
        public const string Avatar = "github:avatar";
    }

    public static class GitHubExtensions
    {
        public static string GetGitHubId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(GitHubClaimTypes.Id);
        }
    }
}
