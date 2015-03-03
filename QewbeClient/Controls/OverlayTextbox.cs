using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QewbeClient.Controls
{
    public class OverlayTextbox : TextBox
    {
        private bool hasModified;

        private string defaultText;
        [Category(@"Appearance")]
        public string DefaultText
        {
            get { return defaultText; }
            set
            {
                defaultText = value;
                if (!hasModified)
                    Text = DefaultText;
            }
        }

        [Category(@"Font")]
        public Color OverlayForeColour { get; set; }

        private Color defaultForeColour;
        public override Color ForeColor
        {
            get { return defaultForeColour; }
            set
            {
                defaultForeColour = value;
                if (hasModified)
                    setForeColour(defaultForeColour);
            }
        }

        [Category("Appearance")]
        public bool IsPasswordBox { get; set; }

        public OverlayTextbox() { }

        public OverlayTextbox(string defaultText)
        {
            DefaultText = defaultText;
            defaultForeColour = ForeColor;
        }

        private void setForeColour(Color colour)
        {
            base.ForeColor = colour;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (Text == DefaultText && !hasModified)
            {
                Text = string.Empty;
                setForeColour(ForeColor);
                if (IsPasswordBox)
                    base.PasswordChar = '*';
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (string.IsNullOrEmpty(Text))
            {
                Text = DefaultText;
                base.PasswordChar = '\0';
                setForeColour(OverlayForeColour);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            hasModified = !string.IsNullOrEmpty(Text);
        }
    }
}
