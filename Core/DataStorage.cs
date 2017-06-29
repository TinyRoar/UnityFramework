using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Linq;

/*
 * (c) 2015 by Tiny Roar | Dario D. Müller
 * 
 * Add XmlInclude attribute dynamically
 * http://stackoverflow.com/questions/2689566/how-to-add-xmlinclude-attribute-dynamically
 */


namespace TinyRoar.Framework
{
    public class DataStorage : Singleton<DataStorage>
    {

        public void WriteText(string filename, string lines = "")
        {
            StreamWriter file = new StreamWriter(filename);
            file.WriteLine(lines);
            file.Close();
        }

        public List<string> ReadText(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            List<string> list = new List<string>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                list.Add(line);
            }
            reader.Close();
            return list;
        }

        // Binary Serializer
        public void SerializeBinary<T>(string filename, T obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, obj);
            stream.Close();
        }

        // Binary Deserializer
        public T DeserializeBinary<T>(string filename) where T : new()
        {
            T obj;
            if (File.Exists(filename))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None);
                try
                {
                    obj = (T)formatter.Deserialize(stream);
                    stream.Close();
                }
                catch
                {
                    UnityEngine.Debug.LogError("Binary File " + filename + " defect");
                    stream.Close();
                    File.Delete(filename);
                    obj = new T();
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("Binary Deserialize File " + filename + " doesn't exists.");
                obj = new T();
            }
            return obj;
        }

        // Xml Serializer
        public void SerializeXml<T>(string filename, ICollection obj, Type type)
        {
            var types = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                             from lType in lAssembly.GetTypes()
                             where type.IsAssignableFrom(lType)
                             select lType).ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(T), types);
            TextWriter writer = new StreamWriter(filename);
            try
            {
                serializer.Serialize(writer, obj);
            }
            finally
            {
                writer.Close();
            } 
        }

        // Xml Deserializer
        public T DeserializeXml<T>(string filename, Type type) where T : new()
        {
            T obj;
            if (File.Exists(filename))
            {

                var types = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from lType in lAssembly.GetTypes()
                                 where type.IsAssignableFrom(lType)
                                 select lType).ToArray();

                XmlSerializer serializer = new XmlSerializer(typeof(T), types);
                TextReader reader = new StreamReader(filename);
                try
                {
                    obj = (T)serializer.Deserialize(reader);
                    reader.Close();
                }
                catch
                {
                    UnityEngine.Debug.LogError("Xml File " + filename + " defect");
                    reader.Close();
                    obj = new T();
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("Xml Deserialize File  " + filename + " doesn't exists.");
                obj = new T();
            }
            return obj;
        }

        // Xml Deserializer
        public T DeserializeXmlContent<T>(string fileContent, Type type) where T : new()
        {
            T obj;
            var types = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from lType in lAssembly.GetTypes()
                            where type.IsAssignableFrom(lType)
                            select lType).ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(T), types);
            TextReader reader = new StringReader(fileContent);
            try
            {
                obj = (T)serializer.Deserialize(reader);
                reader.Close();
            }
            catch
            {
                UnityEngine.Debug.LogError("Xml File " + fileContent + " defect");
                reader.Close();
                obj = new T();
            }
            return obj;
        }

        public void SaveTextureToFile(Texture2D tex, string filename)
        {
            var bytes = tex.EncodeToPNG();
            var file = File.Open(filename, FileMode.Create);
            var binary = new BinaryWriter(file);
            binary.Write(bytes);
            file.Close();
        }

        public void SaveBinaryToFile(byte[] bytes, string filename)
        {
            var file = File.Open(filename, FileMode.Create);
            var binary = new BinaryWriter(file);
            binary.Write(bytes);
            file.Close();
        }

        public void DeleteFile(string filename)
        {
            if (this.FileExists(filename))
            {
                File.Delete(filename);
            }
        }

        public bool FolderExists(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public bool CreateFolder(string path)
        {
            if (FolderExists(path) == false)
            {
                System.IO.Directory.CreateDirectory(path);
                return true;
            }
            return false;
        }

        public bool DeleteFolder(string path)
        {
            if (FolderExists(path))
            {
                System.IO.Directory.Delete(path, true);
                return true;
            }
            return false;
        }

        public bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        public Texture2D LoadPngToTexture2d(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
                tex.LoadImage(fileData);
            }
            return tex;
        }

    }

}