using FaceRecognition1.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FaceRecognition1.Helper
{
    public class TestHelper
    {
        public int[] ludzie = { 5, 15 };
        public int[] neurony = { 5, 12, 20 };
        public int[] warstwy = { 1, 3, 5 };
        public int[] bias = { 0, 1 };
        public int[] rozlacznosc = { 0, 1 };
        public int[] iteracje = {10000, 30000, 80000 };
        public int[] podejscie = { 1,2,3,4 };

        public List<SingleTest> testy;

        public TestHelper()
        {
            this.testy = new List<SingleTest>();
        }

        public async void PerformTests()
        {
            int te = 0;
            string sol = "C:\\Users\\PC\\Documents\\Visual Studio 2013\\Projects\\FaceRecognition1\\WYNIKI";
            for(int l = 0 ; l < ludzie.Length ; l++)
            {
                List<Face> faces = new List<Face>();
                String pietnascie = "C:\\Users\\PC\\Documents\\Visual Studio 2013\\Projects\\FaceRecognition1\\Twarze N 15x20\\ZdjeciaInput302.bin";
                String piec = "C:\\Users\\PC\\Documents\\Visual Studio 2013\\Projects\\FaceRecognition1\\Twarze N 5x20\\ZdjeciaInput104.bin";
                String adr = "";
                if(l == 0)
                    adr = piec;
                else
                    adr = pietnascie;
                FileStream fs = new FileStream(adr, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                BinaryReader br = new BinaryReader(fs);

                faces = (List<Face>)bf.Deserialize(fs);

                fs.Close();
                br.Close();

                for(int n = 0; n < neurony.Length ; n++)
                {
                    for(int w = 0 ; w < warstwy.Length ; w++)
                    {
                        for(int b = 0 ; b < bias.Length ; b++)
                        {
                            for(int r = 0 ; r < rozlacznosc.Length ; r++)
                            {
                                for(int i = 0 ; i < iteracje.Length ; i++)
                                {
                                    for(int p = 0 ; p < podejscie.Length ; p++)
                                    {
                                        SingleTest test = new SingleTest(ludzie[l], neurony[n], warstwy[w], bias[b], rozlacznosc[r], iteracje[i]);
                                        test.RunTest(faces);
                                        testy.Add(test);
                                        Console.WriteLine("test " + te + " przeprowadzaony");
                                        te++;
                                    }
                                }
                            }
                            string name = "sol_" + l + "_" + n + "_" + w +"_" + b +".txt";
                            using (StreamWriter sw = new StreamWriter(sol +"\\"+ name))
                            {
                                for (int i = 0; i < testy.Count; i++)
                                {
                                    if (i != 0 && i % 4 == 0)
                                    {
                                        sw.WriteLine("-------------------------------------------------------------------------------------");
                                    }
                                    string tmpString = "";
                                    tmpString = testy[i].ToText();
                                    sw.WriteLine(tmpString);
                                }
                            }
                            Console.WriteLine("Test performed and file saved!");
                            ///lub tu
                        }
                    }                    
                }
            }
            string name2 = "FINALSOLUTION.txt";
            using (StreamWriter sw = new StreamWriter(sol + "\\" +name2))
            {
                for (int i = 0; i < testy.Count; i++)
                {
                    if (i != 0 && i % 4 == 0)
                    {
                        sw.WriteLine("-------------------------------------------------------------------------------------");
                    }
                    string tmpString = "";
                    tmpString = testy[i].ToText();
                    sw.WriteLine(tmpString);
                    
                }
            }
            MessageBox.Show("Test performed and file saved!");
            //save solutions
        }
    }
}
