using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyLife.Model
{
    class TreeModel
    {
        public TreeModel()
        {
            this.Nodes = new List<TreeModel>();
        }
        public Int32 ID { get; set; }
        public Int32 PID { get; set; }
        public String Name { get; set; }
        public Int32? DiaID { get; set; }

        public List<TreeModel> Nodes { get; set; }

    }

}
