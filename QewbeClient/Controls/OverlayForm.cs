using QewbeClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QewbeClient
{
    public partial class OverlayForm : Form
    {
        public OverlayForm()
        {
            InitializeComponent();
        }

        private const int HK_CAPTUREDRAG = 0xBFFE;
        private const int HK_UPLOADFILE = 0xBFFD;
        private const int HK_CAPTURESCREEN = 0xBFFC;
        private const int HK_CAPTUREWINDOW = 0xBFFB;

        private bool mouseDown;
        private Point dragStartPoint;
        private Rectangle dragRectangle;

        private void OverlayForm_Load(object sender, EventArgs e)
        {
            registerHotkey(Keys.Control | Keys.PrintScreen, HK_CAPTUREDRAG);
            registerHotkey(Keys.Control | Keys.Shift | Keys.U, HK_UPLOADFILE);
            registerHotkey(Keys.PrintScreen, HK_CAPTURESCREEN);
            registerHotkey(Keys.Control | Keys.Shift | Keys.Alt | Keys.W, HK_CAPTUREWINDOW);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Native.WM_HOTKEY)
            {
                switch (m.WParam.ToInt32())
                {
                    case HK_CAPTUREDRAG:
                        {
                            dragStartPoint = Point.Empty;
                            dragRectangle = SystemInformation.VirtualScreen;
                            Opacity = 0.1f;
                            Cursor = Cursors.Cross;
                            WindowState = FormWindowState.Normal;
                            TopMost = true;
                            Location = new Point(dragRectangle.X, dragRectangle.Y);
                            Size = new Size(dragRectangle.Width, dragRectangle.Height);
                            Invalidate(dragRectangle);
                        }
                        break;
                    case HK_CAPTURESCREEN:
                            captureArea(SystemInformation.VirtualScreen);
                        break;
                    case HK_UPLOADFILE:
                        {
                            using (OpenFileDialog ofd = new OpenFileDialog())
                            {
                                if (ofd.ShowDialog() == DialogResult.OK)
                                    Qewbe.UploadQueue.Add(ofd.FileName);
                            }
                        }
                        break;
                    case HK_CAPTUREWINDOW:
                        {
                            Native.Rect rect = new Native.Rect();
                            Native.GetWindowRect(Native.GetForegroundWindow(), ref rect);
                            captureArea(Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom));
                        }
                        break;
                }
            }
            base.WndProc(ref m);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                Opacity = 0;
                WindowState = FormWindowState.Minimized;
                Cursor = Cursors.Default;
                mouseDown = false;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                dragStartPoint = Cursor.Position;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (mouseDown)
            {
                Rectangle lastDragRectangle = dragRectangle;
                dragRectangle = Rectangle.FromLTRB(Math.Min(Cursor.Position.X, dragStartPoint.X), Math.Min(Cursor.Position.Y, dragStartPoint.Y),
                                                   Math.Max(Cursor.Position.X, dragStartPoint.X), Math.Max(Cursor.Position.Y, dragStartPoint.Y));
                this.Invalidate(lastDragRectangle);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mouseDown = false;

            captureArea(dragRectangle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Orange);
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
            if (dragRectangle != SystemInformation.VirtualScreen)
                e.Graphics.FillRectangle(Brushes.Magenta, dragRectangle);
        }

        private void captureArea(Rectangle area)
        {
            using (Bitmap b = new Bitmap(dragRectangle.Width, dragRectangle.Height))
            using (Graphics g = Graphics.FromImage(b))
            {
                g.CopyFromScreen(dragRectangle.X, dragRectangle.Y, 0, 0, dragRectangle.Size);
                Qewbe.UploadQueue.Add(b);
            }
        }

        private void registerHotkey(Keys key, int id)
        {
            uint modifierKeys = Native.MK_NONE;

            if ((key & Keys.Alt) == Keys.Alt)
                modifierKeys |= Native.MK_ALT;
            if ((key & Keys.Shift) == Keys.Shift)
                modifierKeys |= Native.MK_SHIFT;
            if ((key & Keys.Control) == Keys.Control)
                modifierKeys |= Native.MK_CTRL;

            key &= ~Keys.Control & ~Keys.Shift & ~Keys.Alt;
            Native.RegisterHotKey(this.Handle, id, (uint)modifierKeys, (uint)key);
        }
    }
}
