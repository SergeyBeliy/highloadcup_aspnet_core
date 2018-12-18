using System.IO;
using System.IO.Compression;

namespace AccountsApi.Controllers.Infrastructure {
    public class Database { 
        public Database()
        {
            
        }

        public void InitWatcher(string zipPath){
            var fsw = new FileSystemWatcher();
            // fsw.WaitForChanged()
        }
    }
}