using System;

namespace OpenPlatform_Authorization
{
    public class AccessToken
    {
        /// <summary>
        /// 通过授权拿到的Code换取AccessToken
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static async Task<string> ExchangeAccessTokenAsync(string clientId, string clientSecret, string code)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code)
            });
            if (OpenPlatform_Signature.Signature.Printf)
            {
                Console.WriteLine($"请求地址：{OpenPlatform_Signature.Signature.ApiDomain}/x/account-oauth2/v1/token");
                Console.WriteLine($"请求类型：Post");
            }
            var response = await client.PostAsync($"{OpenPlatform_Signature.Signature.ApiDomain}/x/account-oauth2/v1/token", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}