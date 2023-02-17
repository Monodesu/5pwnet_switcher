using Microsoft.Win32;
using System;
using System.Runtime.CompilerServices;

namespace _5pwnet_switcher
{
    public partial class Form1 : Form
    {
        public static Form1? form1;

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
            InsertLog("��������Ϣ��������");
            var x = await Class_Main.GetServerInfo();
            if (x == null)
            {
                InsertLog("�޷���������Ϣ��������");
                await Delay_Async(3000);
                Application.Exit();
            }
            InsertLog("����������Ϣ��������");
            if (!x!.ServerStatus)
            {
                InsertLog("��Ϸ������״̬���ر�");
                InsertLog("�޷���������Ϸ������");
                await Delay_Async(3000);
                Application.Exit();
            }
            if (x.SwitcherOnliveVersion != Class_Main.CurrentVersion)
            {
                InsertLog($"��������Ҫ���¡�{Class_Main.CurrentMajorVersion}.{Class_Main.CurrentVersion}->{Class_Main.CurrentMajorVersion}.{x.SwitcherOnliveVersion}");
                Clipboard.SetText(x.SwitcherDownloadUrl);
                InsertLog("�ѽ����ص�ַ���Ƶ�������");
                InsertLog("����������ĵ�ַ����ʹ��");
                InsertLog("Ctrl+V���ء�");
                InsertLog("10����Զ��˳���");
                await Delay_Async(10000);
                Application.Exit();
                return;
            }

            //Announcement
            int length = 20;
            if (x.Announcement != "")
            {
                InsertLog("���Է���������Ϣ��");
                string tmp = x.Announcement!;
                while (tmp.Length > length)
                {
                    InsertLog(tmp[..length], false);
                    tmp = tmp[length..];
                }
                InsertLog(tmp, false);
            }

            if (x.ServerUrl != "")
            {
                InsertLog("�ѳɹ���ȡ��������ַ");
            }
            else
            {
                InsertLog("��������ַ��ȡʧ�ܡ�");
                await Delay_Async(3000);
                Application.Exit();
            }

            // �Ӹ�Ŀ¼����osu!
            if (File.Exists(".\\osu!.exe"))
            {
                InsertLog("����ͨ����Ŀ¼�ļ�����osu!");
                System.Diagnostics.Process.Start(".\\osu!.exe", $"-devserver {x.ServerUrl}:{x.Port}");
                await Delay_Async(5000);
                Application.Exit();
            }

            // ��ȫ��ע�������osu!
            try
            {
                string? rkv = null;
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey(@"osu!\shell\open\command")!;
                rkv = rk.GetValue(null)!.ToString()![1..];
                rkv = rkv[..rkv.IndexOf("\"")].Trim();
                if (!File.Exists(rkv))
                {
                    InsertLog("δ�ҵ�osu!��");
                    MessageBox.Show("û����ϵͳ�����õ�osu!Ĭ��·�����ҵ�osu!�Ŀ�ִ���ļ��������°�װosu!���Թ���Ա�����������һ��osu!����ʹ�ô���������", "δ�ҵ�osu!·��", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.Exit();
                }
                InsertLog("����ͨ��Ĭ�ϳ�������osu!");
                System.Diagnostics.Process.Start(rkv, $"-devserver {x.ServerUrl}:{x.Port}");
                await Delay_Async(5000);
                Application.Exit();
            }
            catch
            {
                InsertLog("�����˴���");
                MessageBox.Show("δ�ҵ�<osu!>·�������Ƿ�װ��<osu!>��\n����Ѱ�װ��ʹ�ù���Ա�������һ��<osu!>��ʹ�ô���������", "δ�ҵ�osu!·��", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }

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
    }
}