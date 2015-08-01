/*
 * Создано в SharpDevelop.
 * Пользователь: V.Seminchenko
 * Дата: 15.07.2015
 * Время: 10:50
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Net;
using System.IO;
using System.Net.Configuration;
using System.ComponentModel;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;

namespace Total.Net
{
	/// <summary>
	/// Реализует клиент передачи данных через TCP
	/// </summary>
	public class DataClient
	{
		//int currentPort;
		TcpClient client;
		ReceiveCallback callback;
		bool sending = false;
		Thread sendthread;
		
		public bool Sending {
			get{ return sending; }
			set{ sending = value; }
		}
		
		public ReceiveCallback ClientReceiveCallback {
			get{ return callback; }
			set{ callback = value; }
		}
		
		public DataClient()
		{
			//currentPort = port;
			client = new TcpClient();
		}
		
		public bool Connect(IPEndPoint adress)
		{
			client.Connect(adress);
			return client.Connected;
		}
		
		public bool Disconnect()
		{
			Send("CLOSE");
			client.Close();
			return !(client.Connected);
		}
		
		public void Send(String message)
		{
			sendthread = new Thread(new ParameterizedThreadStart(SendThread));
			sendthread.Start(message);
		}
		
		private void SendThread(Object StateInfo)
		{
			sending = true;
			try {
				byte[] buffer = Encoding.ASCII.GetBytes((String)StateInfo);
				client.GetStream().Write(buffer, 0, buffer.Length);
				byte[] bufer = new byte[4096];
				int count;
				count = client.GetStream().Read(bufer, 0, bufer.Length);
				callback(Encoding.ASCII.GetString(bufer, 0, count));
			} catch (Exception e) {
				if (!(e is IOException)) {
					MessageBox.Show(e.Message + "\n" + e.Source);
				}
			}
			sending = false;
		}
		
		
	}
}
