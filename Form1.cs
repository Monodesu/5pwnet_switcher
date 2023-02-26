using Microsoft.Win32;
using System;
using System.Runtime.CompilerServices;

namespace _5pwnet_switcher
{
    public partial class Form1 : Form
    {
        public static Form1? form1;

        private static Class_Main.ServerInfo serverInfo;

        public Form1()
        {
            InitializeComponent();
            form1 = this;
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            this.Shown += new EventHandler(WhenLoaded!);
        }

        public async void WhenLoaded(Object sender, EventArgs e)
        {
            //MessageBox.Show("Loaded");
            await Init();

        }

        async public static Task Init()
        {
            InsertLog($"5pw.net switcher ver{Class_Main.CurrentMajorVersion}.{Class_Main.CurrentVersion}", false);
            InsertLog("尝试至信息服务器。");
            serverInfo = await Class_Main.GetServerInfo();
            if (serverInfo == null)
            {
                InsertLog("无法连接至信息服务器。");
                await Delay_Async(3000);
                Application.Exit();
            }
            InsertLog("已连接至信息服务器。");
            if (!serverInfo!.ServerStatus)
            {
                InsertLog("游戏服务器状态：关闭");
                InsertLog("无法连接至游戏服务器");
                await Delay_Async(3000);
                Application.Exit();
            }
            if (serverInfo.SwitcherOnliveVersion != Class_Main.CurrentVersion)
            {
                InsertLog($"启动器需要更新。{Class_Main.CurrentMajorVersion}.{Class_Main.CurrentVersion}->{Class_Main.CurrentMajorVersion}.{serverInfo.SwitcherOnliveVersion}");
                Clipboard.SetText(serverInfo.SwitcherDownloadUrl);
                InsertLog("已将下载地址复制到剪贴板");
                InsertLog("请在浏览器的地址栏中使用");
                InsertLog("Ctrl+V下载。");
                InsertLog("10秒后自动退出。");
                await Delay_Async(10000);
                Application.Exit();
                return;
            }

            //Announcement
            int length = 20;
            if (serverInfo.Announcement != "")
            {
                InsertLog("来自服务器的消息：");
                string tmp = serverInfo.Announcement!;
                while (tmp.Length > length)
                {
                    InsertLog(tmp[..length], false);
                    tmp = tmp[length..];
                }
                InsertLog(tmp, false);
            }

            if (serverInfo.ServerInfos != null)
            {
                InsertLog("已成功获取服务器地址");
            }
            else
            {
                InsertLog("服务器地址获取失败。");
                await Delay_Async(3000);
                Application.Exit();
            }

            InsertLog("请选择要连接的线路：", false);

            foreach(var x in serverInfo.ServerInfos!)
            {
                InsertLog(x.Note!, false);
            }
            form1!.listBox1.Enabled = true;
        }

        public static void InsertLog(string log, bool addTimesnap = true)
        {
            if (addTimesnap)
                form1!.listBox1.Items.Add($"[{DateTime.Now.ToString("H:m:s")}]{log}");
            else
                form1!.listBox1.Items.Add(log);
            SortList();
        }

        private static void SortList()
        {
            form1!.listBox1.TopIndex = form1.listBox1.Items.Count - (form1.listBox1.Height / form1.listBox1.ItemHeight);
        }

        async static private Task Delay_Async(int time)
        {
            await Task.Delay(time);
        }

        private async void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                return;
            }

            var serverurl = "";
            var serverport = 0;
            var is_server_selected = false;

            foreach(var x in serverInfo.ServerInfos!)
            {
                if (listBox1.SelectedItem.ToString() == x.Note)
                {
                    listBox1.Enabled = false;
                    serverurl = x.Url;
                    serverport = x.Port;
                    is_server_selected = true;
                    InsertLog($"已选定服务器 {x.Note}");
                    break;
                }
            }

            if (!is_server_selected) return;

            // 从根目录启动osu!
            if (File.Exists(".\\osu!.exe"))
            {
                InsertLog("正在通过根目录文件启动osu!");
                System.Diagnostics.Process.Start(".\\osu!.exe", $"-devserver {serverurl}:{serverport}");
                await Delay_Async(5000);
                Application.Exit();
            }

            // 从全局注册表启动osu!
            try
            {
                string? rkv = null;
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey(@"osu!\shell\open\command")!;
                rkv = rk.GetValue(null)!.ToString()![1..];
                rkv = rkv[..rkv.IndexOf("\"")].Trim();
                if (!File.Exists(rkv))
                {
                    InsertLog("未找到osu!。");
                    MessageBox.Show("没有在系统内配置的osu!默认路径下找到osu!的可执行文件，请重新安装osu!或以管理员身份重新运行一次osu!后再使用此启动器！", "未找到osu!路径", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.Exit();
                }
                InsertLog("正在通过默认程序启动osu!");
                System.Diagnostics.Process.Start(rkv, $"-devserver {serverurl}:{serverport}");
                await Delay_Async(5000);
                Application.Exit();
            }
            catch
            {
                InsertLog("发生了错误。");
                MessageBox.Show("未找到<osu!>路径，您是否安装了<osu!>？\n如果已安装请使用管理员身份运行一次<osu!>再使用此启动器！", "未找到osu!路径", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        }
    }
}