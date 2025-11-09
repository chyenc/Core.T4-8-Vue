using MyChy.Core.T4.Common;
using MyChy.Frame.Core.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyChy.Core.T4.Template
{
    public class CoreEFStartupTask
    {
       // private const string IPath = "/MyChy.Core";
        private const string IPath = "/MyChy.Core.Domains";
        private const string Ipath = "/StartupTask";
        //private const string Implementation = "/Implementation";



        /// <summary>
        /// 输出模板
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="EntityNamespace"></param>
        public async Task Write(string Path, IList<MyChyEntityNamespace> list)
        {
            string filePath = Path + IPath + Ipath;
            FileHelper.CreatedFolder(filePath);
            var files= filePath + "/CoreEFStartupTask.txt";

            var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

            StringBuilder sb = new StringBuilder();
            foreach (var i in list)
            {
                foreach (var x in i.FileName)
                {
                    foreach (var y in x.Attributes)
                    {
                        foreach (var z in y.List)
                        {
                            if (y.Types0f == "Attributes" || !string.IsNullOrEmpty(z.Name))
                            {
                                switch (z.Name)
                                {
                                    case "EnumListStringAttribute":
                                        sb.AppendLine($" enumList = new EnumList()");
                                        sb.AppendLine("{");
                                        sb.AppendLine($"Title = \"{x.Description}-{y.Description}\",");
                                        sb.AppendLine($"State = true,");
                                        sb.AppendLine($"Coding = \"{z.Name}\",");
                                        sb.AppendLine($"Rrecommend = 0,");
                                        sb.AppendLine("}; ");
                                        sb.AppendLine($"db.Set<EnumList>().Add(enumList); ");
                                        sb.AppendLine(" ");
                                        break;
                                    case "EnumListCheckAttribute":
                                        sb.AppendLine($" enumList = new EnumList()");
                                        sb.AppendLine("{");
                                        sb.AppendLine($"Title = \"{x.Description}-{y.Description}\",");
                                        sb.AppendLine($"State = true,");
                                        sb.AppendLine($"Coding = \"{z.Name}\",");
                                        sb.AppendLine($"Rrecommend = 0,");
                                        sb.AppendLine("}; ");
                                        sb.AppendLine($"db.Set<EnumList>().Add(enumList); ");
                                        sb.AppendLine(" ");
                                        break;
                                }
                            }
                        }

                    }

                }
            }
            sb.AppendLine("");
            await _sw.WriteAsync(sb.ToString());
            _sw.Close();
        }
    }
}
