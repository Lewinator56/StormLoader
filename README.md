# StormLoader
Mod manager (loader and packager) for stormworks

# Disclaimer
I, StormLoader and mods loaded with stormloader are not affiliated with stormworks, or Sunfire Software LTD in any way. We claim no rights or ownership in any way to any file formats or folder structures used in mods and/or mod packages other than where a packaging format has been designed specifically for the operation of the mod loader. The StormLoader development team cannot guarantee the safety of mods downloaded and installed from sources not sanctioned by the development team. 

# Downloads
- Head to releases to get the latest version!

# Steam Mods?
Yep, thats right, the process is pretty simple, I wont explain quite HOW its working, it just goes on behind the scenes, and makes use of a mission.
For the end user.
* Open the built in modd packager from the top right menu and select all the folders you need (if you have an 'info' folder, make sure its separate from the data folders as selecting the parent for a mod structure may cause issues).
* Select the 'package for steam' checkbox then click package.
* Open stormloader, locate the newly created mission - DO NOT EDIT ANYTHING - and upload it to the workshop.

To download a steam mod:
* Subscribe to the mod on steam - the creator will indicate that it works with stormloader in the description or tag it with [MOD]
* Open stormloader and click 'sync steam' and wait for the mods to be added then launch the game.

# Major Update v1.1 - RELEASED!
I'm slowly working on a pretty significant update which adds a number of requested features and makes existing ones more robust. The feature list is as follows:
* Completely re-written mod install tracking code. This now makes use of serialisation to directly store data structures for mods. This is far faster and more reliable than the old XML version and offers easier expansion in the future.
* Completely re-written profile handling. Profiles are now handled in a similar way to mod install file tracking.
* Support for mods uploaded onto the steam workshop. Hiding data in missions makes this possible.
* A mod packager. This does all the hard work of creating an SLP for you, all you need to do is supply the right folders. Also packages for the workshop as the method for this is complex.
* Mod context menus. 
* Easily access mod files, game files and install locations straight from StormLoader.
* Custom theme support. Change primary, secondary, accent and text colours to your liking.

As this update is still in progress, please feel free to make suggestions by opening new issues!

# Security Update
I've had a few questions about security for database connections. As those of you in the security sector will know, having a direct database connection in a public facing application is not a particually good idea, unless of course there are other protections at the server end. I of course implement protections at the server end, i.e the user that stormloader connects to the database ONLY has SELECT access rights for specific tables and fields, it was however brought to my attention that due to an unnoticed misconfiguration of these access rights, this user could also update mod IDs, this has now been fixed. The details for the user are stored within a DLL, which you can decompile if you wish, though this DLL has now been merged with the executable. As the user has very limited access rights, there is no security risk from allowing this. I will allow connections to the database from other applications using the login details in the DLL, or the DLL itself, however, I advise if you choose to do this, please do not use the URL in the DLL, please use: portal.stormloader.uk as the URL, the URL in the DLL is a relic of a past project and will go inactive soon. In the near future, I will be changing mod database access to an API.

Any questions about this, please dm me on discord or start a new issue here.


# FAQ
**Q** What platforms are supported?<br>
**A** StormLoader only runs on windows.

**Q** What prerequisites do I need?<br>
**A** None, other than the latest .net framework, which is pre-installed on windows.

**Q** Are mods officially supported by the stormworks development team?<br>
**A** While we cannot provide a definitive answer to this, the current stance is that there is no official stance, we have had no contact from the developers.

# Getting Started
* make sure to take a look at the user guide here: https://github.com/Lewinator56/StormLoader/blob/master/StormLoader%20user%20guide.pdf.
* Any bugs, issues or features please create an issue on the repo with the links above.
* If you want to make your own mods, this is the wrong tool, you are looking for the mesh converter, which can be found here: https://github.com/Lewinator56/swMesh2XML_repo.
* Any other support can be requested on discord by DM'ing @Lewinator56#9325 or joining the SMF discord server: https://discord.gg/9HS7cb6.
# Manage your Mods
* Ive recently added an online portal to manage mods for stormworks, and that will directly impact the repository browser in StormLoader, its all secured over HTTPS, and is available here: https://portal.stormloader.uk
* Ill be adding a repo browser to it soon too!

# The Repository
Yep, StormLoader has its own repository, its currently hosted on one of my servers, of course, you can host it yourself (and all the info about it is below). Now, i have to mention that if you want to upload stuff onto it, you will need to create an account, this only needs a username and password, both of which are not classified as identifiable information, of course, im taking appropriate precautions to protect your data. Where applicable, information is transmitted pre-hashed to the server, so your passwords are never stored, transmitted or accessed in plain text, in fact, they only temoprarily exist in local RAM on your system in plain text. The connection infromation for the SQL database is securely stored in a DLL, that isnt open source. Only I have access to the database (and the user for the StormLoader connections), and as such, your data is protected. Should you wish to have data removed, just ask and ill do it ASAP (ok, i *could* have added an option to delete your account from within StormLoader, but chose not to)

Any questions, just open up an issue and I'll respond as soon as i notice the new issue!

# Building Yourself
If you want to build stormloader yourself (because you dont trust me, or want to customize it for your needs), you will need visual studio installed. Stormloader also uses the 'material design in XAML' nuget package, this should be automatically set up if you pull to project from github, however, if this isnt the case, search for it in the nuget package manager.

Additionaly, to enable repository functionality you will need to set up the SQL connection class, this is distributed as a DLL (StormLoaderData) with the release and you can use this as a reference, alternatively, you can build your own class, below is all the code for the DLL, without the connection information for the SQL server.

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
### Online Functionality
If you want to set up the online repository yourself, the database MUST be set up like so:
![image](https://user-images.githubusercontent.com/56686419/113419942-568cbd00-93c0-11eb-9efb-aae8061871e7.png)

It is of course up to you to ensure that the user set up to access the mods database on behalf of stormloader has appropriate access rights ONLY for the actions it should need to perform, these actions are:
* Add new entries to the users table
* Add, update and delete entries from the mods table

Below I have included images of the tables in PHPMyAdmin, as this contains more detail about specific fields:

### mods
![image](https://user-images.githubusercontent.com/56686419/113420287-f4808780-93c0-11eb-80e8-b2fb3ab1c16d.png)

note that while there is the ability to add a path to an external mod file, for example, hosted on a different server rather than internally on the database server, stormloader cannot yet download the mod from this path.
### users
![image](https://user-images.githubusercontent.com/56686419/113420337-095d1b00-93c1-11eb-807e-9c302583f4b3.png)


If you have any questions, open an issue or contact me on discord.



