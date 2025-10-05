using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace unidasmwrapper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.currentSelectCpu = null;
            this.UnidasmTask = null;
            this.UnidasmTaskCancel = null;
            this.config_filename = Path.Combine(Environment.CurrentDirectory, AppDomain.CurrentDomain.FriendlyName + ".config");
            this.config = new Config(config_filename);
            InitializeComponent();

            this.unidasm_stdout = new List<string>();
            this.unidasm_stderr = new List<string>();
            if (this.CheckUnidasm())
            {
                this.MakeSupportCPUList();
            }
        }

        private void ファイルToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private bool CheckUnidasm()
        {
            if (Path.Exists(this.config.unidasm_path))
            {
                this.ts_unidasmchecker.Text = $"unidasm:OK";
                return true;
            }
            else
            {
                this.ts_unidasmchecker.Text = $"unidasm:not found";
                return false;
            }
        }

        private async void MakeSupportCPUList()
        {
            // サポートCPUのリストを取得する（引数無しで実行したら、Usageと「Supported architectures:」の後ろにアーキテクチャ名がリストアップされるのでそれを拾う）

            this.UnidasmTaskCancel = new CancellationTokenSource();
            this.unidasm_stdout.Clear();
            this.unidasm_stderr.Clear();
            await Task.Run(() => { this.TaskUnidasm(new string[] { "" }, this.UnidasmTaskCancel.Token); });
            // 結果取得
            int stat = 0;
            List<string> archs = new List<string>();
            foreach (var s in this.unidasm_stdout)
            {
                switch (stat)
                {
                    case 0:
                        if (s.Contains("architectures:"))
                        {
                            stat = 1;
                        }
                        break;
                    case 1:
                        archs.AddRange(s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                        break;
                }

            }
            archs.Sort();
            // 先頭文字ごとに階層リストを作成する
            Dictionary<string, List<string>> archdic = new Dictionary<string, List<string>>();
            foreach (var arch in archs)
            {
                string _fs = arch.Substring(0, 1).ToUpper();
                char _first = _fs[0];
                if (_first >= '0' && _first <= '9')
                {
                    // 数字
                    if (archdic.Keys.Contains("0-9") == false)
                    {
                        archdic["0-9"] = new List<string>();
                    }
                    archdic["0-9"].Add(arch);
                }
                else if ((_first >= 'A' && _first <= 'Z') || (_first >= 'a' && _first <= 'z'))
                {
                    // 英字
                    if (archdic.Keys.Contains(_fs) == false)
                    {
                        archdic[_fs] = new List<string>();
                    }
                    archdic[_fs].Add(arch);
                }
                else
                {
                    // その他
                    if (archdic.Keys.Contains("その他") == false)
                    {
                        archdic["その他"] = new List<string>();
                    }
                    archdic["その他"].Add(arch);

                }
            }
            // メニューリストを作成する
            this.MI_CPUType.DropDownItems.Clear();
            foreach (var f in archdic.Keys)
            {
                var mi = new ToolStripMenuItem(f);
                archdic[f].Sort();
                foreach (var a in archdic[f])
                {
                    var c = new ToolStripMenuItem(a);
                    c.CheckState = CheckState.Checked;
                    c.Checked = false;
                    c.Click += (s, e) =>
                    {
                        var m = s as ToolStripMenuItem;
                        m.Checked = true;
                        if (this.currentSelectCpu != null)
                        {
                            // 前回値をoffに
                            this.currentSelectCpu.Checked = false;
                        }
                        this.currentSelectCpu = m;
                        this.ts_SelectedCPU.Text = $"CPU={m.Text}";
                    };
                    mi.DropDownItems.Add(c);
                }
                this.MI_CPUType.DropDownItems.Add(mi);
            }
            this.MI_CPUType.Enabled = true;
            this.currentSelectCpu = null;
        }


        private void menuit_SetUnidasm_Click(object sender, EventArgs e)
        {
            // unidasmの指定
            var ocd = new OpenFileDialog();
            ocd.Filter = "unidasm本体(unidasm.exe)|unidasm.exe|実行ファイル(*.exe)|*.exe";
            ocd.FilterIndex = 0;
            ocd.InitialDirectory = Path.GetDirectoryName(config.unidasm_path);
            ocd.Multiselect = false;
            ocd.CheckFileExists = true;
            ocd.CheckPathExists = true;
            ocd.Title = "unidasm.exeを指定してください";
            if (ocd.ShowDialog() == DialogResult.OK)
            {
                this.config.unidasm_path = ocd.FileName;
            }
            if (this.CheckUnidasm())
            {
                this.MakeSupportCPUList();
            }
        }

        private string config_filename;
        public Config config;
        private ToolStripMenuItem? currentSelectCpu;

        delegate void ResultUnidasmFunc(string[] result_stdout);

        private Task? UnidasmTask;
        private CancellationTokenSource? UnidasmTaskCancel;
        private List<string> unidasm_stdout;
        private List<string> unidasm_stderr;

        private void TaskUnidasm(string[] args, CancellationToken _cancel)
        {
            Process unidproc = new Process();
            unidproc.StartInfo.FileName = config.unidasm_path;
            // 空白が含まれていたら両端にダブルクオーテーションを付ける
            string[] t_args = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains(" "))
                {
                    t_args[i] = $"\"{args[i]}\"";
                }
                else
                {
                    t_args[i] = args[i];
                }
            }
            unidproc.StartInfo.Arguments = string.Join(" ", t_args);
            // 標準出力標準エラー出力を非同期イベントで受け取るためにshellexecuteをfalseにしてredirectしてイベントを登録
            unidproc.StartInfo.UseShellExecute = false;
            unidproc.StartInfo.CreateNoWindow = true;
            unidproc.StartInfo.RedirectStandardOutput = true;
            // プロセス開始
            unidproc.Start();
            foreach (var s in unidproc.StandardOutput.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                this.unidasm_stdout.Add(s);
            }
            unidproc.Close();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // configの保存
            this.config.SaveConfig(this.config_filename);
        }

        private void MI_Exit_Click(object sender, EventArgs e)
        {
            // アプリの終了
            this.config.SaveConfig(this.config.config_filename);
            this.Close();
        }

        private void MI_Unite8x2_Click(object sender, EventArgs e)
        {
            // ファイル連結ユーティリティ(8bitx2)
            var frm = new UniteFile();
            frm.ShowDialog();
        }

        private void MI_OpenBinary_Click(object sender, EventArgs e)
        {
            // バイナリファイルを開く
            var ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Title = "逆アセンブルするファイルを指定してください";
            ofd.Multiselect = false;
            ofd.Filter = "全てのファイル(*.*)|*.*";
            ofd.FilterIndex = 0;
            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                var tp = new TabPage(Path.GetFileName(ofd.FileName));
                this.tab_Main.TabPages.Add(tp);
                this.tab_Main.SelectedTab = tp;
            }
        }
    }

    public class Config
    {
        public Config()
        {
            // デフォルト
            this.unidasm_path = "";
            this.config_filename = "";
        }

        public string config_filename;

        public Config(string config_filepath) : this()
        {
            this.IsValid = false;
            this.config_filename = config_filepath;
            this.LoadConfig(config_filepath);
        }

        public bool LoadConfig(string config_filepath)
        {
            try
            {
                using (var rd = new StreamReader(config_filepath))
                {
                    var lines = rd.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    rd.Close();
                    foreach (var line in lines)
                    {
                        var l = line.Trim();
                        if (string.IsNullOrEmpty(l) || l[0] == '#') continue;
                        var kv = l.Split(new char[] { '=' }, 2);
                        if (kv.Length != 2) continue;
                        switch (kv[0])
                        {
                            case Config.key_unidasm_path:
                                this.unidasm_path = kv[1];
                                break;
                        }
                    }
                }
                // 妥当性チェック
                if (Path.Exists(this.unidasm_path) == false)
                {
                    this.unidasm_path = "";
                    return false;
                }
                this.IsValid = true;
            }
            catch (IOException e)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool SaveConfig(string config_filepath)
        {
            try
            {
                using (var wr = new StreamWriter(config_filepath))
                {
                    wr.WriteLine($"{Config.key_unidasm_path}={this.unidasm_path}");
                    wr.Close();
                }
            }
            catch (IOException e)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool IsValid;

        public const string key_unidasm_path = "UnidasmPath";
        public string unidasm_path;
    }
}
