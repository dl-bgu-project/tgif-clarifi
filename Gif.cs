using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace tgif_clarifi
{   
    [DelimitedRecord("\t")]
    class Gif
    {
        public string GifUrl { get; set; }
        public string GifDesc { get; set; }
    }
}
