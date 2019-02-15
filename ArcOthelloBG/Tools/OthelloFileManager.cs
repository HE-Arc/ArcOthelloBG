using ArcOthelloBG.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ArcOthelloBG
{
    class OthelloFileManager
    {
        public static void SaveToFile(string filename, OthelloState state)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, state);
            stream.Close();
        }

        public static OthelloState LoadStateFromFile(string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            OthelloState state = (OthelloState)formatter.Deserialize(stream);
            stream.Close();

            return state;
        }
    }
}
