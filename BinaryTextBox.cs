using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace unidasmwrapper
{
    public partial class BinaryTextBox : UserControl
    {
        public BinaryTextBox()
        {
            this.resAddress = new DrawResource(this.Font.Name, 24, ForeColor);
            this.resBody = new DrawResource(this.Font.Name, 24, ForeColor);
            this.resChar = new DrawResource(this.Font.Name, 24, ForeColor);
            this.charcode = string.Empty;
            this.datas = new byte[0];
            InitializeComponent();
        }

        public enum EndianType
        {
            LittleEndian,
            BigEndian,
        }

        private EndianType endianness = EndianType.LittleEndian;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EndianType Endian
        {
            get { return this.endianness; }
            set
            {
                if (this.endianness != value)
                {
                    // ToDo: 表示内容の切り替え
                    this.UpdateDisplay();
                }
                this.endianness = value;
            }
        }

        private string charcode;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string CharCode
        {
            get { return this.charcode; }
            set
            {
                if (this.charcode != value)
                {
                    // ToDo:表示内容の切り替え
                    this.UpdateDisplay();
                }
                this.charcode = value;
            }
        }

        public enum ElementSize
        {
            Byte,
            Word,
            DWord,
        }

        private ElementSize elemsize = ElementSize.Byte;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ElementSize ElemSize
        {
            get { return elemsize; }
            set
            {
                if (elemsize != value)
                {
                    // ToDo:表示内容の切り替え
                    this.UpdateDisplay();
                }
                elemsize = value;
            }
        }

        public enum LineElems
        {
            Elem8,
            Elem16,
            Elem32,
        }

        private LineElems lineelems = LineElems.Elem16;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public LineElems LineElementNums
        {
            get { return lineelems; }
            set
            {
                if ( lineelems != value)
                {
                    // ToDo: 表示内容の切り替え
                    this.UpdateDisplay();
                }
                lineelems = value;
            }
        }

        private byte[] datas;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte[] Datas
        {
            get { return datas; }
            set
            {
                this.datas = value;
                // ToDo: 表示内容の切り替え
                this.UpdateDisplay();
            }
        }

        private bool isVisibleAddress;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsVisibleAddress
        {
            get { return isVisibleAddress; }
            set
            {
                if ( isVisibleAddress != value )
                {
                    // ToDo: 表示内容の切り替え
                    this.UpdateDisplay();
                }
                this.isVisibleAddress = value;
            }
        }

        private struct DrawResource
        {
            public Font font;
            public Brush brush;
            public StringFormat format;

            public DrawResource(string fontname, int fontsize, Color col)
            {
                this.font = new Font(fontname, fontsize);
                this.brush = new SolidBrush(col);
                this.format = new StringFormat(StringFormatFlags.NoFontFallback | StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox);
            }
        }

        private DrawResource resAddress;
        private DrawResource resBody;
        private DrawResource resChar;

        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // 表示内容の更新
            Int64 offset = 0;
            float dispx = this.Margin.Left;
            float dispy = this.Margin.Top;
            float spacex = 8;
            // 描画準備
            
            StringBuilder bb = new StringBuilder();
            List<byte> bbc = new List<byte>();
            Int64 ox = 0;
            while (dispy < (this.Height - this.Margin.Bottom) && (offset < this.datas.Length) )
            {
                // アドレス表示
                SizeF szAddr = e.Graphics.MeasureString($"{offset:X08}", resAddress.font, 10000, resAddress.format);
                e.Graphics.DrawString($"{offset:X08}", resAddress.font, resAddress.brush, dispx, dispy);
                dispx += szAddr.Width + spacex;
                // binary本体表示
                bb.Clear();
                bbc.Clear();
                switch (lineelems)
                {
                    case LineElems.Elem8:
                        ox = 8;
                        break;
                    case LineElems.Elem16:
                        ox = 16;
                        break;
                    case LineElems.Elem32:
                        ox = 32;
                        break;
                }
                switch ( elemsize )
                {
                    case ElementSize.Byte:
                        // endian関係なし
                        for (Int64 o = 0; o < ox; o++)
                        {
                            if ((offset + o) < this.datas.Length)
                            {
                                bb.Append($"{this.datas[offset + o]:X02} ");
                                bbc.Add(this.datas[offset + o]);
                            }
                            else
                            {
                                bb.Append($"   ");
                            }
                        }
                        break;
                    case ElementSize.Word:
                        if ( endianness == EndianType.LittleEndian )
                        {
                            for (Int64 o = 0; o < ox; o+=2)
                            {
                                if ((offset + o) < this.datas.Length)
                                {
                                    UInt16 v = (ushort)((UInt16)this.datas[offset + o] + (UInt16)(this.datas[offset + o + 1]<<8));
                                    bb.Append($"{v:X04} ");
                                    bbc.Add(this.datas[offset + o]);
                                    bbc.Add(this.datas[offset + o + 1]);
                                }
                                else
                                {
                                    bb.Append($"     ");
                                }
                            }
                        }
                        else
                        {
                            for (Int64 o = 0; o < ox; o += 2)
                            {
                                if ((offset + o) < this.datas.Length)
                                {
                                    UInt16 v = (ushort)((UInt16)(this.datas[offset + o]<<8) + (UInt16)(this.datas[offset + o + 1]));
                                    bb.Append($"{v:X04} ");
                                    bbc.Add(this.datas[offset + o]);
                                    bbc.Add(this.datas[offset + o + 1]);
                                }
                                else
                                {
                                    bb.Append($"     ");
                                }
                            }
                        }
                        break;
                    case ElementSize.DWord:
                        break;
                }
                string _x = bb.ToString();
                szAddr = e.Graphics.MeasureString(_x, resBody.font, 10000, resBody.format);
                e.Graphics.DrawString(_x, resBody.font, resBody.brush, dispx, dispy);
                dispx += szAddr.Width + spacex;

                // 次の行へ
                dispx = this.Margin.Left;
                dispy += szAddr.Height + 3;     // margin
                offset += ox;



            }


        }

        private void UpdateDisplay()
        {
            this.Invalidate();
        }
    }
}
