using System.Buffers.Text;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using SaberTools.CrashReporter;

namespace SslDumpViewer
{
    public partial class Form1 : Form
    {
        string currentDumpPath = "";
        SslDmp currentSslDmp = null;
        ErrorYaml currentErrorYaml = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            var sslDmpPath = "";
            var errorYamlPath = "";
            if (args.Length > 1)
            {
                var dumpInfo = CrashDump.Build(args[1]);
                if (dumpInfo == null)
                {
                    sslDmpPath = args[1];
                    currentDumpPath = sslDmpPath.Substring(0, sslDmpPath.Length - Path.GetFileName(sslDmpPath).Length);
                    errorYamlPath = Path.Join(currentDumpPath, "error.yaml");
                }
                else
                {
                    var sslDmp = Directory.GetFiles(dumpInfo.leech_arguments.dump_path, "*.ssldmp");
                    if (sslDmp.Length > 0)
                    {
                        sslDmpPath = sslDmp[0];
                    }
                    currentDumpPath = dumpInfo.leech_arguments.dump_path;
                    errorYamlPath = Path.Join(dumpInfo.leech_arguments.dump_path, "error.yaml");
                }
            }
            LoadCrashDump(sslDmpPath);
            LoadErrorYaml(errorYamlPath);
        }

        private void LoadErrorYaml(string path)
        {
            if (!File.Exists(path))
            {
                outputLog.Clear();
                return;
            }

            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(file);
                var content = reader.ReadToEnd();
                currentErrorYaml = JsonConvert.DeserializeObject<ErrorYaml>(content); //Hey, as long as it works
            }

            ReloadError();
        }

        private void LoadCrashDump(string path)
        {
            if (!File.Exists(path))
            {
                callstackTree.BeginUpdate();
                callstackTree.Nodes.Clear();
                callstackTree.EndUpdate();

                argumentsTree.BeginUpdate();
                argumentsTree.Nodes.Clear();
                argumentsTree.EndUpdate();

                memoryTree.BeginUpdate();
                memoryTree.Nodes.Clear();
                memoryTree.EndUpdate();
                return;
            }

            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(file);
                var content = reader.ReadToEnd();
                currentSslDmp = JsonConvert.DeserializeObject<SslDmp>(content);
            }
            ReloadDump();
        }

        private void ReloadError()
        {
            outputLog.Clear();
            outputLog.Text += $"Dump Path: {currentDumpPath}\r\n";
            outputLog.Text += $"Executable: {currentErrorYaml.executable}\r\n";
            outputLog.Text += $"Message: {currentErrorYaml.message.Replace("\n", "\r\n")}\r\n";
        }

        private void ReloadDump()
        {
            BuildCallstack();
            if (callstackTree.Nodes.Count > 0)
            {
                callstackTree.SelectedNode = callstackTree.Nodes[0];
            }
        }

        private void BuildCallstack()
        {
            callstackTree.BeginUpdate();
            callstackTree.Nodes.Clear();
            foreach (var callstack in currentSslDmp.callstack)
            {
                TreeNode node = new TreeNode($"{callstack.funcType} {callstack._class}.{callstack.func}");
                node.Tag = callstack;

                callstackTree.Nodes.Add(node);
            }
            callstackTree.EndUpdate();
        }

        private void callstackTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = (TreeNode)e.Node;
            var callstack = (SslCallStackItem)node.Tag;
            BuildArguments(callstack);
            BuildMemory(callstack);
        }

        private void BuildArguments(SslCallStackItem callStackItem)
        {
            argumentsTree.BeginUpdate();
            argumentsTree.Nodes.Clear();

            foreach (var arg in callStackItem.args)
            {
                TreeNode node = new TreeNode();
                node.Tag = arg.value;
                if (arg.value is SslValueString)
                {
                    var sslVal = (SslValueString)arg.value;
                    node.Text = $"{arg.name} = \"{sslVal.value}\"";
                }
                else
                {
                    node.Text = $"{arg.name}";
                }
                argumentsTree.Nodes.Add(node);
            }

            argumentsTree.EndUpdate();
        }

        private void BuildTree(TreeNode parent, SslValueAbstract value)
        {
            if (parent.Nodes.Count != 0)
            {
                return;
            }
            if (value is SslValueString)
            {
                return;
            }
            if (value is SslValuePtr)
            {
                var sslPtr = (SslValuePtr)value;
                if (!currentSslDmp.objects.ContainsKey(sslPtr.id)) return;
                var fields = currentSslDmp.objects[sslPtr.id];
                foreach (var key in fields.Keys)
                {
                    TreeNode node = new TreeNode();
                    var fieldValue = fields[key];
                    node.Tag = fieldValue;
                    if (fieldValue is SslValueString)
                    {
                        var sslVal = ((SslValueString)fieldValue).value;
                        node.Text = $"{key} = \"{sslVal}\"";
                    }
                    else
                    {
                        node.Text = $"{key}";
                    }

                    parent.Nodes.Add(node);
                }

                return;
            }
            if (value is SslValueArray)
            {
                var sslArray = (SslValueArray)value;
                var idx = 0;
                foreach (var item in sslArray.array)
                {
                    TreeNode node = new TreeNode();
                    node.Tag = item;
                    if (item is SslValueString)
                    {
                        var sslVal = (SslValueString)item;
                        node.Text = $"{idx} = \"{sslVal.value}\"";
                    }
                    else
                    {
                        node.Text = $"{idx}";
                    }

                    parent.Nodes.Add(node);
                    ++idx;
                }
                return;
            }
            if (value is SslValueDictionary)
            {
                var sslDict = (SslValueDictionary)value;
                foreach (var key in sslDict.dict.Keys)
                {
                    TreeNode node = new TreeNode();
                    var fieldValue = sslDict.dict[key];
                    node.Tag = fieldValue;
                    if (fieldValue is SslValueString)
                    {
                        var sslVal = ((SslValueString)fieldValue).value;
                        node.Text = $"{key} = \"{sslVal}\"";
                    }
                    else
                    {
                        node.Text = $"{key}";
                    }

                    parent.Nodes.Add(node);
                }
                return;
            }
        }
        private void BuildMemory(SslCallStackItem callStackItem)
        {
            memoryTree.BeginUpdate();
            memoryTree.Nodes.Clear();

            {
                TreeNode node = new TreeNode($"this {callStackItem._this.id}");
                var sslPtr = new SslValuePtr();
                sslPtr.id = callStackItem._this.id;
                node.Tag = sslPtr;

                memoryTree.Nodes.Add(node);
            }

            foreach (var uid in callStackItem.uids)
            {
                TreeNode node = new TreeNode($"{uid}");
                var sslPtr = new SslValuePtr();
                sslPtr.id = uid.Value;
                node.Tag = sslPtr;

                memoryTree.Nodes.Add(node);
            }

            memoryTree.EndUpdate();
        }

        private bool argumentsUpdating = false; //an old friend
        private void argumentsTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (argumentsUpdating)
            {
                return;
            }
            var node = e.Node;
            argumentsUpdating = true;
            ((TreeView)sender).BeginUpdate();
            BuildTree(node, (SslValueAbstract)node.Tag);
            ((TreeView)sender).EndUpdate();
            argumentsUpdating = false;
        }

        private bool memTreeUpdating = false; //an old friend
        private void memoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (memTreeUpdating)
            {
                return;
            }
            var node = e.Node;
            memTreeUpdating = true;
            ((TreeView)sender).BeginUpdate();
            BuildTree(node, (SslValueAbstract)node.Tag);
            ((TreeView)sender).EndUpdate();
            memTreeUpdating = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                currentDumpPath = folderBrowserDialog1.SelectedPath;
                Reload();
            }
        }

        private void Reload()
        {
            var sslDmp = Directory.GetFiles(currentDumpPath, "*.ssldmp");
            var sslDmpPath = "";
            var errorYamlPath = "";
            if (sslDmp.Length > 0)
            {
                sslDmpPath = sslDmp[0];
            }
            errorYamlPath = Path.Join(currentDumpPath, "error.yaml");
            LoadCrashDump(sslDmpPath);
            LoadErrorYaml(errorYamlPath);
        }
    }
}
