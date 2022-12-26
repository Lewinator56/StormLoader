using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace StormLoader.Packager
{
    internal class ModPackager
    {
        public static void Package(string name, string author, string version, bool steam, Dictionary<String, DirectoryInfo> data)
        {
            
            bool isComplete = false;

            //setup working directories
            string packageRoot = "./packages/" + name;
            string slpRoot = packageRoot + "/SLP";
            string steamRoot = packageRoot + "/steam";
            Directory.CreateDirectory(packageRoot);
            Directory.CreateDirectory(slpRoot);

            // create the slp zip
            ZipFile z = new ZipFile();
            DbgLog.WriteLine("Creating mod package: " + name);

            
            try
            {
                z.AddDirectory(data["meshes"].FullName, "Meshes");
            } catch {
                DbgLog.WriteLine("No mesh folder, bypassing");
            }
            try
            {
                z.AddDirectory(data["definitions"].FullName, "Definitions");
            } catch {
                DbgLog.WriteLine("No definitions folder, bypassing");
            }
            try
            {
                z.AddDirectory(data["audio"].FullName, "Audio");
            } catch {
                DbgLog.WriteLine("No audio folder, bypassing");
            }
            try
            {
                z.AddDirectory(data["graphics"].FullName, "Graphics");
            } catch {
                DbgLog.WriteLine("No graphics folder, bypassing");
            }
            try
            {
                z.AddDirectory(data["data"].FullName, "Data");
            } catch {
                DbgLog.WriteLine("No data folder, bypassing");
            }

            // copy info directory
            try
            {
                z.AddDirectory(data["info"].FullName);
            } catch {
                DbgLog.WriteLine("No info folder, bypassing");
            }
            //create metadata file
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.IndentChars = "\t";
            xws.OmitXmlDeclaration = false;
            xws.Encoding = Encoding.UTF8;

            Directory.CreateDirectory("./meta");
            XmlWriter xw = XmlWriter.Create("./meta" + "/Metadata.xml", xws);
            xw.WriteStartDocument();
            xw.WriteStartElement("Metadata");
            xw.WriteElementString("Author", author);
            xw.WriteElementString("Version", version);
            xw.WriteEndElement();
            xw.WriteEndDocument();

            xw.Close();
            z.AddFile("./meta/Metadata.xml", "/");
            


            z.Save(slpRoot+ "/" + name + ".slp");
            Directory.Delete("./meta", true);

            if (steam)
            {
                PackageSteam(steamRoot, slpRoot, name);
                MessageBox.Show("Your mod package has automatically been created in the stormworks missions directory. Open stormworks, enter the mission editor, open the mission (DO NOT EDIT IT) and upload it to the workshop!");
                System.Diagnostics.Process.Start("explorer.exe", new DirectoryInfo(steamRoot).FullName);
            }
            System.Diagnostics.Process.Start("explorer.exe", new DirectoryInfo(slpRoot).FullName);

        }

        public static void PackageSteam(string steamRoot, string slpRoot, string name)
        {
            //Directory.CreateDirectory(steamRoot);
            string missionsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/stormworks/data/missions";
            string missionRoot = Path.Combine(missionsFolder, name);

            

            Directory.CreateDirectory(missionsFolder);
            Directory.CreateDirectory(missionRoot);

            var psi = new ProcessStartInfo("cmd.exe", $@" /C mklink /j ""{new DirectoryInfo(steamRoot).FullName}"" ""{missionRoot}""");
            //DbgLog.WriteLine($@" /C mklink /j ""{new DirectoryInfo(steamRoot).FullName}"" ""{missionRoot}""");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi).WaitForExit();

            File.Create(missionRoot + "/script.lua");
            ZipFile z = new ZipFile();
            z.AddFile(slpRoot + "/" + name + ".slp");
            z.Save(missionRoot + "/vehicle_2.xml");

            /**
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.IndentChars = "\t";
            xws.OmitXmlDeclaration = false;
            xws.Encoding = Encoding.UTF8;

            XmlWriter xw = XmlWriter.Create(missionRoot + "/playlist.xml", xws);
            xw.WriteStartDocument();
            xw.WriteStartElement("playlist");
            xw.WriteAttributeString("path_id", "app_data/data/missions/" + name);
            xw.WriteAttributeString("folder_path", "data/missions/" + name);
            xw.WriteAttributeString("file_store", "4");
            xw.WriteAttributeString("name", name);

            xw.WriteStartElement("locations");
            xw.WriteAttributeString("location_id_counter", "2");
            xw.WriteAttributeString("component_id_counter", "2");

            xw.WriteStartElement("locations");

            xw.
            xw.WriteElementString("Author", author);
            xw.WriteElementString("Version", version);
            xw.WriteEndElement();
            xw.WriteEndDocument();

            xw.Close();
            **/

            // be lazy...
            string data = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<playlist path_id=""app_data/data/missions/{name}"" folder_path=""data/missions/{name}"" file_store=""4"" name=""{name}"">
	<locations location_id_counter=""2"" component_id_counter=""2"">
		<locations>
			<l id=""1"" name=""OCEAN"" is_env_mod=""true"" env_mod_spawn_num=""1"">
				<components>
					<c component_type=""1"" id=""1"" name=""id_1"" dynamic_object_type=""2"" character_outfit_category=""11"" character_type=""1"" vehicle_file_store=""1"">
						<spawn_transform 30=""0.108637"" 31=""0.879519"" 32=""0.5""/>
						<spawn_bounds>
							<min x=""-0.475"" y=""-0.475"" z=""-0.475""/>
							<max x=""0.475"" y=""0.475"" z=""0.475""/>
						</spawn_bounds>
						<graph_links/>
					</c>
					<c component_type=""3"" id=""2"" name=""id_2"" dynamic_object_type=""2"" character_outfit_category=""11"" character_type=""1"" vehicle_file_name=""data/missions/vehicle_2.xml"" vehicle_file_store=""4"">
						<spawn_transform 30=""0.125707"" 31=""3.75"" 32=""-12.516552""/>
						<spawn_bounds>
							<min x=""-14.625"" y=""-4.125"" z=""-21.125""/>
							<max x=""14.125"" y=""3.375"" z=""10.375""/>
						</spawn_bounds>
						<spawn_local_offset x=""0.25"" y=""0.375"" z=""5.375""/>
						<graph_links/>
					</c>
				</components>
			</l>
			<l id=""2"" name=""OCEAN"" env_mod_spawn_num=""1"">
				<components/>
			</l>
		</locations>
	</locations>
</playlist>";
            File.WriteAllText(missionRoot + "/playlist.xml", data);

        }

        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);
    }
}
