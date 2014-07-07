using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyLife.Model
{
    class DiaryModel
    {
        public Int32 ID { get; set; }
        public Int32 PubTime { get; set; }
        public Int32 IsUpload { get; set; }
        public String Title { get; set; }
        public String Contents { get; set; }
    }
}
