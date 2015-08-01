/*
 * Создано в SharpDevelop.
 * Пользователь: V.Seminchenko
 * Дата: 08.07.2015
 * Время: 13:34
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Configuration;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;

namespace Total.Net
{
	/// <summary>
	/// Эхо сервер на udp
	/// </summary>
	///
	
	public class EchoServer
	{
		
		UdpClient udp;
		UdpClient echoUdp;
		int currentPort;
		Thread srvtr;
		bool serverStop = true;
		bool echoStop = true;
		SortedList<string,string> states = new SortedList<string, string>();
		int timeout = 1000;
		System.Threading.Timer tmr;
		
		public int EchoTimeout {
			get{ return timeout; }
			set {
				if ((value > 0)) {
					timeout = value;
				}
			}
		}
		
		public bool isServerRun {
			get { return !(serverStop); }
		}
		
		public string ServerState {
			get {
				if (states.IndexOfKey("STATE") != -1) {
					return states["STATE"];
				} else {
					return "UNDEFINED";
				}
			}
			set {
				states["STATE"] = value;
			}
		}
		
		public SortedList<string,string> EchoTable {
			get {
				return states;
			}
		}
		
		public void SetPairToEchoTable(string key, string value)
		{
			if (states.IndexOfKey(key) >= 0) {
				states[key] = value;
			} else {
				states.Add(key, value);
			}
		}
		
		public EchoServer(int port)
		{
			try {
				udp = new UdpClient();
				currentPort = port;
				states.Add("STATE", "WAIT");
				
			} catch (Exception e) {
				MessageBox.Show(e.Message + "\n" + e.Source);
			}
			
		}
		
		
		private void ServerThread()
		{
			try {
				if (udp != null) {
					udp.Close();
				}
				
				udp = new UdpClient(currentPort);
				
				while (true) {
					
					IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Any, currentPort);
					Byte[] message = udp.Receive(ref ipendpoint);
					
					if (serverStop) {
						break;
					}
					
					string returnData = Encoding.ASCII.GetString(message);
					if (states.IndexOfKey(returnData) >= 0) {
						Byte[] sendBytes = Encoding.ASCII.GetBytes(states[returnData]);
						udp.Send(sendBytes, sendBytes.Length, ipendpoint);
					} else {
						Byte[] sendBytes = Encoding.ASCII.GetBytes("UNDEFINED");
						udp.Send(sendBytes, sendBytes.Length, ipendpoint);
					}
					
				}
			} catch (Exception e) {	
				if ((e as Win32Exception).ErrorCode != 10004) {
					MessageBox.Show(e.Message + "\n" + e.Source);	
				}
			}
		}
		
		public void StartServer()
		{
			srvtr = new Thread(new ThreadStart(ServerThread));
			serverStop = false;
			srvtr.Start();
		}
		
		public void StopServer()
		{
			serverStop = true;
			try {
				UdpClient stoper = new UdpClient(new IPEndPoint(IPAddress.Loopback, currentPort));
				Byte[] sendBytes = Encoding.ASCII.GetBytes(" ");
				//stoper.Send(sendBytes, sendBytes.Length);
				if (udp != null) {
					udp.Close();
				}
			} catch (Exception e) {
				MessageBox.Show(e.Message + "\n" + e.Source);
			}
		}
		
		private void StopEcho(object state)
		{
			echoStop = true;
			try {
				tmr.Change(-1, -1);
				if (echoUdp != null) {
					echoUdp.Close();
				}
			} catch (Exception e) {
				MessageBox.Show(e.Message + "\n" + e.Source);
			}
		}
		
		public List<IPEndPoint> Echo(string key, string value)
		{
			List<IPEndPoint> lst = new List<IPEndPoint>();
			try {
				IPEndPoint endpnt = new IPEndPoint(IPAddress.Broadcast, currentPort);
				echoUdp = new UdpClient();
				
				Byte[] sendBytes = Encoding.ASCII.GetBytes(key);
				echoUdp.Send(sendBytes, sendBytes.Length, endpnt);
				
				TimerCallback tmrCallback = new TimerCallback(StopEcho);
				tmr = new System.Threading.Timer(tmrCallback, null, timeout, -1);
				
				while (true) {
					endpnt = new IPEndPoint(IPAddress.Any, currentPort);
					Byte[]	receiveBytes = echoUdp.Receive(ref endpnt);
					
					/*if (echoStop) {
						break;
					}*/
					
					String msg = Encoding.ASCII.GetString(receiveBytes);
					if (msg == value) {
						if (lst.IndexOf(endpnt) < 0) {
							lst.Add(endpnt);
						}
					}
					
				}
			} catch (Exception e) {
				if ((e as Win32Exception).ErrorCode != 10004) {
					MessageBox.Show(e.Message + "\n" + e.Source);	
				}

			}
			return lst;
		}
		
		
		public List<IPEndPoint> Echo()
		{
			return Echo("STATE", "OK");
		}
		
		public string Echo(IPEndPoint server, string key)
		{
			String ech = "";
			try {
				echoUdp = new UdpClient();
				
				Byte[] sendBytes = Encoding.ASCII.GetBytes(key);
				echoUdp.Send(sendBytes, sendBytes.Length, server);
				
				TimerCallback tmrCallback = new TimerCallback(StopEcho);
				tmr = new System.Threading.Timer(tmrCallback, null, timeout, -1);
				Byte[]	receiveBytes = echoUdp.Receive(ref server);
				
				/*if (echoStop) {
						break;
					}*/
				
				String msg = Encoding.ASCII.GetString(receiveBytes);
				ech = msg;

				
			} catch (Exception e) {
				MessageBox.Show(e.Message + "\n" + e.Source);
			}
			return ech;
		}
	}
	
}