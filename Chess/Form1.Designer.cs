namespace Chess
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
            // 
            // Form1
            // 
            t = new();
            try
            {
                t.Text = Directory.GetCurrentDirectory();
            }
            catch (Exception e) 
            {
                t.Text = e.Message;
            }
            t.ReadOnly = true;
            t.Size = new Size(400, 300);
            t.Location = new Point(0);

            board = new();
            save = new();
            save.Text = "Save current game";
            save.Size = new Size(125, 125);
            save.Location = new(0, 100);
            save.MouseClick += board.SaveGame;
            load = new();
            load.Text = "Load previous game";
            load.Size = new Size(125, 125);
            load.Location = new(150, 100);
            load.MouseClick += board.LoadGame;

            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1920, 1080);
            Controls.Add(board);
            Controls.Add(save);
            Controls.Add(load);
            Controls.Add(t);
            Name = "Chess";
            Text = "Chess";
            Load += Form1_Load;
        }

        #endregion

        private Board board;
        private Button save;
        private Button load;
        private TextBox t;
    }
}
