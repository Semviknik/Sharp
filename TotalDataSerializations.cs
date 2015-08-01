/*
 * Создано в SharpDevelop.
 * Пользователь: V.Seminchenko
 * Дата: 27.07.2015
 * Время: 15:52
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace Total.Data.Serializations
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	/// 
	
	
	[Serializable]
	public class Person
	{
		public String Name {get;set;}
		public String LastName {get;set;}
		public Adress HomeAdress {get;set;}
	}
	
	[Serializable]
	public class Adress
	{
		public int Index {get;set;}
		public String Country {get;set;}
		public String State {get;set;}
		public String Provance {get;set;}
		public String City {get;set;}
		public String Street {get;set;}
		public String Home {get;set;}
		
		public Adress()
		{
			
		}
		
		public Adress(int index, String country, String state, String provance, String city, String street, String home)
		{
			Index = index;
			Country = country;
			State = state;
			Provance = provance;
			City = city;
			Street = street;
			Home = home;
		}
	}
	
	public static class xmlSerializePreparator
	{
		public static Object GetSerializeObject(XmlDocument xml)
		{
			XmlElement xroot = xml.DocumentElement;
			String st = xroot.Name;
			Assembly asmb = Assembly.GetExecutingAssembly();
			Type[] types = asmb.GetTypes();
			Type t = null;
			foreach (var elm in types) {
				if (elm.Name == st) {
					t = elm;
				}
			}
			if (t != null) {
				Object obj = Activator.CreateInstance(t);
				return obj;
			}else {return null;}

		}
		
		public static Byte[] SerializeObjectToBytes(Object obj)
		{
			XmlSerializer formatter = new XmlSerializer(typeof(Person));
			using (MemoryStream fs = new MemoryStream())
            {
				formatter.Serialize(fs, obj);
				return fs.ToArray();
				
            }
			
		}
		
		public static String SerializeObjectToString(Object obj)
		{
			return Encoding.ASCII.GetString(SerializeObjectToBytes(obj));
		}
		
		public static void SerializeObjectToFile(Object obj, String fileName)
		{
			using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
			{
				fs.SetLength(0);
				byte[] buffer = SerializeObjectToBytes(obj);
				fs.Write(buffer, 0, buffer.Length);
			}
		}
		
		public static Object DeserializeObjectFromBytes(Byte[] xml)
		{
			using (MemoryStream fs = new MemoryStream(xml))
            {
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(fs);
				Object obj = xmlSerializePreparator.GetSerializeObject(xmlDoc);
				XmlSerializer formatter = new XmlSerializer(obj.GetType());
				fs.Seek(0, SeekOrigin.Begin);
				return (Object)formatter.Deserialize(fs);
            }
		}
		
		public static Object DeserializeObjectFromString(String xml)
		{
			return DeserializeObjectFromBytes(Encoding.ASCII.GetBytes(xml));
		}
		
		public static Object DeserializeObjectFromFile(String fileName)
		{
			using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
			{
				byte[] buffer = new byte[fs.Length];
				fs.Read(buffer, 0, (int)fs.Length);
				return DeserializeObjectFromBytes(buffer);
			}
		}
	}
	
}
