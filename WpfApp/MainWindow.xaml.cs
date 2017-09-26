using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TaskMessagesLibrary;

namespace WpfApp
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource CTS;
        CancellationToken CT;
        Task LongRunningTask;
        TaskMessages taskMessages = new TaskMessages();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void DoLongRunnigTask(CancellationToken CT)
        {
            int[] IDs = { 1,3,4,7,11,18,29,35,47,56,63,76,88,96,100};

            for (int i=0; i<IDs.Length && !CT.IsCancellationRequested ;i++)
            {
                
                taskMessages.AddMessage($"Procesando IDs: {IDs[i]}", 2);
            }

            if (CT.IsCancellationRequested)
            {
                taskMessages.AddMessage("Proceso Cancelado...", 0);
                CT.ThrowIfCancellationRequested();
            }
        }

        private void StarTask_Click(object sender, RoutedEventArgs e)
        {
            CTS = new CancellationTokenSource();
            CT = CTS.Token;

            taskMessages.MessageEventHandler += (s, ev) =>
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    lbMessage.Content += ev.Message;
                });

            };


            //Emulando tareas

            Task T3 = new Task(() => taskMessages.AddMessage("Ejecutando Tarea 3", 8));
            T3.Start();


            Task T4 = new Task(() => taskMessages.AddMessage("Ejecutando Tarea 4", 3));
            T4.Start();

            Task T5 = new Task(() => taskMessages.AddMessage("Ejecutando Tarea 5", 3));
            T5.Start();

            //Emulando tareas con resultado

            Task<int> TV = Task.Run<int>(() =>
            {
                int Result = 0;
                Thread.Sleep(1000);
                Result = new Random().Next();
                return Result;
            });

            taskMessages.AddMessage($"EL valor del Numero es: {TV.Result}", 2);


            taskMessages.AddMessage("Al final del hilo principal", 0);

            //Emulando tareas de larga duracion con control de cancelacion
            Task.Run(() => 
            {
                LongRunningTask = Task.Run(() =>
                {
                    DoLongRunnigTask(CT);
                },CT);

                try
                {
                    LongRunningTask.Wait();
                    
                }
                catch (AggregateException aggregateExeception)
                {
                    foreach (var inner in aggregateExeception.InnerExceptions)
                    {
                        if (inner is TaskCanceledException)
                        {
                            taskMessages.AddMessage("Tarea Cancelada", 0);
                        }
                        else
                        {
                            taskMessages.AddMessage(inner.Message, 0);
                        }

                    }
                }
            });

        }

        private void CancelTask_Click(object sender, RoutedEventArgs e)
        {
            CTS.Cancel();    
        }

        private void ShowStatus_Click(object sender, RoutedEventArgs e)
        {
            taskMessages.AddMessage($"Estado: {LongRunningTask.Status}", 0);
        }
    }
}
