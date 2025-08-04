using System;
using System.Windows.Forms;

public class ConfirmCloseForm : Form
{
    public bool Proceed { get; private set; } = false;

    public ConfirmCloseForm(string message, string yesButtonText, string noButtonText)
    {
        InitializeComponentCustom(message, yesButtonText, noButtonText);
    }

    private void InitializeComponentCustom(string message, string yesBtnText, string noBtnText)
    {
        this.Text = "";
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ClientSize = new System.Drawing.Size(320, 140);

        Label lbl = new Label()
        {
            Text = message,
            AutoSize = false,
            Size = new System.Drawing.Size(300, 60),
            Location = new System.Drawing.Point(10, 10),
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
        };
        this.Controls.Add(lbl);

        Button btnYes = new Button()
        {
            Text = yesBtnText,
            DialogResult = DialogResult.OK,
            Size = new System.Drawing.Size(120, 30),
            Location = new System.Drawing.Point(50, 80),
        };
        btnYes.Click += (s, e) => { Proceed = true; this.Close(); };
        this.Controls.Add(btnYes);

        Button btnNo = new Button()
        {
            Text = noBtnText,
            DialogResult = DialogResult.Cancel,
            Size = new System.Drawing.Size(120, 30),
            Location = new System.Drawing.Point(170, 80),
        };
        btnNo.Click += (s, e) => { Proceed = false; this.Close(); };
        this.Controls.Add(btnNo);

        this.AcceptButton = btnYes;
        this.CancelButton = btnNo;
    }
}
