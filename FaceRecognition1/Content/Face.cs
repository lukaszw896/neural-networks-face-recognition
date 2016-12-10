using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition1.Content
{
    /// <summary>
    /// Klasa ktora zawiera elementy kazdej twarzy
    /// potrzebne do uzycia w sieci neuronowej. 
    /// name, index identyfikuje zdjecie.
    /// features sa inputem do sieci neuronowej.
    /// networkIndex bedzie oczekiwanym wynikiem sieci.
    /// </summary>
    [Serializable]
    public class Face
    {
        public string name { get; set; }
        public string folderName { get; set; }
        public int index { get; set; }
        public int networkIndex { get; set; }
        public List<float> features { get; set; }

        /// <summary>
        /// Pusty konstruktor
        /// </summary>
        public Face()
        {
            this.name = "unknown";
            this.folderName = "unknown";
            this.index = -1;
            this.networkIndex = GenerateNetworkIndex();
            this.features = new List<float>();
        }

        /// <summary>
        /// Metoda sprawdza czy twarz zostala zainicjowana. 1 jak sie uda
        /// -1 jak sie nie uda
        /// </summary>
        public int ValidateFace()
        {
            if(this.name!="unknown" && this.index!=-1 && this.networkIndex!=-1 && this.folderName != "unknown" && this.features.Count>1)
                return 1;
            else
                return -1;
        }

        /// <summary>
        /// Tu bedziemy tworzyc index ktory bedzie odpowiedzia sieci
        /// narazie nie wiadomo czy to musza byc kolejne inty, czy mozna 
        /// je jakos generowac z nazw pliku i folderu
        /// </summary>
        public int GenerateNetworkIndex()
        {
            int index = 0;
            return index;
        }
    }
}
