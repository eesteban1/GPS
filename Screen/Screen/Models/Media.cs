using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Screen.Models
{
    public class Media
    {
        public Guid MediaID { get; set; }
        public string path { get; set; }
        public DateTime LocalDatetime { get; set; }
        public FileImageSource source = null;
        public FileImageSource Source => source ?? (source = new FileImageSource() { File = path });
    }
}
