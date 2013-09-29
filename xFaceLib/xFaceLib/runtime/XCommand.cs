using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xFaceLib.runtime
{
    public class XCommand
    {
        public String className { get; private set; }
        public String methodName { get; private set; }
        public String callbackId { get; private set; }
        public String arguments { get; private set; }

        /// <summary>
        /// 解析js端传入的command字符串,创建command对象
        /// </summary>
        /// <param name="commandStr">js 端传入的command string</param>
        /// <returns>js端传人的command错误则返回null,正常返回command对象</returns>
        public static XCommand parse(string commandStr)
        {
            if (string.IsNullOrEmpty(commandStr))
            {
                return null;
            }

            string[] split = commandStr.Split('/');
            if (split.Length < 3)
            {
                return null;
            }

            XCommand command = new XCommand();

            command.className = split[0];
            command.methodName = split[1];
            command.callbackId = split[2];
            command.arguments = split.Length <= 3 ? String.Empty : String.Join("/", split.Skip(3));

            // 检查非法名字
            if (command.className.IndexOfAny(new char[] { '@', ':', ',', '!', ' ' }) > -1)
            {
                return null;
            }

            return command;
        }

        /// <summary>
        /// Private ctr to disable class creation.
        /// New class instance must be initialized via CordovaCommandCall.Parse static method.
        /// </summary>
        private XCommand()
        {
        }
    }
}
