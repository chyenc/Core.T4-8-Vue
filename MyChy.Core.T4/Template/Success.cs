using MyChy.Core.T4.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyChy.Core.T4.Template
{
    public class Success
    {
        /// <summary>
        /// 输出模板
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="EntityNamespace"></param>
        public async Task Write(string Path, IList<MyChyEntityNamespace> list)
        {
            await CreatTxt(Path, list);
        }

        private async Task CreatTxt(string Path, IList<MyChyEntityNamespace> list)
        {
            string files = Path + "/Success.cs";

            var _sw = new StreamWriter(new FileStream(files, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read), Encoding.UTF8);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("生成成功");
            await _sw.WriteAsync(sb.ToString());
            _sw.Close();


        }
    }
}
