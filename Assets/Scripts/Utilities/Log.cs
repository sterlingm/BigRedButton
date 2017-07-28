using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDB
{
	public enum LogLevel: int 
	{
		Error = 0,
		Warning = 1,
		High = 2,
		Med = 3,
		Low = 4
	}

	public class Log : SingletonBehaviour<Log, MonoBehaviour> 
	{
		public LogLevel logLevel = LogLevel.Low;

		protected override void OnInstanceInit()  
		{
			DontDestroyOnLoad( this );
		}

		public static LogLevel LogLevel
		{
			get
			{
				if (Log.instance != null) {
					return Log.instance.logLevel;
				} else {
					Debug.LogWarning ("Log instance does not exist ? WTF");
					return LogLevel.Low;
				}
			}
		}

		public static void SetLogLevel(LogLevel newLogLevel)
		{
			Log.instance.logLevel = newLogLevel;
		}


	    public static void Error(string error)
	    {
			Log.LogWithLevel (error, LogLevel.Error);
	    }

		public static void Warning(string warning)
	    {
			Log.LogWithLevel (warning, LogLevel.Warning);
	    }

		public static void High(string log)
		{
			Log.LogWithLevel (log, LogLevel.High);
		}

		public static void Med(string log)
		{
			Log.LogWithLevel (log, LogLevel.Med);
		}

		public static void Low(string log)
		{
			Log.LogWithLevel (log, LogLevel.Low);
		}

		public static void None(string log)
		{
			// dont log anything
		}

		public static void LogWithLevel(string log, LogLevel logLevel) 
		{
			if (Log.LogLevel >= logLevel) {
				if (logLevel == LogLevel.Error) {
					Debug.LogError (log.WithTimestamp ());
				} else if (logLevel == LogLevel.Warning) {
					Debug.LogWarning (log.WithTimestamp ());
				} else {
					Debug.Log (log.WithTimestamp ());
				}
			}
		}
	}
}