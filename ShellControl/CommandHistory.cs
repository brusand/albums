#region Using directives

using System;
using System.IO;
using System.Collections;
using System.Text;
using IniParser;
using IniParser.Model;
using IniParser.Parser;
using NDepend.Path;

#endregion

namespace UILibrary
{
	
	
	internal class BaseApp
	{
		string strAlbumsINIFilePath = @"%USERPROFILE%/INIAlbums.ini";
		static FileIniDataParser inifiledata;
		static IniData inidata;
		static IAbsoluteFilePath IniAbsoluteFilePath;
		
	
		public BaseApp() {
			if (inifiledata == null) {
				inifiledata = new FileIniDataParser();
				var iniFilePath = strAlbumsINIFilePath.ToEnvVarFilePath();
				
				iniFilePath.TryResolve(out IniAbsoluteFilePath);
				
				if (File.Exists(IniAbsoluteFilePath.ToString())) {
						inidata = inifiledata.ReadFile(IniAbsoluteFilePath.ToString());
				}
				else 	{
				 	inifiledata.Parser.Configuration.CommentString = "#";
				 	inidata = new IniData();
				 	inifiledata.WriteFile(IniAbsoluteFilePath.ToString(), inidata);
				}
			}		
		
		
		}
		public void iniwrite(string section, string key, string value ) {
			iniwrite( section, key, value, null );
			
		}		
		public void iniwrite(string section, string key, string value, string comment) {
			if (inidata.Sections.GetSectionData(section) == null) {
				inidata.Sections.AddSection(section);
			}
			if (inidata.Sections.GetSectionData(section).Keys.ContainsKey(key) ) {
				inidata[section][key] = value;
			}
			else {
				inidata.Sections.GetSectionData(section).Keys.AddKey(key, value);
			}
			if (comment != null) {
				inidata.Sections.GetSectionData(section).Keys.GetKeyData(key).Comments
            .Add(comment);
			}
			inifiledata.WriteFile(IniAbsoluteFilePath.ToString(), inidata);
			
		}
		public string iniget(string section, string _key) {
			if (inidata.Sections.GetSectionData(section) == null) {
				return null;
			}
			
			if (inidata.Sections.GetSectionData(section).Keys.ContainsKey(_key))
				return inidata[section][_key].Trim();
			else return null;
		}
		
		public string inisearch(string section, string _key) {
			
			SectionData sd = iniGetSection(section);
			foreach(KeyData key in sd.Keys) {
				if (key.KeyName.Contains(_key)) {
					if (key.KeyName.StartsWith("_")) {
						return key.Value;
					}
					else return key.KeyName;
				}
			}
			return null;
		}		
		public SectionData iniGetSection(string section) {
			return inidata.Sections.GetSectionData(section);
		}		


	}
    internal class CommandHistory
    {
        private int currentPosn;
        private string lastCommand;
        private ArrayList commandHistory = new ArrayList();
		private BaseApp baseApp = null;
		
        internal CommandHistory()
        {
        	// get persist history
        	baseApp = new BaseApp();
        	var i = 1;
        	var command="";
        	while (baseApp.iniget("hist", i.ToString()) != null) {
        		command = baseApp.iniget("hist", i.ToString());
        		commandHistory.Add(command);
	        	lastCommand = command;
		        currentPosn = commandHistory.Count;
		        i++;
        	}
        }

        internal void Add(string command)
        {
            if (command != lastCommand)
		    {
			    commandHistory.Add(command);
	        	lastCommand = command;
		        currentPosn = commandHistory.Count;
		        baseApp.iniwrite("hist", currentPosn.ToString(), command);
		        
		    }
        }

        internal bool DoesPreviousCommandExist()
        {
            return currentPosn > 0;
        }

        internal bool DoesNextCommandExist()
        {
            return currentPosn < commandHistory.Count - 1;
        }

        internal string GetPreviousCommand()
        {
            lastCommand = (string)commandHistory[--currentPosn];
            return lastCommand;
        }

        internal string GetNextCommand()
        {
            lastCommand = (string)commandHistory[++currentPosn];
            return LastCommand;
        }

        internal string LastCommand
        {
            get { return lastCommand; }
        }

        internal string[] GetCommandHistory()
        {
            return (string[])commandHistory.ToArray(typeof(string));
        }
    }
}
