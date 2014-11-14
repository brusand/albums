using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Text;
//using System.EventArgs;

using UILibrary;
//using ShellAlbums;
using CLAP;
using ShellCommands;

namespace ShellConsoleApp
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public delegate void ConsoleEventHandler(object sender, ConsoleEventArgs e);

	public class ConsoleEventArgs : System.EventArgs {
		// Provide one or more constructors, as well as fields and
		// accessors for the arguments.
		public ConsoleEventArgs(string _line) {
			this.line = _line;
		}
		public string line { get; set; }
	}
	
	[System.ComponentModel.DefaultEvent("ConsoleLogEvent")]
	public class ConsoleLog {

		public event ConsoleEventHandler ConsoleLogEvent;
		public event ConsoleEventHandler ConsoleDepotPromptEvent;
		public event ConsoleEventHandler ConsoleEventPromptEvent;

		public virtual void OnConsoleLogEvent(ConsoleEventArgs e) {
			if (ConsoleLogEvent != null) ConsoleLogEvent(this, e);
		}
		
		public void DoLog(string line) {
			OnConsoleLogEvent(new ConsoleEventArgs(line + "\r\n" ) );
		}
		public void DoPromptDepot(string depotPrompt) {
			ConsoleDepotPromptEvent(this, new ConsoleEventArgs(depotPrompt) );
		}
		public void DoPromptEvent(string eventPrompt) {
			ConsoleEventPromptEvent(this, new ConsoleEventArgs(eventPrompt) );
		}
	}

	public class ShellForm : System.Windows.Forms.Form
	{
		UILibrary.ShellControl shellControl1;
		private string helpText;
		ConsoleLog console;
		//Parser parser;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		public ShellForm()
		{
			//
			// Required for Windows Form Designer support
			//
			//parser = new Parser();
			//parser = new Parser<ConfigApp, DepotsApp>();
			console = new ConsoleLog();
			console.ConsoleLogEvent += new ConsoleEventHandler(this.form_Console);
			console.ConsoleDepotPromptEvent += new ConsoleEventHandler(this.form_depot_prompt);
			console.ConsoleEventPromptEvent += new ConsoleEventHandler(this.form_event_prompt);


			InitializeComponent();

			shellControl1.CommandEntered += new UILibrary.EventCommandEntered(shellControl1_CommandEntered);

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ShellControl Demo");
			stringBuilder.Append(System.Environment.NewLine);
			stringBuilder.Append("*******************************************");
			stringBuilder.Append(System.Environment.NewLine);
			stringBuilder.Append("Commands Available");
			stringBuilder.Append(System.Environment.NewLine);
			stringBuilder.Append("(1) All DOS commands that operate on a single line");
			stringBuilder.Append(System.Environment.NewLine);
			stringBuilder.Append("(2) prompt - Changes prompt. Usage (prompt=<desired_prompt>");
			stringBuilder.Append(System.Environment.NewLine);
			stringBuilder.Append("(3) history - prints history of entered commands");
			stringBuilder.Append(System.Environment.NewLine);
			stringBuilder.Append("(4) cls - Clears the screen");
			stringBuilder.Append(System.Environment.NewLine);

			helpText = stringBuilder.ToString();
			int status = Parser.RunWinForms<ConfigApp, DepotsApp>("depots sel _depot".Split(new char[] { ' ' }), new ConfigApp(console), new DepotsApp(console)  );
			status = Parser.RunWinForms<ConfigApp, DepotsApp, EventApp>("event sel last".Split(new char[] { ' ' }), new ConfigApp(console), new DepotsApp(console), new EventApp(console) );

		}
		
		private void form_Console(object sender, ConsoleEventArgs e) {
			shellControl1.WriteText(e.line);
		}
		private void form_depot_prompt(object sender, ConsoleEventArgs e) {
			this.shellControl1.Prompt = e.line + ":>>>";
			//shellControl1.WriteText(this.shellControl1.Prompt);
			
			//shellControl1.WriteText("\r\n");
		}
		private void form_event_prompt(object sender, ConsoleEventArgs e) {
			this.shellControl1.Prompt = e.line + ">>>";
			//shellControl1.WriteText(this.shellControl1.Prompt);
			//shellControl1.WriteText("\r\n");
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.shellControl1 = new UILibrary.ShellControl();
			this.SuspendLayout();
			// 
			// shellControl1
			// 
			this.shellControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.shellControl1.Location = new System.Drawing.Point(0, 0);
			this.shellControl1.Name = "shellControl1";
			this.shellControl1.Prompt = ">>>";
			this.shellControl1.ShellTextBackColor = System.Drawing.Color.Black;
			this.shellControl1.ShellTextFont = new System.Drawing																																																																																																																																	.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.shellControl1.ShellTextForeColor = System.Drawing.Color.LimeGreen;
			this.shellControl1.Size = new System.Drawing.Size(360, 269);
			this.shellControl1.TabIndex = 0;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(360, 269);
			this.Controls.Add(shellControl1);
			this.Name = "ShellConsole";
			this.Text = "ShellControl App";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.Run(new ShellForm());
		}

		void shellControl1_CommandEntered(object sender, UILibrary.CommandEnteredEventArgs e)
		{
			string command = e.Command;

			if (!ProcessInternalCommand(command))
			{
				ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
				startInfo.Arguments = "/C " + e.Command;
				startInfo.RedirectStandardError = true;
				startInfo.RedirectStandardOutput = true;
				startInfo.UseShellExecute = false;
				startInfo.CreateNoWindow = true;
				Process p = Process.Start(startInfo);
				string output = p.StandardOutput.ReadToEnd();
				string error = p.StandardError.ReadToEnd();

				p.WaitForExit();
				if (output.Length != 0)
					shellControl1.WriteText(output);
				else if (error.Length != 0)
					shellControl1.WriteText(error);
			}
		}

		private bool ProcessInternalCommand(string command)
		{
//			if (command == "cls")
//				shellControl1.Clear();
//			else if (command == "history")
//			{
//				string []commands = shellControl1.GetCommandHistory();
//				StringBuilder stringBuilder = new StringBuilder(commands.Length);
//				foreach(string s in commands)
//				{
//					stringBuilder.Append(s);
//					stringBuilder.Append(System.Environment.NewLine);
//				}
//				shellControl1.WriteText(stringBuilder.ToString());
//			}
//			else if (command == "help")
//			{
//				shellControl1.WriteText(GetHelpText());
//
//			}
//			else if (command.StartsWith("prompt"))
//			{
//				string[] parts = command.Split(new char[] { '=' });
//				if (parts.Length == 2 && parts[0].Trim() == "prompt")
//					shellControl1.Prompt = parts[1].Trim();
//			}
//			else
//				return false;
//
//			return true;

			int status = Parser.RunWinForms<ConfigApp, DepotsApp, EventApp>(command.Split(new char[] { ' ' }), new ConfigApp(console), new DepotsApp(console), new EventApp(console)  );
			
			//int status = Parser.RunWinForms<ConfigApp, DepotsApp>(command.Split(new char[] { ' ' }));
			
			//shellControl1.WriteText("status " + status);
			if (status == 0) return true;
			else return false;
			
		}

		private string GetHelpText()
		{
			return helpText;
		}
		
		
//		public class BaseApp
//		{
//			
//			public ConsoleLog console;
//
//			public BaseApp(ConsoleLog _console) {
//				console = _console;
//			}
//			public BaseApp() {
//				console = null;
//			}			
//			[Error]
//			public static void Error(ExceptionContext ex)
//			{
//				//            shellControl1.ForegroundColor = ConsoleColor.Red;
//				//            shellControl1.WriteText(ex.Exception.Message);
//				//console.ResetColor();
//			}
//
//			[Empty]
//			[Help(Aliases = "h,?")]
//			public static void Help(string help)
//			{
//				//            shellControl1.WriteText(help);
//			}
//
//			//        [Global]
//			//        public static void Debug()
//			//        {
//			//            Debugger.Launch();
//			//        }
//			//       public lo
//
//		}
//		
//		[TargetAlias("config")]
//		public class ConfigApp : BaseApp {
//			
//			//private event ConsoleEventHandler ConsoleLogEvent;
//			public ConfigApp(ConsoleLog consoleLog) : base (consoleLog) {
//
//            }
//
//			[Verb]
//			public void List()
//			{
//				//            shellControl1.WriteText("list");
//
//				//for (int i = 0; i < count; i++)
//				//{
//				//    Console.WriteLine("variable {0} = {1}", _key, _value);
//				//}
//			}
//			
//			[Verb(Description="set key value")]
//			public void set([AliasesAttribute("arg0")] string key, [AliasesAttribute("arg1")] string value)
//			{
//				//for (int i = 0; i < count; i++)
//				//{
//				//                shellControl1.WriteText("Config set {0} = {1}"+ key+ '-'+ value);
//				//}
//				//BaseApp.logConsole("config set");
//
//				// ConsoleLogEvent(new Object(), new ConsoleEventArgs());
//				console.DoLog("config log set ");
//			}
//		
////			static protected void OnConsoleEvent(ConsoleEventArgs e)
////			{
////
////				if (ConsoleEvent == null) {
////					ConsoleLog consoleLog = new ConsoleLog();
////					
////					ConsoleEvent += new ConsoleEventHandler(consoleLog.OnConsole);
////				}
////				if(ConsoleEvent!=null) {
////					//ConsoleEvent(new Object(), e);
////					ConsoleLog.DoLog("config");
////				}
////			}
//		}
//		
//		[TargetAlias("depots")]
//		public class DepotsApp : BaseApp
//		{
//			public DepotsApp(ConsoleLog consoleLog) : base (consoleLog) {
//				
//			}
//			[Verb]
//			public static void List()
//			{
//				//            shellControl1.WriteText("list");
//
//				//for (int i = 0; i < count; i++)
//				//{
//				//    Console.WriteLine("variable {0} = {1}", _key, _value);
//				//}
//			}
//			[Verb(Description="set key value")]
//			public static void set([AliasesAttribute("arg0")] string key, [AliasesAttribute("arg1")] string value)
//			{
//				//for (int i = 0; i < count; i++)
//				//{
//				//                shellControl1.WriteText("Depot set {0} = {1}" + key +'/'+ value);
//				//}
//			}
//		}
	}

}
