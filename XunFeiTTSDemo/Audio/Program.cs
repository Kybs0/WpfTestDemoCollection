/*
 * 由SharpDevelop创建。
 * 用户： hello! 黄元元
 * 日期: 2018/8/29
 * 时间: 12:42
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using TTS;//TTS是自己封装的dll，里面封装了错误编号，以及需要的结构体，在debug路径下
using System.IO;
using System.Runtime.InteropServices;
using System.Media;
using System.Threading;

namespace Audio
{
	class Program
	{
		public static void Main(string[] args){
			//main函数中的代码可根据自己的需求进行修改
			for(int i=0;i<10;i++){
				button1_Click(Console.ReadLine());//返回值作为函数的参数，用来输入识别的语句
			}
			Console.ReadKey(true);
		}
		
		
		private static void button1_Click(string str)
		{
			int ret = 0;
		    IntPtr session_ID;
			//APPID请勿随意改动
			string login_configs = "appid = 5ac9f8d1";//登录参数,自己注册后获取的appid
			string text=str;//text是待合成文本
			string filename = "Call.wav"; //合成的语音文件名
			uint audio_len = 0;
			SynthStatus synth_status = SynthStatus.MSP_TTS_FLAG_STILL_HAVE_DATA;
			ret = TTSDll.MSPLogin(string.Empty, string.Empty, login_configs);//第一个参数为用户名，第二个参数为密码，第三个参数是登录参数，用户名和密码需要在http://open.voicecloud.cn
			//MSPLogin方法返回失败
			if (ret != (int)ErrorCode.MSP_SUCCESS)
			{
				return;
			}
			//string parameter = "engine_type = local, voice_name=xiaoyan, tts_res_path =fo|res\\tts\\xiaoyan.jet;fo|res\\tts\\common.jet, sample_rate = 16000";
			string _params = "ssm=1,ent=sms16k,vcn=xiaoyan,spd=medium,aue=speex-wb;7,vol=x-loud,auf=audio/L16;rate=16000";
			//string @params = "engine_type = local,voice_name=xiaoyan,speed=50,volume=50,pitch=50,rcn=1, text_encoding = UTF8, background_sound=1,sample_rate = 16000";
			session_ID = TTSDll.QTTSSessionBegin(_params, ref ret);
			//QTTSSessionBegin方法返回失败
			if (ret != (int)ErrorCode.MSP_SUCCESS)
			{
				return;
			}
			ret = TTSDll.QTTSTextPut(Ptr2Str(session_ID), text, (uint)Encoding.Default.GetByteCount(text), string.Empty);
			//QTTSTextPut方法返回失败
			if (ret != (int)ErrorCode.MSP_SUCCESS)
			{
				return;
			}
			//内存流可直接在内存进行读写，不需要临时缓冲区或者临时文件
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(new byte[44], 0, 44);//为结构体开辟空间，后面用来存储音频文件结构体
			while (true)
			{
				IntPtr source = TTSDll.QTTSAudioGet(Ptr2Str(session_ID), ref audio_len, ref synth_status, ref ret);
				byte[] array = new byte[(int)audio_len];
				if (audio_len > 0)
				{
					Marshal.Copy(source, array, 0, (int)audio_len);
				}
				memoryStream.Write(array, 0, array.Length);//将合成的音频字节数据存放到内存流中
				Thread.Sleep(150);//防止CPU频繁占用
				if (synth_status == SynthStatus.MSP_TTS_FLAG_DATA_END || ret != 0)
					break;
			}
			WAVE_Header wave_Header = getWave_Header((int)memoryStream.Length - 44);
			byte[] array2 = StructToBytes(wave_Header);
			memoryStream.Position = 0L;//将指针定位到开头
			memoryStream.Write(array2, 0, array2.Length);//存储结构体的字节数组
			memoryStream.Position = 0L;//将指针定位到开头
			SoundPlayer soundPlayer = new SoundPlayer(memoryStream);//通过读取内存流中的数据创建播放器
			//soundPlayer.Stop();
			soundPlayer.Play();//播放音频
			if (filename != null)
			{
				//通过文件名创建音频文件流
				FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
				memoryStream.WriteTo(fileStream);//将内存流中的数据写入到文件流，文件流写入到音频文件中I
				memoryStream.Close();//关闭流
				fileStream.Close();
			}
			ret = TTSDll.QTTSSessionEnd(Ptr2Str(session_ID), "");//结束会话
			ret = TTSDll.MSPLogout();//退出登录
		}

		/// <summary>
		/// 结构体转字符串
		/// </summary>
		/// <param name="structure"></param>
		/// <returns></returns>
		private static byte[] StructToBytes(object structure)
		{
			int num = Marshal.SizeOf(structure);
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			byte[] result;
			try
			{
				Marshal.StructureToPtr(structure, intPtr, false);
				byte[] array = new byte[num];
				Marshal.Copy(intPtr, array, 0, num);
				result = array;
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return result;
		}
		
		/// <summary>
		/// 结构体初始化赋值
		/// </summary>
		/// <param name="data_len"></param>
		/// <returns></returns>
		private static WAVE_Header getWave_Header(int data_len)
		{
			return new WAVE_Header
			{
				RIFF_ID = 1179011410,//相当于SDK例子中的"RIFF"，只是转换成了对应的ASCL值
				File_Size = data_len + 36,
				RIFF_Type = 1163280727,
				FMT_ID = 544501094,
				FMT_Size = 16,
				FMT_Tag = 1,
				FMT_Channel = 1,
				FMT_SamplesPerSec = 16000,
				AvgBytesPerSec = 32000,
				BlockAlign = 2,
				BitsPerSample = 16,
				DATA_ID = 1635017060,
				DATA_Size = data_len
			};
		}
		/// <summary>
		/// 语音音频头
		/// </summary>
		private  struct WAVE_Header
		{
			public int RIFF_ID;
			public int File_Size;
			public int RIFF_Type;
			public int FMT_ID;
			public int FMT_Size;
			public short FMT_Tag;
			public ushort FMT_Channel;
			public int FMT_SamplesPerSec;
			public int AvgBytesPerSec;
			public ushort BlockAlign;
			public ushort BitsPerSample;
			public int DATA_ID;
			public int DATA_Size;
		}
		/// 指针转字符串
		/// </summary>
		/// <param name="p">指向非托管代码字符串的指针</param>
		/// <returns>返回指针指向的字符串</returns>
		public static string Ptr2Str(IntPtr p)
		{
			List<byte> lb = new List<byte>();
			while (Marshal.ReadByte(p) != 0)
			{
				lb.Add(Marshal.ReadByte(p));
				p = p + 1;
			}
			byte[] bs = lb.ToArray();
			return Encoding.Default.GetString(lb.ToArray());
		}
	}
}