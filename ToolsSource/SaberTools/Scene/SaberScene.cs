using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaberTools.Scene
{
    public class SaberScene
    {
        public string name;

        public SceneCDT cdt;

        public CDList serverCDList;
        public ClassList serverClassList;
        public DOMList serverDomList;

        public CDList clientCDList;
        public ClassList clientClassList;
        public DOMList clientDomList;

        public SaberScene(string name, CDList serverCDList, ClassList serverClassList, DOMList serverDomList, CDList clientCDList, ClassList clientClassList, DOMList clientDomList, SceneCDT cdt)
        {
            this.serverCDList = serverCDList;
            this.serverClassList = serverClassList;
            this.serverDomList = serverDomList;
            this.clientCDList = clientCDList;
            this.clientClassList = clientClassList;
            this.clientDomList = clientDomList;
            this.cdt = cdt;
            this.name = name;
        }

        public SaberScene(string path)
        {
            name = path.Split("\\").Last().Replace(".scn", "");

            var cdListClientPath = Path.Join(path, name + ".cd_list");
            if (File.Exists(cdListClientPath))
            {
                using (var fileStream = new FileStream(cdListClientPath, FileMode.Open, FileAccess.Read))
                {
                    var reader = new BinaryReader(fileStream);
                    clientCDList = new CDList(reader);
                }
            }

            var cdListServerPath = Path.Join(path, "server", name + ".cd_list");
            if (File.Exists(cdListServerPath))
            {
                using (var fileStream = new FileStream(cdListServerPath, FileMode.Open, FileAccess.Read))
                {
                    var reader = new BinaryReader(fileStream);
                    serverCDList = new CDList(reader);
                }
            }

            var domListClientPath = Path.Join(path, name + ".dom_list");
            if (File.Exists(domListClientPath))
            {
                using (var fileStream = new FileStream(domListClientPath, FileMode.Open, FileAccess.Read))
                {
                    var reader = new BinaryReader(fileStream);
                    clientDomList = new DOMList(reader);
                }
            }

            var domListServerPath = Path.Join(path, "server", name + ".dom_list");
            if (File.Exists(domListServerPath))
            {
                using (var fileStream = new FileStream(domListServerPath, FileMode.Open, FileAccess.Read))
                {
                    var reader = new BinaryReader(fileStream);
                    serverDomList = new DOMList(reader);
                }
            }

            var classListClientPath = Path.Join(path, name + ".class_list");
            if (File.Exists(classListClientPath))
            {
                using (var fileStream = new FileStream(classListClientPath, FileMode.Open, FileAccess.Read))
                {
                    var reader = new BinaryReader(fileStream);
                    clientClassList = new ClassList(reader);
                }
            }

            var classListServerPath = Path.Join(path, "server", name + ".class_list");
            if (File.Exists(classListServerPath))
            {
                using (var fileStream = new FileStream(classListServerPath, FileMode.Open, FileAccess.Read))
                {
                    var reader = new BinaryReader(fileStream);
                    serverClassList = new ClassList(reader);
                }
            }

            var cdtPath = Path.Join(path, name + ".cdt");
            if (File.Exists(cdtPath))
            {
                using (var fileStream = new FileStream(cdtPath, FileMode.Open, FileAccess.Read))
                {
                    var reader = new BinaryReader(fileStream);
                    cdt = new SceneCDT(reader);
                }
            }
        }
    
        public void Save(string path)
        {
            Directory.CreateDirectory(Path.Join(path, "server"));
            if (clientCDList != null)
            {
                var cdListClientPath = Path.Join(path, name + ".cd_list");
                using (var fileStream = new FileStream(cdListClientPath, FileMode.Create, FileAccess.Write))
                {
                    var writer = new BinaryWriter(fileStream);
                    clientCDList.Write(writer);
                }
            }
            if (serverCDList != null) {
                var cdListServerPath = Path.Join(path, "server", name + ".cd_list");
                using (var fileStream = new FileStream(cdListServerPath, FileMode.Create, FileAccess.Write))
                {
                    var writer = new BinaryWriter(fileStream);
                    serverCDList.Write(writer);
                }
            }

            if (clientDomList != null)
            {
                var domListClientPath = Path.Join(path, name + ".dom_list");
                using (var fileStream = new FileStream(domListClientPath, FileMode.Create, FileAccess.Write))
                {
                    var writer = new BinaryWriter(fileStream);
                    clientDomList.Write(writer);
                }
            }
            if (serverDomList != null)
            {
                var domListServerPath = Path.Join(path, "server", name + ".dom_list");
                using (var fileStream = new FileStream(domListServerPath, FileMode.Create, FileAccess.Write))
                {
                    var writer = new BinaryWriter(fileStream);
                    serverDomList.Write(writer);
                }
            }

            if (clientClassList != null)
            {
                var classListClientPath = Path.Join(path, name + ".class_list");
                using (var fileStream = new FileStream(classListClientPath, FileMode.Create, FileAccess.Write))
                {
                    var writer = new BinaryWriter(fileStream);
                    clientClassList.Write(writer);
                }
            }
            if (serverClassList != null)
            {
                var classListServerPath = Path.Join(path, "server", name + ".class_list");
                using (var fileStream = new FileStream(classListServerPath, FileMode.Create, FileAccess.Write))
                {
                    var writer = new BinaryWriter(fileStream);
                    serverClassList.Write(writer);
                }
            }
        }
    }
}
