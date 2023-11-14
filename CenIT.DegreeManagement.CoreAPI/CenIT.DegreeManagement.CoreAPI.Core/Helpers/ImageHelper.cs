using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CenIT.DegreeManagement.CoreAPI.Core.Helpers
{
    public class ImageHelper
    {
        private Size CalcSize(int w, int h)
        {
            var iW = 700;
            var iH = h * 540 / w;

            if (iH > 540)
            {
                iW = w * 540 / h;
                return iW < 700 ? new Size(700 * 2, 700 * h / w * 2) : new Size(iW * 2, 540 * 2);
            }

            iW = 540 * w / h;
            return new Size(iW * 2, 540 * 2);
        }
    }
}
