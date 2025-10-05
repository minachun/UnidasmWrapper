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

        private void �t�@�C��ToolStripMenuItem_Click(object sender, EventArgs e)
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
            // �T�|�[�gCPU�̃��X�g���擾����i���������Ŏ��s������AUsage�ƁuSupported architectures:�v�̌��ɃA�[�L�e�N�`���������X�g�A�b�v�����̂ł�����E���j

            this.UnidasmTaskCancel = new CancellationTokenSource();
            this.unidasm_stdout.Clear();
            this.unidasm_stderr.Clear();
            await Task.Run(() => { this.TaskUnidasm(new string[] { "" }, this.UnidasmTaskCancel.Token); });
            // ���ʎ擾
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
            // �擪�������ƂɊK�w���X�g���쐬����
            Dictionary<string, List<string>> archdic = new Dictionary<string, List<string>>();
            foreach (var arch in archs)
            {
                string _fs = arch.Substring(0, 1).ToUpper();
                char _first = _fs[0];
                if (_first >= '0' && _first <= '9')
                {
                    // ����
                    if (archdic.Keys.Contains("0-9") == false)
                    {
                        archdic["0-9"] = new List<string>();
                    }
                    archdic["0-9"].Add(arch);
                }
                else if ((_first >= 'A' && _first <= 'Z') || (_first >= 'a' && _first <= 'z'))
                {
                    // �p��
                    if (archdic.Keys.Contains(_fs) == false)
                    {
                        archdic[_fs] = new List<string>();
                    }
                    archdic[_fs].Add(arch);
                }
                else
                {
                    // ���̑�
                    if (archdic.Keys.Contains("���̑�") == false)
                    {
                        archdic["���̑�"] = new List<string>();
                    }
                    archdic["���̑�"].Add(arch);

                }
            }
            // ���j���[���X�g���쐬����
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
                            // �O��l��off��
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
            // unidasm�̎w��
            var ocd = new OpenFileDialog();
            ocd.Filter = "unidasm�{��(unidasm.exe)|unidasm.exe|���s�t�@�C��(*.exe)|*.exe";
            ocd.FilterIndex = 0;
            ocd.InitialDirectory = Path.GetDirectoryName(config.unidasm_path);
            ocd.Multiselect = false;
            ocd.CheckFileExists = true;
            ocd.CheckPathExists = true;
            ocd.Title = "unidasm.exe���w�肵�Ă�������";
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
            // �󔒂��܂܂�Ă����痼�[�Ƀ_�u���N�I�[�e�[�V������t����
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
            // �W���o�͕W���G���[�o�͂�񓯊��C�x���g�Ŏ󂯎�邽�߂�shellexecute��false�ɂ���redirect���ăC�x���g��o�^
            unidproc.StartInfo.UseShellExecute = false;
            unidproc.StartInfo.CreateNoWindow = true;
            unidproc.StartInfo.RedirectStandardOutput = true;
            // �v���Z�X�J�n
            unidproc.Start();
            foreach (var s in unidproc.StandardOutput.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                this.unidasm_stdout.Add(s);
            }
            unidproc.Close();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // config�̕ۑ�
            this.config.SaveConfig(this.config_filename);
        }

        private void MI_Exit_Click(object sender, EventArgs e)
        {
            // �A�v���̏I��
            this.config.SaveConfig(this.config.config_filename);
            this.Close();
        }

        private void MI_Unite8x2_Click(object sender, EventArgs e)
        {
            // �t�@�C���A�����[�e�B���e�B(8bitx2)
            var frm = new UniteFile();
            frm.ShowDialog();
        }

        private void MI_OpenBinary_Click(object sender, EventArgs e)
        {
            // �o�C�i���t�@�C�����J��
            var ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Title = "�t�A�Z���u������t�@�C�����w�肵�Ă�������";
            ofd.Multiselect = false;
            ofd.Filter = "�S�Ẵt�@�C��(*.*)|*.*";
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
            // �f�t�H���g
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
                // �Ó����`�F�b�N
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
