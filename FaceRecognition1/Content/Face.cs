using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition1.Content
{
    public class Face
    {
        public string name { get; set; }
        public int index { get; set; }
        public int networkIndex { get; set; }
        public List<float> features { get; set; }

        public Face()
        {
            this.name = "unknown";
            this.index = -1;
            this.networkIndex = -1;
            this.features = new List<float>();
        }
        public int ValidateFace()
        {
            if(this.name!="unknown" && this.index!=-1 && this.networkIndex!=-1 && this.features.Count>1)
                return 1;
            else
                return -1;
        }
    }
}
