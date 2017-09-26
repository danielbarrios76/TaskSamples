using System;
using System.Threading;

namespace TaskMessagesLibrary
{
    public class TaskMessages
    {
        public event Action<object, TaskMessageEventArgs> MessageEventHandler;

        public void AddMessage(string message, int seconds)
        {
            Thread.Sleep(1000 * seconds);
            var thread = Thread.CurrentThread.ManagedThreadId;

            MessageEventHandler?.Invoke(this, 
                new TaskMessageEventArgs($"{message} Hilo actual evento: {thread}\n"));
        }

    }

    public class TaskMessageEventArgs : EventArgs
    {
        public string Message;

        public TaskMessageEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
