using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AIS_bus_station
{
    class Fonts
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
           IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private PrivateFontCollection fonts = new PrivateFontCollection();

        public Fonts()
        {
            byte[] fontData = Properties.Resources.Garet;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.Garet.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.Garet.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
        }

        public Font UseGaret(float size = 16.0F)
        {
            return new Font(fonts.Families[0], size);
        }

        public Font UseGaretBold(float size = 16.0F)
        {
            return new Font(fonts.Families[0], size, FontStyle.Bold);
        }
    }
}
