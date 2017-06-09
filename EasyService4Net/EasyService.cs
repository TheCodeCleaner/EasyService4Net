using System;
using System.Reflection;
using System.ServiceProcess;
using EasyService4Net.ServiceInternals;

namespace EasyService4Net
{
    public class EasyService
    {
        #region Events

        public event Action OnServiceStarted;
        public event Action OnServiceStopped;

        #endregion

        #region Fields

        private readonly string _serviceDisplayName;
        private readonly WindowsServiceManager _serviceManager;
        private readonly RegistryManipulator _registryManipulator;

        #endregion

        #region C'tor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceDisplayName">null for using the assembly name.</param>
        /// <param name="serviceName">null for using the assembly name.</param>
        public EasyService(string serviceDisplayName = null, string serviceName = null)
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
            _serviceDisplayName = serviceDisplayName ?? assemblyName;
            serviceName = serviceName ?? assemblyName;

            _serviceManager = new WindowsServiceManager(_serviceDisplayName);
            _registryManipulator = new RegistryManipulator(serviceName);

            InternalService.OsStarted += Start;
            InternalService.OsStopped += Stop;
            ProjectInstaller.InitInstaller(_serviceDisplayName,serviceName);
        }

        #endregion

        #region Public

        public void Run(string[] args)
        {
            if (args.Length == 0)
            {
                RunConsole();
                return;
            }

            switch (args[0])
            {
                case "-service":
                    RunService();
                    break;
                case "-install":
                    InstallService();
                    break;
                case "-uninstall":
                    UnInstallService();
                    break;
                default:
                    throw new Exception(args[0] + " is not a valid command!");
            }
        }

        #endregion

        #region Private

        private void RunConsole()
        {
            Start();
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            Stop();
        }

        private static void RunService()
        {
            ServiceBase[] servicesToRun = { new InternalService() };
            ServiceBase.Run(servicesToRun);
        }

        private void InstallService()
        {
            _serviceManager.Install();
            _registryManipulator.AddMinusServiceToRegistry();
            _serviceManager.Start();
        }

        private void UnInstallService()
        {
            if (!_serviceManager.IsInstalled())
                return;

            _serviceManager.Stop();
            _registryManipulator.RemoveMinusServiceFromRegistry();
            _serviceManager.UnInstall();
        }

        private void Stop()
        {
            if (OnServiceStopped != null)
                OnServiceStopped.Invoke();
        }

        private void Start()
        {
            Console.WriteLine("Service {0} started ", _serviceDisplayName);
            if (OnServiceStarted != null)
                OnServiceStarted.Invoke();
        }

        #endregion
    }
}
