using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tgif_clarifi
{
    class GifResult
    {

        public Gif gif { get; set; }
        public List<Tag> tagsList { get; set; }
        public string time { get; set; }
    }
}
