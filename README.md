# StormLoader
Mod manager (loader and packager) for stormworks

# Disclaimer
I, StormLoader and mods loaded with stormloader are not affiliated with stormworks, or Sunfire Software LTD in any way. We claim no rights or ownership in any way to any file formats or folder structures used in mods and/or mod packages other than where a packaging format has been designed specifically for the operation of the mod loader. The StormLoader development team cannot guarantee the safety of mods downloaded and installed from sources not sanctioned by the development team. 

# Downloads
- Head to releases to get the latest version!

# FAQ
**Q** What platforms are supported?<br>
**A** StormLoader only runs on windows.

**Q** What prerequisites do I need?<br>
**A** None, other than the latest .net framework, which is pre-installed on windows.

**Q** Are mods officially supported by the stormworks development team?<br>
**A** While we cannot provide a definitive answer to this, the current stance is that there is no official stance, we have had no contact from the developers.

# Building Yourself
If you want to build stormloader yourself (because you dont trust me, or want to customize it for your needs), you will need visual studio installed. Stormloader also uses the 'material design in XAML' nuget package, this should be automatically set up if you pull to project from github, however, if this isnt the case, search for it in the nuget package manager.

Additionaly, to enable repository functionalyity you will need to set up the SQL connection class, this is distributed as a DLL (StormLoaderData) with the release and you can use this as a reference, alternatively, you can build your own class, below is all the code for the DLL, without the connection information for the SQL server.

```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace StormLoaderRepoConnectorData
{
    public class Connection
    {
        static string server = ;
        static string database = ;
        static string user = ;
        static string password = ;
        static string port = "3306";

        public static MySqlConnection connect()
        {
            MySqlConnection conn;
            string constr = "server=" + server + ";user=" + user + ";database=" + database + ";port=" + port + ";password=" + password;
            conn = new MySqlConnection(constr);
            return conn;
        }
    }
}
```
If your class differes from this, you may well need to change the code that sets up the connection the `SQLManager` class, you will need to ensure the code in the `connect` method sets the `conn` variable, like so. You may also completely disregard the use of another class returning a connection object and create a new one in the connection method in `SQLManager`, this can be done by copying the code in the connection method in the DLL (above) to replace the code in the connection method in `SQLManager` (without the `return conn` line)

```C#
public void connect()
{
    //string constr = "server=" + server + ";user=" + user + ";database=" + database + ";port=" + port + ";password=" + password;
    conn = StormLoaderRepoConnectorData.Connection.connect();
}
```
