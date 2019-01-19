using System;

namespace ItSoft.ClientService
{
    public interface IService
    {
        event EventHandler Connected;
        event EventHandler Disconnected; 


        void Start();
        void Stop();
    }
}