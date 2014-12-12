using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    public static class IOUtils
    {
        /**
   * Open a BufferedReader to a file, class path entry or URL specified by a String name.
   * If the String starts with https?://, then it is first tried as a URL. It
   * is next tried as a resource on the CLASSPATH, and then it is tried
   * as a local file. Finally, it is then tried again in case it is some network-available
   * file accessible by URL. If the String ends in .gz, it
   * is interpreted as a gzipped file (and uncompressed). The file is then
   * interpreted as a utf-8 text file.
   *
   * @param textFileOrUrl What to read from
   * @return The BufferedReader
   * @throws IOException If there is an I/O problem
   */
    public static StreamReader readerFromString(string textFileOrUrl){
        return new StreamReader(File.OpenRead(textFileOrUrl));
      }

    
    }
}
