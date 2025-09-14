namespace SslDumpViewer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            callstackTree = new TreeView();
            label1 = new Label();
            argumentsTree = new TreeView();
            label2 = new Label();
            label3 = new Label();
            memoryTree = new TreeView();
            outputLog = new TextBox();
            button1 = new Button();
            folderBrowserDialog1 = new FolderBrowserDialog();
            SuspendLayout();
            // 
            // callstackTree
            // 
            callstackTree.Location = new Point(12, 32);
            callstackTree.Name = "callstackTree";
            callstackTree.Size = new Size(262, 432);
            callstackTree.TabIndex = 0;
            callstackTree.AfterSelect += callstackTree_AfterSelect;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(55, 15);
            label1.TabIndex = 1;
            label1.Text = "CallStack";
            // 
            // argumentsTree
            // 
            argumentsTree.Location = new Point(280, 32);
            argumentsTree.Name = "argumentsTree";
            argumentsTree.Size = new Size(526, 206);
            argumentsTree.TabIndex = 2;
            argumentsTree.AfterSelect += argumentsTree_AfterSelect;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(280, 14);
            label2.Name = "label2";
            label2.Size = new Size(66, 15);
            label2.TabIndex = 3;
            label2.Text = "Arguments";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(280, 241);
            label3.Name = "label3";
            label3.Size = new Size(77, 15);
            label3.TabIndex = 4;
            label3.Text = "MemoryView";
            // 
            // memoryTree
            // 
            memoryTree.Location = new Point(280, 259);
            memoryTree.Name = "memoryTree";
            memoryTree.Size = new Size(526, 205);
            memoryTree.TabIndex = 5;
            memoryTree.AfterSelect += memoryTree_AfterSelect;
            // 
            // outputLog
            // 
            outputLog.Location = new Point(12, 470);
            outputLog.Multiline = true;
            outputLog.Name = "outputLog";
            outputLog.ReadOnly = true;
            outputLog.ScrollBars = ScrollBars.Vertical;
            outputLog.Size = new Size(713, 104);
            outputLog.TabIndex = 6;
            // 
            // button1
            // 
            button1.Location = new Point(731, 470);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 7;
            button1.Text = "Open...";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(818, 586);
            Controls.Add(button1);
            Controls.Add(outputLog);
            Controls.Add(memoryTree);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(argumentsTree);
            Controls.Add(label1);
            Controls.Add(callstackTree);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Form1";
            Text = "Ssl Dump Viewer";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TreeView callstackTree;
        private Label label1;
        private TreeView argumentsTree;
        private Label label2;
        private Label label3;
        private TreeView memoryTree;
        private TextBox outputLog;
        private Button button1;
        private FolderBrowserDialog folderBrowserDialog1;
    }
}
