using System;
using System.Threading;

namespace AccountsApi.Infrastructure
{
    public class Logger : ILogger
    {
        public void Debug(string message)
        {
           Console.WriteLine($"{GetDate()} {Thread.CurrentThread.ManagedThreadId} DEBUG: {message}");
        }

        public void Error(string message)
        {
            Error(message, null);
        }

        public void Error(string message, Exception ex)
        {
           Console.WriteLine($"{GetDate()} {Thread.CurrentThread.ManagedThreadId} ERROR: {message}");
           LogException(ex);
        }

        public void Info(string message)
        {
            Console.WriteLine($"{GetDate()} {Thread.CurrentThread.ManagedThreadId} INFO: {message}");
        }

        private void LogException(Exception ex){
            if(ex == null) return;
            Console.WriteLine($"{GetDate()} {Thread.CurrentThread.ManagedThreadId} Exception: {ex.GetType()}");
            Console.WriteLine($"{GetDate()} {Thread.CurrentThread.ManagedThreadId} {ex.Message}");
            Console.WriteLine($"{GetDate()} {Thread.CurrentThread.ManagedThreadId} {ex.StackTrace}");
        }

        private string GetDate(){
            return DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss.fff");
        }
    }
}