using Flurl.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _5pwnet_switcher
{
    public partial class Class_Main
    {
        public readonly static int CurrentMajorVersion = 1;
        public readonly static int CurrentVersion = 2;
        private readonly static string BaseUrl = "https://desu.life/5pw.net";

        static IFlurlRequest http()
        {
            return BaseUrl.AllowHttpStatus(HttpStatusCode.NotFound).AllowHttpStatus(HttpStatusCode.BadGateway);
        }


        public class ServerInfo
        {
            public string? ServerUrl { get; set; }
            public int Port { get; set; }
            public string? Announcement { get; set; }
            public int SwitcherOnliveVersion { get; set; }
            public string? SwitcherDownloadUrl { get; set; }
            public bool ServerStatus { get; set; }
        }

        async public static Task<ServerInfo> GetServerInfo()
        {
            var res = await http().GetAsync();

            if (res.StatusCode != 200)
            {
                //返回服务器已关闭
                return null!;
            }

            try
            {
                return await res.GetJsonAsync<ServerInfo>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"解析服务器消息时发生了错误。\n\n错误信息：{ex.Message}\n\n\nStackTrace:{ex.StackTrace}");
                Application.Exit();
                return null!;
            }
        }



















    }
}
