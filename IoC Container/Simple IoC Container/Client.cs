using Simple_IoC_Container.Communicator;
using Simple_IoC_Container.Communicator.Interfaces;
using Simple_IoC_Container.IoCContainer;
using Simple_IoC_Container.Services;
using Simple_IoC_Container.Services.Interfaces;
using System;

namespace Simple_IoC_Container
{
    public class Client
    {
        /// <summary>
        /// Simple C# IoC Container
        /// Don't use this as your IoC Container! It's only created to get you familiar about the concepts.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            SendMessage();

            Console.ReadKey();
        }

        static void SendMessage()
        {
            var container = SimpleIoC.CreateInstance();

            container.UseSingleton();

            container.Register<IMessageService, MessageService>();
            container.Register<ICommunicator, EmailCommunicator>();
            container.Register<ILogger, Logger>();

            // Resolve the dependencies
            var resolvedService = container.Resolve<IMessageService>();
            // Add another service for testing the singleton usability
            var resolvedService2 = container.Resolve<IMessageService>();

            resolvedService.SendMessage("This is a simple IoC Container!");
            resolvedService2.SendMessage("This message is sent by message service");

            // Because of the singleton is configured for the container, it will return number (2).
            //var count = resolvedService.GetSentCount();

        }

    }
}
