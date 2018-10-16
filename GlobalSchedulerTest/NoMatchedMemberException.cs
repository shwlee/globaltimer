using System;
using System.Runtime.Serialization;
using System.Security;

namespace GlobalSchedulerTest
{
	[Serializable]
	public class NoMatchedMemberException : Exception
	{
		public NoMatchedMemberException()
		{
		}

		public NoMatchedMemberException(string message)
			: base(message)
		{
		}

		public NoMatchedMemberException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public NoMatchedMemberException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
