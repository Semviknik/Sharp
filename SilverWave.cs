/*
 * Created by SharpDevelop.
 * User: V.Seminchenko
 * Date: 19.01.2015
 * Time: 11:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.IO;
using System.Security.AccessControl;
using SharpUtils;

namespace SilverWave
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class AudioData
	{
		public Single[] data;
		public AudioData()
		{
			
		}
	}
	
	public class SilverWave
	{
		public Single lastMaxLevel;
		public UInt32 size;
		public AudioData rightC;
		public AudioData leftC;
		public Single deltaTime;
		public Single aTime;
		public UInt16 nChannels;
		public UInt32 nSamplesPerSec;
		public UInt16 nBitsPerSample;
   		
		public SilverWave()
		{
			rightC = new AudioData();
			leftC = new AudioData();	
		}
		~SilverWave()
		{
			rightC.data = null;
			leftC.data = null;
		}
		public SilverWave(UInt16 channels, UInt32 samplesPerSec)
		{
			nChannels = channels;
			nSamplesPerSec = samplesPerSec;
			nBitsPerSample = 16;
			deltaTime = 1 / samplesPerSec;
			SetSize(0);	
		}
		public void SaveWaveFile(String fPath)
		{
			FileStream fStream;
			MemoryStream ms;
			UInt32 k, d, rsize;
			UInt16 ar, al;
			short iar, ial;
			ms = new MemoryStream();
			ms.SetLength(((nBitsPerSample * nChannels * size) / 8) + 44);
			rsize = (UInt32)((nBitsPerSample * nChannels * size) / 8) + 36;
			d = MathExt.StrToDWORD("RIFF");
			ms.Write(BitConverter.GetBytes(d), 0, 4);
			ms.Write(BitConverter.GetBytes(rsize), 0, 4);
			d = MathExt.StrToDWORD("WAVE");
			ms.Write(BitConverter.GetBytes(d), 0, 4);
			d = MathExt.StrToDWORD("fmt ");
			ms.Write(BitConverter.GetBytes(d), 0, 4);
			d = 16;
			ms.Write(BitConverter.GetBytes(d), 0, 4);
			d = (UInt32)65536 * nChannels + 1;
			ms.Write(BitConverter.GetBytes(d), 0, 4);
			d = nSamplesPerSec;
			ms.Write(BitConverter.GetBytes(d), 0, 4);
			d = (nSamplesPerSec * nChannels * nBitsPerSample) / 8;
			ms.Write(BitConverter.GetBytes(d), 0, 4);
			d = (UInt32)(65536 * nBitsPerSample + ((nChannels * nBitsPerSample) / 8));
			ms.Write(BitConverter.GetBytes(d), 0, 4);
			d = MathExt.StrToDWORD("data");
			ms.Write(BitConverter.GetBytes(d), 0, 4);
			d = rsize - 36;
			ms.Write(BitConverter.GetBytes(d), 0, 4);
//Stereo file
			if (nChannels == 2) {
				for (int i = 0; i < rightC.data.Length; i++) {
					iar = (short)(rightC.data[i] * 3276);
					ial = (short)(leftC.data[i] * 3276);
					ar = (UInt16)iar;
					al = (UInt16)ial;
					d = (UInt32)((ar << 16) + al);
					ms.Write(BitConverter.GetBytes(d), 0, 4);
				}
			}
//Mono file
			if (nChannels == 1) {
				k = (UInt32)(rightC.data.Length) / 2;
				for (int i = 0; i < k; i++) {
					iar = (short)(rightC.data[2 * i + 1] * 3276);
					ial = (short)(rightC.data[2 * i] * 3276);
					ar = (UInt16)iar;
					al = (UInt16)ial;
					d = (UInt32)((ar << 16) + al);
					ms.Write(BitConverter.GetBytes(d), 0, 4);
				}
			}
			ms.Flush();
			fStream = new FileStream(fPath, FileMode.OpenOrCreate);
			fStream.SetLength(((nBitsPerSample * nChannels * size) / 8) + 44);
			ms.Seek(0, SeekOrigin.Begin);
			ms.CopyTo(fStream);
			fStream.Flush();

			fStream.Close();
			ms.Close();
			ms = null;
			fStream = null;
		}
    	
		public void SetSize(UInt32 length)
		{
			rightC = null;
			leftC = null;
			rightC = new AudioData();
			leftC = new AudioData();
			if (nChannels == 1) {
				rightC.data = new float[(int)length];
				leftC.data = new float[0];
			
			}
			if (nChannels == 2) {
				rightC.data = new float[(int)length];
				leftC.data = new float[(int)length];
			}
			size = length;	
		}
		public void SetAudioData(int position, float leftChannalData, float rightChanelData)
		{
			if (nChannels == 2) {
				if (position <= size) {
					leftC.data[position] = leftChannalData;
					rightC.data[position] = rightChanelData;
				}
			}

			if (nChannels == 1) {
				if (position <= size) {
					rightC.data[position] = (rightChanelData + leftChannalData) / 2;
				}
			}

		}
		
	}
}
