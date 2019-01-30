using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FaceRecognition1.Helper
{
    public class XmlFileWriter
    {
        public static object WriteLock = new object();

        public static void WriteDataToFile(string path, double learningError, double validationError, double testingError, TimeSpan elapsedTime,
            int iterationsCount, double learningRate, double momentum, int hiddenLayersCount, int neuronsCount, bool bias, TimeSpan timeSinceStart, bool[] activeFeatures = null, int seed = -1)
        {
            lock (WriteLock)
            {
                if (!File.Exists(path))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.NewLineOnAttributes = true;
                    using (XmlWriter writer = XmlWriter.Create(path, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Networks");

                        writer.WriteStartElement("Network");
                        writer.WriteElementString("LearningError", learningError.ToString());
                        writer.WriteElementString("ValidationError", validationError.ToString());
                        writer.WriteElementString("TestingError", testingError.ToString());
                        writer.WriteElementString("ElapsedTime", elapsedTime.ToString());
                        writer.WriteElementString("IterationsCount", iterationsCount.ToString());
                        writer.WriteElementString("LearningRate", learningRate.ToString());
                        writer.WriteElementString("Momentum", momentum.ToString());
                        writer.WriteElementString("HiddenLayersCount", hiddenLayersCount.ToString());
                        writer.WriteElementString("NeuronsCount", neuronsCount.ToString());
                        writer.WriteElementString("Bias", bias.ToString());
                        writer.WriteElementString("TimeSinceStart", timeSinceStart.ToString());
                        if (activeFeatures != null) writer.WriteElementString("ActiveFeatures", String.Concat(activeFeatures));
                        else writer.WriteElementString("ActiveFeatures", "allFeatures");
                        if (seed != -1) writer.WriteElementString("Seed", seed.ToString());
                        else writer.WriteElementString("Seed", "Static seed");
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        writer.Flush();
                        writer.Close();
                    }
                }
                else
                {
                    XDocument xDocument = XDocument.Load(path);
                    XElement root = xDocument.Element("Networks");
                    IEnumerable<XElement> rows = root.Descendants("Network");
                    XElement firstRow = rows.First();
                    firstRow.AddBeforeSelf(
                       new XElement("Network",
                       new XElement("LearningError", learningError.ToString()),
                    new XElement("ValidationError", validationError.ToString()),
                    new XElement("TestingError", testingError.ToString()),
                    new XElement("ElapsedTime", elapsedTime.ToString()),
                    new XElement("IterationsCount", iterationsCount.ToString()),
                    new XElement("LearningRate", learningRate.ToString()),
                    new XElement("Momentum", momentum.ToString()),
                    new XElement("HiddenLayersCount", hiddenLayersCount.ToString()),
                    new XElement("NeuronsCount", neuronsCount.ToString()),
                    new XElement("Bias", bias.ToString()),
                    new XElement("TimeSinceStart", timeSinceStart.ToString()),
                    activeFeatures != null ? new XElement("ActiveFeatures", String.Concat(activeFeatures)) : new XElement("ActiveFeatures", "allFeatures"),
                    seed != -1 ? new XElement("Seed",seed.ToString()) : new XElement("Seed", "Static seed")
                    ));

                    xDocument.Save(path);
                }
            }
        }
    }
}
