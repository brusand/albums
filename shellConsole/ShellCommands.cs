/*
 * Created by SharpDevelop.
 * User: EBDU6461
 * Date: 16/10/2014
 * Time: 13:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using CLAP;
using ShellConsoleApp;
using IniParser;
using IniParser.Model;
using IniParser.Parser;

using NDepend.Path;
using ShellUtils;
using ExifLib;
using System.Globalization;

namespace ShellCommands
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	        class Event
            {
                public string Name { get; set; }
                public DateTime From { get; set; }
            }
	        
	public class BaseApp
	{
		string strAlbumsINIFilePath = @"%USERPROFILE%/INIAlbums.ini";
		public ConsoleLog console;
		static FileIniDataParser inifiledata;
		static IniData inidata;
		static IAbsoluteFilePath IniAbsoluteFilePath;
		
		public BaseApp(ConsoleLog _console) {
			console = _console;
			BaseAppIni();

		}
		public BaseApp() {
			BaseAppIni();
		}
		public void BaseAppIni() {
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

			return inidata[section][_key];
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
		[Error]
		public  void Error(ExceptionContext ex)
		{
			//            shellControl1.ForegroundColor = ConsoleColor.Red;
			//            shellControl1.WriteText(ex.Exception.Message);
			//console.ResetColor();
		}

		[Empty]
		[Help(Aliases = "h,?")]
		public  void Help(string help)
		{
			//            shellControl1.WriteText(help);
		}

		//        [Global]
		//        public static void Debug()
		//        {
		//            Debugger.Launch();
		//        }
		//       public lo

	}
	
	[TargetAlias("config")]
	public class ConfigApp : BaseApp {
		
		//private event ConsoleEventHandler ConsoleLogEvent;
		public ConfigApp(ConsoleLog consoleLog) : base (consoleLog) {

		}

		[Verb]
		public void List()
		{
				//            shellControl1.WriteText("list");
	
				//for (int i = 0; i < count; i++)
				//{
				//    Console.WriteLine("variable {0} = {1}", _key, _value);
				//}
		}
		
		[Verb(Description="set key value")]
		public void set([AliasesAttribute("arg0")] string key, [AliasesAttribute("arg1")] string value, [AliasesAttribute("m")] string message)
		{
			//for (int i = 0; i < count; i++)
			//{
			//                shellControl1.WriteText("Config set {0} = {1}"+ key+ '-'+ value);
			//}
			//BaseApp.logConsole("config set");

			// ConsoleLogEvent(new Object(), new ConsoleEventArgs());
			if (message == null) {
				iniwrite("config", key, value, message);
			}
			else iniwrite("config", key, value, message);
			//inidata["config"][key] = value;
			console.DoLog("config log set " + iniget("config", key));
		}
		
//			static protected void OnConsoleEvent(ConsoleEventArgs e)
//			{
//
//				if (ConsoleEvent == null) {
//					ConsoleLog consoleLog = new ConsoleLog();
//
//					ConsoleEvent += new ConsoleEventHandler(consoleLog.OnConsole);
//				}
//				if(ConsoleEvent!=null) {
//					//ConsoleEvent(new Object(), e);
//					ConsoleLog.DoLog("config");
//				}
//			}
	}
	
	[TargetAlias("depots")]
	public class DepotsApp : BaseApp
	{
		public DepotsApp(ConsoleLog consoleLog) : base (consoleLog) {
			
		}
		[Verb]
		public  void List()
		{
			//            shellControl1.WriteText("list");

			//for (int i = 0; i < count; i++)
			//{
			//    Console.WriteLine("variable {0} = {1}", _key, _value);
			//}
			SectionData depotsSection = iniGetSection("depots");
			foreach(KeyData key in depotsSection.Keys) 
     			 console.DoLog(key.KeyName + " = " + key.Value);
			
			
		}
		[Verb(Description="set key value")]
		public  void set([AliasesAttribute("arg0")] string key, [AliasesAttribute("arg1")] string value, [AliasesAttribute("m")] string message)
		{
			iniwrite("depots", key, value, message);
			console.DoLog(key  + " = " + iniget("depots", key));
		}
		
		[Verb(Description="add label path (description) ")]
		public  void add([AliasesAttribute("arg0")] string label,  [AliasesAttribute("arg1")] string path, [AliasesAttribute("m")] string message)
		{
			//for (int i = 0; i < count; i++)
			//{
			//                shellControl1.WriteText("Depot set {0} = {1}" + key +'/'+ value);
			//}
			

			
			if (!System.IO.Directory.Exists(path)){
			    	//create if not exist
			    	System.IO.Directory.CreateDirectory(path);
			}
			iniwrite("depots", label, path, message);
			iniwrite("depots", "_depot", label);
			
			//inidata["config"][key] = value;
			System.IO.Directory.SetCurrentDirectory(path);
			console.DoPromptDepot(label);
		}
		
		[Verb(Description="sel label")]
		public  void sel([AliasesAttribute("arg0")] string label)
		{
			string _label = inisearch("depots", label);
			string _path  = iniget("depots", _label);
			if (_path != null) {
				iniwrite("depots", "_depot", _label);
				System.IO.Directory.SetCurrentDirectory(_path);
				
				console.DoPromptDepot(_label);
			}
		}

	}
	
	[TargetAlias("event")]
	public class EventApp : BaseApp
	{
		ConsoleLog console;
		FileIniDataParser inieventfiledata;
		IniData inieventdata;
		string strINIEventPath;
		
		public EventApp(ConsoleLog _console) : base (_console)
		{
			console = _console;
		}
		
		[Verb]
		public  void Sel([AliasesAttribute("arg0")] string label, [AliasesAttribute("d")] string depot)
		{
			//            shellControl1.WriteText("list");
			
			
			// if event = .. set parent event
			
			// find the event in the depot
			
			// if only one -> set current event
			
			
			
			//string [] events;
			string eventsRoot = null;
			
			if (depot == null) {
				depot =  iniget("depots", "_depot");
				if (depot != null) {
					eventsRoot = Directory.GetCurrentDirectory();
				}
				else {
					console.DoLog("on doit d abord définir un dépot");
					return;
				}
				
			}

			eventsRoot =  iniget("depots", depot);

			//
			if (label == null) {
				// retour a la racine du depot
				iniwrite(depot, "_event", null);
				string _labelDepot = iniget("depots", "_depot");
				string _path  = iniget("depots", _labelDepot);
				if (_path != null) {
					System.IO.Directory.SetCurrentDirectory(_path);
					console.DoPromptDepot(_labelDepot);
				}
				return;				
			}
			else if (label.StartsWith("last")) {
//					var _event = events.FirstOrDefault();
					var eventRelPath = iniget(depot, "_event");

					//iniwrite("events", "_event", eventRelPath);
					
					//inidata["config"][key] = value;
					string eventAbsolutePath = Path.Combine(eventsRoot,eventRelPath.ToString());
					System.IO.Directory.SetCurrentDirectory(eventAbsolutePath);
					console.DoPromptEvent(iniget("depots", "_depot") + ":" + eventRelPath);
					return;
				
			}
			else label = "*" + label + "*";
			
			// events
			if (eventsRoot != null) {
				
//				var events = Parallel.ForEach(Directory.EnumerateDirectories(eventsRoot), file=> {
//				        
//				                 	console.DoLog(file.Substring(file.LastIndexOf("\\") + 1));
//				  });
//				if (label == null) label="*";
				//else label = "*" + label + "*";		
				
				var events = from eventdir in Directory.EnumerateDirectories(eventsRoot, label, SearchOption.AllDirectories).AsParallel()
					select eventdir;
				
				var eventslist = events.ToList();
				if (eventslist.Count() > 1 ) {
					foreach(var _event in eventslist ) {
							// affiche la différence d'event
							var eventRelPath = ShellUtils.Utils.MakeRelativePath( eventsRoot , _event);
							console.DoLog(eventRelPath);
							 
					}					
				}
				else {
					var _event = events.FirstOrDefault();
					var eventRelPath = ShellUtils.Utils.MakeRelativePath( eventsRoot , _event);

					//iniwrite("events", "_event", eventRelPath);
					
					//inidata["config"][key] = value;
					iniwrite(iniget("depots", "_depot"), "_event", eventRelPath);
					
					System.IO.Directory.SetCurrentDirectory(_event.ToString());
					console.DoPromptEvent(iniget("depots", "_depot") + ":" + eventRelPath);
						
				}
				
			
//				}

			}
			//for (int i = 0; i < count; i++)
			//{
			//    Console.WriteLine("variable {0} = {1}", _key, _value);
			//}
		}	
		
		[Verb]
		public  void List([AliasesAttribute("arg0")] string label, [AliasesAttribute("d")] string depot)
		{
			//            shellControl1.WriteText("list");
			// string [] events;
			string eventsRoot = null;
			
			if (depot == null) {
				eventsRoot = Directory.GetCurrentDirectory();
			}
			else {
				eventsRoot =  iniget("depots", depot);
			}
			if (label == null) label="*";
			//else if (label. '..'
			//else label = "*" + label + "*";
			// events
			if (eventsRoot != null) {
//				Parallel.ForEach(Directory.EnumerateDirectories(eventsRoot), file=> {
//				        
//				                 	console.DoLog(file.Substring(file.LastIndexOf("\\") + 1));
//				  });
				var events = from eventdir in Directory.EnumerateDirectories(eventsRoot, label, SearchOption.AllDirectories).AsParallel()
					let _event = Path.GetFileName(eventdir)
					let _eventsel = _event.Substring(_event.LastIndexOf("_") + 1)
					select _eventsel;
				
				foreach(var _event in events ) {
					console.DoLog(_event);
				}
				
			}

			//for (int i = 0; i < count; i++)
			//{
			//    Console.WriteLine("variable {0} = {1}", _key, _value);
			//}
		}
		[Verb(Description="add name --from")]
		public void add([AliasesAttribute("arg0")] string _name, [AliasesAttribute("f")] string from, [AliasesAttribute("t")] string to)
		{
		
			
			
			string eventName =  getEventName(_name, from, to);
			
			console.DoLog("event add  " + eventName );
			string depot = iniget("depots","_depot");
			console.DoLog("event event depot  " + depot );
			string eventRootPath = getEventRootPath(iniget("depots", depot), eventName, from, to);
			console.DoLog("event event root path  " + eventRootPath );
			createEvent(eventRootPath, eventName, from, to);
			
			
		}
		[Verb(Description="set attb value --event")]
		public void set([AliasesAttribute("arg0")] string _key, [AliasesAttribute("arg1")] string value)
		{
			

				// si  null -> list toutes les proprietees de l event
				string depot = iniget("depots","_depot");
				string _eventName =  iniget(depot,"_event");
				getEventSet(iniget("depots", depot), _eventName, _key, value);
				

			
			
		}
		[Verb(Description="get attb --event")]
		public void get([AliasesAttribute("arg0")] string _key, [AliasesAttribute("e")] string _event)
		{
			
				string depot = iniget("depots","_depot");
				
				// si  null -> list toutes les proprietees de l event
				if (_event == null) {
					_event =  iniget(depot,"_event");
				}
				
				string value = getEventGet(iniget("depots", depot), _event, _key);
				
				console.DoLog( _key + " = " + value );
			
			
		}
		[Verb(Description="import  --event")]
		public void import([AliasesAttribute("arg0")] string _files, [AliasesAttribute("e")] string _event)
		{
			
				string depot = iniget("depots","_depot");
				string depotRoot = null;
				
				depotRoot =  iniget("depots", iniget("depots", "_depot"));
				
				// si  null -> list toutes les proprietees de l event
				if (_event == null) {
					_event =  iniget(depot,"_event");
				}
				else {
						// search event
				}
				string [] files =  Directory.GetFiles(@_files);
				console.DoLog("event import   " + _files );
				if (files != null) {
					foreach(string _file in files) {
						console.DoLog("event import file  " + _file );
						ExifReader reader = new ExifReader(@_file);
						DateTime datePictureTaken;
					    if (reader.GetTagValue<DateTime>(ExifTags.DateTimeOriginal, 
					                                    out datePictureTaken))
					    {
					        // Do whatever is required with the extracted information
					        
					        console.DoLog(string.Format("The picture was taken on {0}", 
					           datePictureTaken));
					        
					        String importTo = lookupEvent(_event, datePictureTaken );

					        var eventRelPath = ShellUtils.Utils.MakeRelativePath( depotRoot , importTo);

							// get Relative path
							string targetName = null;
							// start with creationDate
							
							targetName = datePictureTaken.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss") + "_";
							string[] strPaths = eventRelPath.Split(Path.DirectorySeparatorChar);
							 for (int count = 0; count <= strPaths.Length - 1; count++)
					         {
							 	if (count > 0 ) targetName += "-";
							 	if (strPaths[count].Length > 0) {
							 		targetName +=  strPaths[count].Split('_')[1]  ;
							 	}
					         }
							
							// target file name start with yearmmdate_norvege_jour1_IM031.jpg
							targetName += "_" + Path.GetFileName(_file);
							File.Copy(_file, Path.Combine(importTo, targetName), true);
							
						}


					}
				}
			
			
		}			
		private bool createEvent(string eventRootPath, string _name, string _from, string _to) {
			bool result = false;

			
			// create dir
			DirectoryInfo eventpath = Directory.CreateDirectory(eventRootPath + Path.DirectorySeparatorChar + _name);
			if (eventpath != null) {
				// create inifile
				strINIEventPath = eventpath.FullName + Path.DirectorySeparatorChar + _name +".ini";
				inieventfiledata = new FileIniDataParser();
				if (File.Exists(strINIEventPath)) {
						inieventdata = inieventfiledata.ReadFile(strINIEventPath);
				}
				else 	{
				 	inieventfiledata.Parser.Configuration.CommentString = "#";
				 	inieventdata = new IniData();
				}
				inieventdata.Sections.AddSection("event");				
				inieventdata.Sections.GetSectionData("event").Keys.AddKey("from", _from);
				if (_to != null) {
					inieventdata.Sections.GetSectionData("event").Keys.AddKey("to", _to);
					
				}
				inieventfiledata.WriteFile(strINIEventPath, inieventdata);					
					
					

				result = true;
			}
			return result;
		}
		private string getEventName(string _name, string _from, string _to) {
			string eventName = null;
			eventName = _name;
			if (_from != null) {
				DateTime fromDate = DateTime.Parse(_from);
				eventName = String.Format("{0:yyyyMMdd_}", fromDate) + _name;
			}
			return eventName;
		}

		
		private string lookupEvent(string _event, DateTime _from ) {
			
			string eventsRoot = null;

//			if (_event == null) {
				// on commence à la racine
				eventsRoot =  iniget("depots", iniget("depots", "_depot"));

//			}
//			else {
				// on recherche l'event 
				
				// si event non créé on le créé
//			}

			//Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

			string eventRootPath = getEventRootPathFast(eventsRoot, _event, _from.ToShortDateString() , null);
			console.DoLog("LookupEvent : event root path " +  eventRootPath);
				              
			return eventRootPath;	
			// return ShellUtils.Utils.MakeRelativePath(eventsRoot, eventRootPath);
			
			
		}
		
		private string getEventRootPathFast(string eventRootPath, string _eventname, string _from, string _to) {
			
			string eventMatched = eventRootPath;
			
			Func<string, bool> checkDate = (x) => {  return matchEventDates(x, _from); };
			var events = (from eventdir in Directory.EnumerateDirectories(eventRootPath, "*",SearchOption.AllDirectories)
					
				//where eventdir => myFunc(eventdir)
				let eventFrom = getEventGet(eventdir,"from")
				where checkDate(eventdir)

				select new Event { Name = _eventname = eventdir, From = DateTime.Parse(eventFrom)} ).OrderByDescending(c=>c.From);
				
				console.DoLog( "eventMatchFast (" + _from + ") start ");
				
//				var eventslist = events.ToList().FirstOrDefault();
				var eventslist = events.ToList();
				
				
				if (eventslist.Count() > 1 ) {
					foreach(var _event in eventslist ) {
							// affiche la différence d'event
							var eventRelPath = ShellUtils.Utils.MakeRelativePath( eventRootPath , _event.Name);
							console.DoLog( "eventMatchFast (" + _from + ") "  + _event.From + eventRelPath  );
						    eventMatched = _event.Name; 
					}					
				}
			
			
				console.DoLog( "eventMatchFast (" + _from + ")  return " + eventslist.FirstOrDefault().Name );
			
				return eventslist.FirstOrDefault().Name;
		}
		private string getEventRootPath(string eventRootPath, string _eventname, string _from, string _to) {
			
//				var events = from eventdir in Directory.EnumerateDirectories(eventsRoot, "*", SearchOption.AllDirectories).AsParallel()
//					select eventdir;
				
//				var eventslist = events.ToList();			
			
			
			
			
			string prevEvent = eventRootPath;
			string [] events =  Directory.GetDirectories(eventRootPath);
			console.DoLog("event getEventRootPath   " + eventRootPath );
			if (events != null) {
				foreach(string _event in events) {
					console.DoLog("event getEventRootPath in events eventRootPath  " + eventRootPath );

					console.DoLog("event getEventRootPath  in events _event  " + _event );

					var eventRelPath = ShellUtils.Utils.MakeRelativePath( eventRootPath , _event);
					console.DoLog("event getEventRootPath  _event  " + eventRelPath );
					console.DoLog("event getEventRootPath  _event from  " + getEventGet(eventRootPath, eventRelPath, "from") );

					DateTime eventfolderfromdt = DateTime.ParseExact(getEventGet(eventRootPath, eventRelPath, "from"),"dd/MM/yyyy", CultureInfo.InvariantCulture);

					DateTime eventfrom = DateTime.ParseExact(_from,"dd/MM/yyyy",CultureInfo.InvariantCulture);
					if (DateTime.Compare(eventfolderfromdt, eventfrom) <= 0  ) {
						// 
						string eventfolderto = getEventGet(eventRootPath, eventRelPath, "to");

						if (eventfolderto != null ) {
							DateTime eventfoldertodt = DateTime.ParseExact(eventfolderto,"dd/MM/yyyy",null);

							if (DateTime.Compare(eventfoldertodt, eventfrom) >= 0) {
					
								return _event;
							}
						}
						else {
							return prevEvent;
						}
					}
					else {
						prevEvent = _event;
					}
					console.DoLog(_event);
				}				
			}
			
			return eventRootPath;
		}
		private bool matchEventDates(string _event, string _date) {
			var matched = false;
					var eventRootPath = _event;
					//console.DoLog("matchEventDate getEventRootPath  in events _event  " + _event );

					var eventRelPath = ShellUtils.Utils.MakeRelativePath( eventRootPath , _event);
					//console.DoLog("matchEventDate getEventRootPath  _event  " + eventRelPath );
					//console.DoLog("matchEventDate getEventRootPath  _event from  " + getEventGet(eventRootPath, eventRelPath, "from") );

					DateTime eventfolderfromdt = DateTime.ParseExact(getEventGet(eventRootPath, eventRelPath, "from"),"dd/MM/yyyy", CultureInfo.InvariantCulture);

					DateTime eventfrom = DateTime.ParseExact(_date,"dd/MM/yyyy",CultureInfo.InvariantCulture);
					if (DateTime.Compare(eventfolderfromdt, eventfrom) <= 0  ) {
						// 
						string eventfolderto = getEventGet(eventRootPath, eventRelPath, "to");

						if (eventfolderto != null ) {
							DateTime eventfoldertodt = DateTime.ParseExact(eventfolderto,"dd/MM/yyyy",null);

							if (DateTime.Compare(eventfoldertodt, eventfrom) >= 0) {
					
								return true;
							}
						}
						else {
							return false;
						}
					}
					else {
						return false;
					}




			
			return matched;
		}
		private void getEventSet(string eventRootPath, string _name, string _key, string _value) {
			
			string section = "event";

			string eventAbsolutePath = Path.Combine(eventRootPath,_name);
			
			strINIEventPath = eventAbsolutePath + Path.DirectorySeparatorChar + Path.GetFileName(eventAbsolutePath) +".ini";
			
			inieventfiledata = new FileIniDataParser();
			if (File.Exists(strINIEventPath)) {
					inieventdata = inieventfiledata.ReadFile(strINIEventPath);
					var eventSection = inieventdata.Sections.GetSectionData(section);
					
					if (_key == null) {
						foreach(KeyData key in eventSection.Keys) {
							console.DoLog(key.KeyName + " = " + key.Value);
						}
					}
					else {
						if (_value == null) {
							foreach(KeyData key in eventSection.Keys) {
								if (key.KeyName.StartsWith(_name)) console.DoLog(key.KeyName + " = " + key.Value);
							}
						}
						else {
							if (inieventdata.Sections.GetSectionData(section).Keys.ContainsKey(_key) ) {
								inieventdata[section][_key] = _value;
							}
							else {
								inieventdata.Sections.GetSectionData(section).Keys.AddKey(_key, _value);
							}
							inieventfiledata.WriteFile(strINIEventPath.ToString(), inieventdata);
							console.DoLog(_key + " = " + inieventdata[section][_key]);

							
						}
					}

			}
		}
		private string getEventGet(string eventRootPath, string _name, string _key) {
			
			string section = "event";

			string eventAbsolutePath = Path.Combine(eventRootPath,_name);
			//console.DoLog("getEventGet " + eventAbsolutePath );
			
			strINIEventPath = eventAbsolutePath + Path.DirectorySeparatorChar + Path.GetFileName(eventAbsolutePath) +".ini";
			//console.DoLog("getEventGet eventAbsolutePath " + strINIEventPath );
			
			inieventfiledata = new FileIniDataParser();
			if (File.Exists(strINIEventPath)) {
					inieventdata = inieventfiledata.ReadFile(strINIEventPath);
					var eventSection = inieventdata.Sections.GetSectionData(section);
					
					if (_key != null) {
						foreach(KeyData key in eventSection.Keys) {
							if (key.KeyName.StartsWith(_key)) {
								return inieventdata[section][key.KeyName];
							}
						}
					}

			}
			return null;
		}
		private string getEventGet(string _name, string _key) {
			
			string section = "event";

			string eventAbsolutePath = _name;
			//console.DoLog("getEventGet " + eventAbsolutePath );
			
			strINIEventPath = eventAbsolutePath + Path.DirectorySeparatorChar + Path.GetFileName(eventAbsolutePath) +".ini";
			//console.DoLog("getEventGet eventAbsolutePath " + strINIEventPath );
			
			inieventfiledata = new FileIniDataParser();
			if (File.Exists(strINIEventPath)) {
					inieventdata = inieventfiledata.ReadFile(strINIEventPath);
					var eventSection = inieventdata.Sections.GetSectionData(section);
					
					if (_key != null) {
						foreach(KeyData key in eventSection.Keys) {
							if (key.KeyName.StartsWith(_key)) {
								return inieventdata[section][key.KeyName];
							}
						}
					}

			}
			return null;
		}		
		private string getEventFrom(string eventRootPath, string _event) {
			string eventFrom;
			
			//IAbsolutePath _eventAbsolutePath = Path.GetFullPath(_event);
			IDirectoryPath _eventAbsolutePath = _event.ToAbsoluteDirectoryPath();
			//var _eventAbsolutePath = _event.ToAbsolutePath();
			eventFrom = _eventAbsolutePath.DirectoryName.Split('_')[0];
			return eventFrom;
		}
		private string getEventFrom(string _event) {
			string eventFrom;
			
			//IAbsolutePath _eventAbsolutePath = Path.GetFullPath(_event);
			IDirectoryPath _eventAbsolutePath = _event.ToAbsoluteDirectoryPath();
			//var _eventAbsolutePath = _event.ToAbsolutePath();
			eventFrom = _eventAbsolutePath.DirectoryName.Split('_')[0];
			return eventFrom;
		}			
	}

}
