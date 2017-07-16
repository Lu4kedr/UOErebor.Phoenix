using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Phoenix.Plugins
{
    public static class XmlSerializeHelper<T>
    {
        private static string path_name = Core.Directory + @"\Profiles\XML\" + World.Player.Name + @"\";
        private static string path = Core.Directory + @"\Profiles\XML\";
        public static Type _type = typeof(T);


        public static void Save(string filename, object obj, bool path_Name)
        {
            string _path;
            if (path_Name) _path = path_name;
            else _path = path;
            try
            {

                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }

                if (File.Exists(_path + filename))
                {
                    if (File.Exists(_path + filename + "_backup"))
                        File.Delete(_path + filename + "_backup");
                    File.Copy(_path + filename, _path + filename + "_backup");
                    File.Delete(_path + filename);
                }
                var serializer = new XmlSerializer(_type);
                using (var stream = File.Open(_path + filename, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                {
                    serializer.Serialize(stream, obj);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.InnerException.ToString()); }

        }
        private static bool IsExist(string filename, bool path_Name)
        {
            string _path;
            if (path_Name) _path = path_name;
            else _path = path;
            return (File.Exists(_path + filename));
        }

        public static void Load(string filename, out T obj, bool path_Name)
        {

            if (IsExist(filename, path_Name))
            {
                obj = Deserialize(filename, path_Name);
                return;
            }
            obj = default(T);
        }
        private static T Deserialize(string filename, bool path_Name)
        {
            string _path;
            if (path_Name) _path = path_name;
            else _path = path;
            T XMLOBJ;
            var serializer = new XmlSerializer(_type);
            using (var stream = File.OpenRead(_path + filename))
            {
                XMLOBJ = (T)serializer.Deserialize(stream);
            }
            return XMLOBJ;



        }

    }
}
