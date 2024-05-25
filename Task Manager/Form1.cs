using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using System.Diagnostics;


namespace Task_Manager
{
	public partial class MainForm : System.Windows.Forms.Form
	{

		private Dictionary<int, Process> d_processes;

		public MainForm()
		{
			InitializeComponent();
			d_processes = new Dictionary<int, Process>();

			listViewProcesses.Columns.Add("Name");
			listViewProcesses.Columns.Add("PID");
			listViewProcesses.Columns.Add("CPU");
			listViewProcesses.Columns.Add("RAM");
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			Process[] processList = Process.GetProcesses();
			List<int> currentProcessIds = processList.Select(p => p.Id).ToList();

			if (!currentProcessIds.SequenceEqual(this.d_processes.Keys.ToList()))
			{
				listViewProcesses.BeginUpdate();
				listViewProcesses.Items.Clear();

				foreach (var process in processList)
				{
					try
					{
						//var cpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
						var ramUsage = process.PrivateMemorySize64 / 1048576.0;

						//cpuCounter.NextValue();
						//float cpuUsage = cpuCounter.NextValue() / Environment.ProcessorCount;

						ListViewItem item = new ListViewItem(process.ProcessName);
						item.SubItems.Add(process.Id.ToString());
						item.SubItems.Add(/*{cpuUsage:F2} */"%");
						item.SubItems.Add($"{ramUsage:F2} MB");

						listViewProcesses.Items.Add(item);
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Error accessing process {process.ProcessName}: {ex.Message}");
					}
				}

				listViewProcesses.EndUpdate();
				d_processes = currentProcessIds.ToDictionary(id => id, id => processList.FirstOrDefault(p => p.Id == id));
			}


			this.Text = $"Task Manager - {currentProcessIds.Count} Processes";
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool FreeConsole();
	}
}
