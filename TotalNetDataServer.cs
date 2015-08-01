/*
 * Создано в SharpDevelop.
 * Пользователь: V.Seminchenko
 * Дата: 13.07.2015
 * Время: 11:07
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Net;
using System.Net.Configuration;
using System.ComponentModel;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;

namespace Total.Net
{
	
	public delegate string ReceiveCallback(string data);
	public delegate void EventContainer();
	
	
	/// <summary>
	/// Реализует сервер передачи данных через TCP
	/// </summary>
	///
	public class DataServer
	{
		TcpListener listner;
		bool serverStopt = false;
		List<ConnectedClient> clients;
		ReceiveCallback callback;
		Thread runThread;
		
		public event EventContainer onServerStop;
		
		public ReceiveCallback ClientReceiveCallback {
			get{ return callback; }
			set{ callback = value; }
		}
		
		
		public  DataServer(int port)
		{
			listner = new TcpListener(IPAddress.Any, port);
			clients = new List<ConnectedClient>();
			listner.Stop();
			listner.Start();
			
		}
		
		public void Start()
		{
			if (runThread == null) {
				runThread = new Thread(new ThreadStart(RuningThread));
				runThread.Start();
			}
		}
		
		public void RuningThread()
		{
			try{
			while (true) {
				TcpClient client = listner.AcceptTcpClient();
				if (serverStopt) {
					break;
				}
				// Создаем поток
				Thread tr = new Thread(new ParameterizedThreadStart(ClientThread));
				tr.Start(client);
				
			}
			}catch(Exception e){
				if ((e as Win32Exception).ErrorCode != 10004) {
				MessageBox.Show(e.Message+"\n"+e.Source);	
				}
			}
		}
		
		public void Stop()
		{
			serverStopt = true;
			
			if (listner != null) {
				listner.Stop();
			}
			// И вызываем событие отановки сервера и все подписаные методы в Классах-потоках ConnectedClient
			if (onServerStop != null) {
				onServerStop();
			}
		}
		
		private void ClientThread(Object StateInfo)
		{
			var cln = new ConnectedClient((TcpClient)StateInfo);
			cln.ClientReceiveCallback = this.callback;
			this.onServerStop += cln.StopClientConnection;
			cln.Start();
		}
	}
	
	public class ConnectedClient
	{
		TcpClient connectedClient;
		ReceiveCallback callback;
		
		public ReceiveCallback ClientReceiveCallback {
			get{ return callback; }
			set{ callback = value; }
		}
		
		public ConnectedClient(TcpClient client)
		{
			connectedClient = client;
		}
		
		public void Start()
		{
			byte[] bufer = new byte[4096];
			int count;
			bool closeConnection = false;
			string request;
			try {
				
				while (!(closeConnection)) {
					count = connectedClient.GetStream().Read(bufer, 0, bufer.Length);
					request = Encoding.ASCII.GetString(bufer, 0, count);
					
					if (request == "CLOSE") {
						byte[] buffer = Encoding.ASCII.GetBytes("CLOSING");
						connectedClient.GetStream().Write(buffer, 0, buffer.Length);
						closeConnection = true;
					} else {
						byte[] buffer = Encoding.ASCII.GetBytes(callback(request));
						connectedClient.GetStream().Write(buffer, 0, buffer.Length);
					}
				}
				
				connectedClient.Close();
				
			} catch (Exception e) {
				MessageBox.Show(e.Message+"\n"+e.Source);	
			}
		}
		
		public void Stop()
		{
			if (connectedClient != null) {
				connectedClient.Close();
			}
		}
		
		public void StopClientConnection()
		{
			this.Stop();
		}
	}
}
