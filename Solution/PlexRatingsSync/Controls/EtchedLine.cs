using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace DS.Controls
{
  /// <summary>
  /// Summary description for UserControl1.
  /// </summary>
  public class EtchedLine : UserControl
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public EtchedLine()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      // Avoid receiving the focus.
      SetStyle(ControlStyles.Selectable, false);
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
          components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      // 
      // EtchedLine
      // 
      this.Name = "EtchedLine";
    }
    #endregion

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      SizeF textSize = new SizeF();

      if (!string.IsNullOrWhiteSpace(_text))
      {
        Font font = this.Font;

        textSize = e.Graphics.MeasureString(_text, font);

        float x = 0.0F;
        float y = this.Height / 2 - (textSize.Height / 2) - 1;
        SolidBrush drawBrush = new SolidBrush(Color.Black);
        StringFormat drawFormat = new StringFormat();

        e.Graphics.DrawString(_text, this.Font, drawBrush, x, y, drawFormat);

        drawFormat.Dispose();
        drawBrush.Dispose();
      }

      Brush lightBrush = new SolidBrush(_lightColor);
      Brush darkBrush = new SolidBrush(_darkColor);
      Pen lightPen = new Pen(lightBrush, 1);
      Pen darkPen = new Pen(darkBrush, 1);

      if (this.Edge == EtchEdge.Top)
      {
        e.Graphics.DrawLine(darkPen, 0, 0, this.Width, 0);
        e.Graphics.DrawLine(lightPen, 0, 1, this.Width, 1);
      }
      else if (this.Edge == EtchEdge.Bottom)
      {
        e.Graphics.DrawLine(darkPen, 0, this.Height - 2,
          this.Width, this.Height - 2);
        e.Graphics.DrawLine(lightPen, 0, this.Height - 1,
          this.Width, this.Height - 1);
      }
      else if (this.Edge == EtchEdge.Center)
      {
        e.Graphics.DrawLine(darkPen, textSize.Width, (this.Height / 2) - 1,
          this.Width, (this.Height / 2) - 1);
        e.Graphics.DrawLine(lightPen, textSize.Width, (this.Height / 2),
          this.Width, (this.Height / 2));
      }

      lightBrush.Dispose();
      darkBrush.Dispose();
      lightPen.Dispose();
      darkPen.Dispose();
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);

      Refresh();
    }

    Color _darkColor = SystemColors.ControlDark;

    [Category("Appearance")]
    Color DarkColor
    {
      get { return _darkColor; }

      set
      {
        _darkColor = value;
        Refresh();
      }
    }

    Color _lightColor = SystemColors.ControlLightLight;

    [Category("Appearance")]
    Color LightColor
    {
      get { return _lightColor; }

      set
      {
        _lightColor = value;
        Refresh();
      }
    }

    EtchEdge _edge = EtchEdge.Top;

    [Category("Appearance")]
    public EtchEdge Edge
    {
      get
      {
        return _edge;
      }

      set
      {
        _edge = value;
        Refresh();
      }
    }

    string _text = string.Empty;

    [Category("Appearance")]
    public string TextLabel
    {
      get
      {
        return _text;
      }

      set
      {
        _text = value;
        Refresh();
      }
    }
  }

  public enum EtchEdge
  {
    Top,
    Bottom,
    Center
  }
}
