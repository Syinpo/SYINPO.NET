using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syinpo.ApiDoc.Generation.Model {
    public class DocNode {
        public string Name {
            get;set;
        }
        public string Title {
            get;set;
        }
        public string Path {
            get;set;
        }

        public int Level {
            get; set;
        }

        public List<DocNode> Children = new List<DocNode>();
    }
}
