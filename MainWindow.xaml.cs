using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2015._12._14_EXAMINATION_3
{
    class Fan
    {
        public bool Ticket { get; set; }
        public int NumberSector { get; set; }
        public int NumberPlace { get; set; }
        public int NumberFan { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.ComponentModel.BackgroundWorker backgroundWorker;

        Random rand = new Random();

        int capacityStadium = 0;
        int amountOfOccupiedSpace;
        int countInQueue = 0;
        int countAfterQueue = 0;
        int leavStadium;
        List<int> countOccupiedSpace = new List<int>();
        List<int> countFreeSpace = new List<int>();
        List<int> sections = new List<int>();

        List<Fan> fans = new List<Fan>();
        List<Fan> queueFanWhithTickets = new List<Fan>();

        object locker = new object();

        Thread createFan;

        Thread sequrity1;
        Thread sequrity2;
        Thread sequrity3;
        Thread sequrity4;

        Thread stuard1;
        Thread stuard2;
        Thread stuard3;
        Thread stuard4;
        Thread stuard5;
        Thread stuard6;

        Thread leaveSector1;
        Thread leaveSector2;
        Thread leaveSector3;
        Thread leaveSector4;
        Thread leaveSector5;
        Thread leaveSector6;



        public MainWindow()
        {
            InitializeComponent();
            CreateBackground();
            //backgroundWorker = (System.ComponentModel.BackgroundWorker)this.FindResource("backgroundWorker");
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            ShowData();


        }

        private void ShowData()
        {
            if (occupiedSpace1 == null) return;

            occupiedSpace1.Text = countOccupiedSpace[0].ToString();
            freeSpace1.Text = countFreeSpace[0].ToString();

            occupiedSpace2.Text = countOccupiedSpace[1].ToString();
            freeSpace2.Text = countFreeSpace[1].ToString();

            occupiedSpace3.Text = countOccupiedSpace[2].ToString();
            freeSpace3.Text = countFreeSpace[2].ToString();

            occupiedSpace4.Text = countOccupiedSpace[3].ToString();
            freeSpace4.Text = countFreeSpace[3].ToString();

            occupiedSpace5.Text = countOccupiedSpace[4].ToString();
            freeSpace5.Text = countFreeSpace[4].ToString();

            occupiedSpace6.Text = countOccupiedSpace[5].ToString();
            freeSpace6.Text = countFreeSpace[5].ToString();

            tbAmountOfOccupiedSpace.Text = amountOfOccupiedSpace.ToString();
            btAmountOfFreSpace.Text = (capacityStadium - amountOfOccupiedSpace).ToString();

            btAmountOfPeopleBeforeGuard.Text = "0";
            btAmountOfPeopleAfterGuard.Text = "0";
            btLeavStadium.Text = "0";

        }
        private void ClearSectors()
        {
            lbSector1.Items.Clear();
            lbSector2.Items.Clear();
            lbSector3.Items.Clear();
            lbSector4.Items.Clear();
            lbSector5.Items.Clear();
            lbSector6.Items.Clear();
        }

        private void LoadData()
        {
            fans = new List<Fan>();
            queueFanWhithTickets = new List<Fan>();

            countInQueue = 0;
            countAfterQueue = 0;
            leavStadium = 0;

            int.TryParse(tboxCapacityStadium.Text, out capacityStadium);
            amountOfOccupiedSpace = 0;

            sections = new List<int>();
            countFreeSpace = new List<int>();
            countOccupiedSpace = new List<int>();

            int ii = (int)(capacityStadium / 6);
            for (int i = 0; i < 6; i++)
            {
                sections.Add(ii);
                countFreeSpace.Add(ii);
                countOccupiedSpace.Add(0);
            }
            ii = (capacityStadium - ii * 6 > 0) ? (capacityStadium - ii * 6) : 0;
            sections[0] += ii;
            countFreeSpace[0] += ii;
        }
        int countGoFan;
        private void CreateFans()
        {
            countGoFan = rand.Next((int)(capacityStadium * 0.3), (int)(capacityStadium * 2.5));
            int place, sector;
            for (; countGoFan > 0; )
            {
                lock (locker)
                {
                    place = -1;
                    sector = -1;

                    for (int j = 0; j < sections.Count; j++)
                    {
                        if (sections[j] > 0)
                        {
                            place = sections[j]--;
                            sector = j;
                            break;
                        }
                    }
                    fans.Add(new Fan() { Ticket = place >= 0 ? true : false, NumberPlace = place, NumberSector = sector, NumberFan = countGoFan });

                    countInQueue = fans.Count;
                    backgroundWorker.ReportProgress(1, btAmountOfPeopleBeforeGuard);//ЗАСТАВЛЯЕТ ЗАПУСТИТЬСЯ СОБЫТИЮ ProgressChanged
                countGoFan--;//-----------------------!!!!!!!!!!!!!!!!!1--------------------

                }
            }
        }
        private void QueueFanWhithTickets()
        {
            for (; ; )
                lock (locker)
                {
                    if (fans.Count > 0)
                    {
                        if (((Fan)fans.Last<Fan>()).Ticket == true)
                            queueFanWhithTickets.Add(fans.Last<Fan>());
                        fans.Remove(fans.Last<Fan>());

                        //Thread.Sleep(5);

                        countAfterQueue = queueFanWhithTickets.Count;
                        backgroundWorker.ReportProgress(1, btAmountOfPeopleBeforeGuard);//ЗАСТАВЛЯЕТ ЗАПУСТИТЬСЯ СОБЫТИЮ ProgressChanged
                    }
                    else break;
                }
        }
        private void ControlQueue(ListBox lb, System.ComponentModel.DoWorkEventArgs e, int numSector)
        {
            string tr;// = Thread.CurrentThread.Name;
            for (; ; )
            {
                lock (locker)
                {
                    tr = Thread.CurrentThread.Name;
                    if (fans.Count > 0 || queueFanWhithTickets.Count > 0)
                    {
                        if (queueFanWhithTickets.Count > 0)
                        {
                            //List<Fan> f = queueFanWhithTickets.Select(a => a.NumberSector == numSector).ToList<Fan>();
                            var f = queueFanWhithTickets.Where(a => a.NumberSector == numSector).ToList<Fan>();
                            if (f.Count != 0 && f != null)
                            {
                                if (backgroundWorker.CancellationPending == true)//ЗАПРОСИЛО ЛИ ПРИЛОЖЕНИЕ ОТМЕНУ ФОНОВОЙ ОПЕРАЦИИ ИЗ ВНЕ
                                {
                                    e.Cancel = true;
                                    break;
                                }
                                else
                                {
                                    Thread.Sleep(20);

                                    countInQueue = fans.Count;
                                    countAfterQueue = queueFanWhithTickets.Count;

                                    backgroundWorker.ReportProgress(1, lb);//ЗАСТАВЛЯЕТ ЗАПУСТИТЬСЯ СОБЫТИЮ ProgressChanged
                                    queueFanWhithTickets.Remove(f.Last<Fan>());
                                }

                            }
                            //queueFanWhithTickets.Remove(queueFanWhithTickets.Last<Fan>());
                        }
                    }
                    else break;
                }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            ShowData();

            ClearSectors();
            if (backgroundWorker.IsBusy != true)
            {
                backgroundWorker.RunWorkerAsync();//ЭТОТ МЕТОД ВЫЗЫВАЕТ СОБЫТИЕ DoWork

            }
            btnStart.IsEnabled = false;

        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (backgroundWorker.WorkerSupportsCancellation == true)//ПРОВЕРЯЕМ ПОДДЕРЖКУ ОТМЕНЫ ВЫПОЛНЕНИЯ АСИНХРОННОЙ ОПЕРАЦИИ(ПРЕРЫВАНИЯ ПОТОКА)
            {
                backgroundWorker.CancelAsync();//СТАВИТ CancellationPending = true


                sequrity1.Abort();
                sequrity2.Abort();
                sequrity3.Abort();
                sequrity4.Abort();
                stuard1.Abort();
                stuard2.Abort();
                stuard3.Abort();
                stuard4.Abort();
                stuard5.Abort();
                stuard6.Abort();

            }
            CreateBackground();
            LoadData();
        }
        private void CreateBackground()
        {
            backgroundWorker = new System.ComponentModel.BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
            };
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

        }


        private void BackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {
                #region Prixod
                if (e.UserState is ListBox)
                {
                    ListBox lb = (ListBox)e.UserState;
                    lb.Items.Add("Fan №" + lb.Items.Count);
                    switch (int.Parse(lb.Tag.ToString()))
                    {
                        case 1:
                            {
                                amountOfOccupiedSpace++;
                                countOccupiedSpace[0]++;
                                countFreeSpace[0]--;
                                occupiedSpace1.Text = countOccupiedSpace[0].ToString();
                                freeSpace1.Text = countFreeSpace[0].ToString();
                            }
                            break;
                        case 2:
                            {
                                amountOfOccupiedSpace++;
                                countOccupiedSpace[1]++;
                                countFreeSpace[1]--;
                                occupiedSpace2.Text = countOccupiedSpace[1].ToString();
                                freeSpace2.Text = countFreeSpace[1].ToString();
                            }
                            break;
                        case 3:
                            {
                                amountOfOccupiedSpace++;
                                countOccupiedSpace[2]++;
                                countFreeSpace[2]--;
                                occupiedSpace3.Text = countOccupiedSpace[2].ToString();
                                freeSpace3.Text = countFreeSpace[2].ToString();
                            }
                            break;
                        case 4:
                            {
                                amountOfOccupiedSpace++;
                                countOccupiedSpace[3]++;
                                countFreeSpace[3]--;
                                occupiedSpace4.Text = countOccupiedSpace[3].ToString();
                                freeSpace4.Text = countFreeSpace[3].ToString();
                            }
                            break;
                        case 5:
                            {
                                amountOfOccupiedSpace++;
                                countOccupiedSpace[4]++;
                                countFreeSpace[4]--;
                                occupiedSpace5.Text = countOccupiedSpace[4].ToString();
                                freeSpace5.Text = countFreeSpace[4].ToString();
                            }
                            break;
                        case 6:
                            {
                                amountOfOccupiedSpace++;
                                countOccupiedSpace[5]++;
                                countFreeSpace[5]--;
                                occupiedSpace6.Text = countOccupiedSpace[5].ToString();
                                freeSpace6.Text = countFreeSpace[5].ToString();
                            }
                            break;
                    }
                    tbAmountOfOccupiedSpace.Text = amountOfOccupiedSpace.ToString();
                    btAmountOfFreSpace.Text = (capacityStadium - amountOfOccupiedSpace).ToString();


                }


                try
                {
                    countInQueue = fans.Count;
                    countAfterQueue = queueFanWhithTickets.Count;

                    btAmountOfPeopleBeforeGuard.Text = countInQueue.ToString();
                    btAmountOfPeopleAfterGuard.Text = countAfterQueue.ToString();

                }
                catch (Exception)
                {
                    CreateBackground();
                }
                #endregion
            }
            else if (e.ProgressPercentage == 10)
            {
                #region Uxod
                ListBox lb = (ListBox)e.UserState;

                try
                {
                    if (lb.Items.Count == 0) return;

                    lb.Items.Remove(lb.Items.Count - 1);

                    switch (int.Parse(lb.Tag.ToString()))
                    {
                        case 1:
                            {
                                if (countOccupiedSpace[0] == 0) break;

                                leavStadium++;
                                amountOfOccupiedSpace--;
                                countOccupiedSpace[0]--;
                                countFreeSpace[0]++;
                                occupiedSpace1.Text = countOccupiedSpace[0].ToString();
                                freeSpace1.Text = countFreeSpace[0].ToString();
                            }
                            break;
                        case 2:
                            {
                                if (countOccupiedSpace[1] == 0) break;
                                leavStadium++;
                                amountOfOccupiedSpace--;
                                countOccupiedSpace[1]--;
                                countFreeSpace[1]++;
                                occupiedSpace2.Text = countOccupiedSpace[1].ToString();
                                freeSpace2.Text = countFreeSpace[1].ToString();
                            }
                            break;
                        case 3:
                            {
                                if (countOccupiedSpace[2] == 0) break;
                                amountOfOccupiedSpace--;
                                leavStadium++;
                                countOccupiedSpace[2]--;
                                countFreeSpace[2]++;
                                occupiedSpace3.Text = countOccupiedSpace[2].ToString();
                                freeSpace3.Text = countFreeSpace[2].ToString();
                            }
                            break;
                        case 4:
                            {
                                if (countOccupiedSpace[3] == 0) break;
                                leavStadium++;
                                amountOfOccupiedSpace--;
                                countOccupiedSpace[3]--;
                                countFreeSpace[3]++;
                                occupiedSpace4.Text = countOccupiedSpace[3].ToString();
                                freeSpace4.Text = countFreeSpace[3].ToString();
                            }
                            break;
                        case 5:
                            {
                                if (countOccupiedSpace[4] == 0) break;
                                leavStadium++;
                                amountOfOccupiedSpace--;
                                countOccupiedSpace[4]--;
                                countFreeSpace[4]++;
                                occupiedSpace5.Text = countOccupiedSpace[4].ToString();
                                freeSpace5.Text = countFreeSpace[4].ToString();
                            }
                            break;
                        case 6:
                            {
                                if (countOccupiedSpace[5] == 0) break;
                                leavStadium++;
                                amountOfOccupiedSpace--;
                                countOccupiedSpace[5]--;
                                countFreeSpace[5]++;
                                occupiedSpace6.Text = countOccupiedSpace[5].ToString();
                                freeSpace6.Text = countFreeSpace[5].ToString();
                            }
                            break;
                    }
                    tbAmountOfOccupiedSpace.Text = amountOfOccupiedSpace.ToString();
                    btAmountOfFreSpace.Text = (capacityStadium - amountOfOccupiedSpace).ToString();
                    btLeavStadium.Text = leavStadium.ToString();

                }
                catch (Exception)
                {

                    CreateBackground();
                }
                #endregion
            }


        }

        private void BackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            createFan = new Thread(CreateFans);
            createFan.Start();
             
            sequrity1 = new Thread(QueueFanWhithTickets) { IsBackground = true };
            sequrity2 = new Thread(QueueFanWhithTickets) { IsBackground = true };
            sequrity3 = new Thread(QueueFanWhithTickets) { IsBackground = true };
            sequrity4 = new Thread(QueueFanWhithTickets) { IsBackground = true }; 

            sequrity1.Start();
            sequrity2.Start();
            sequrity3.Start();
            sequrity4.Start();

            stuard1 = new Thread(delegate() { ControlQueue(lbSector1, e, 0); }) { Name = "stuard1", IsBackground = true };
            stuard2 = new Thread(delegate() { ControlQueue(lbSector2, e, 1); }) { Name = "stuard2", IsBackground = true };
            stuard3 = new Thread(delegate() { ControlQueue(lbSector3, e, 2); }) { Name = "stuard3", IsBackground = true };
            stuard4 = new Thread(delegate() { ControlQueue(lbSector4, e, 3); }) { Name = "stuard4", IsBackground = true };
            stuard5 = new Thread(delegate() { ControlQueue(lbSector5, e, 4); }) { Name = "stuard5", IsBackground = true };
            stuard6 = new Thread(delegate() { ControlQueue(lbSector6, e, 5); }) { Name = "stuard6", IsBackground = true };

            //Thread.Sleep(1000);

            stuard1.Start();
            stuard2.Start();
            stuard3.Start();
            stuard4.Start();
            stuard5.Start();
            stuard6.Start();



            sequrity1.Join();
            sequrity2.Join();
            sequrity3.Join();
            sequrity4.Join();

            Thread.Sleep(10000);
            leaveSector1 = new Thread(delegate() { LeaveSector(lbSector1, e, 0); }) { IsBackground = true };
            leaveSector2 = new Thread(delegate() { LeaveSector(lbSector2, e, 0); }) { IsBackground = true };
            leaveSector3 = new Thread(delegate() { LeaveSector(lbSector3, e, 0); }) { IsBackground = true };
            leaveSector4 = new Thread(delegate() { LeaveSector(lbSector4, e, 0); }) { IsBackground = true };
            leaveSector5 = new Thread(delegate() { LeaveSector(lbSector5, e, 0); }) { IsBackground = true };
            leaveSector6 = new Thread(delegate() { LeaveSector(lbSector6, e, 0); }) { IsBackground = true };

            leaveSector1.Start();
            leaveSector2.Start();
            leaveSector3.Start();
            leaveSector4.Start();
            leaveSector5.Start();
            leaveSector6.Start();

            leaveSector1.Join();
            leaveSector2.Join();
            leaveSector3.Join();
            leaveSector4.Join();
            leaveSector5.Join();
            leaveSector6.Join();


            stuard1.Join();
            stuard2.Join();
            stuard3.Join();
            stuard4.Join();
            stuard5.Join();
            stuard6.Join();


        }

        private void LeaveSector(ListBox lb, System.ComponentModel.DoWorkEventArgs e, int numSector)
        {
            for (; amountOfOccupiedSpace>0; )
            {
                lock (locker)
                {
                   Thread.Sleep(rand.Next(50, 250));
                   backgroundWorker.ReportProgress(10, lb);//ЗАСТАВЛЯЕТ ЗАПУСТИТЬСЯ СОБЫТИЮ ProgressChanged

                }
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                //MessageBox.Show("Извените посадка закончена");
                btnStart.IsEnabled = true;

            }
            else if (e.Error != null)
            {
                MessageBox.Show("Error: " + e.Error.Message);
                btnStart.IsEnabled = true;

            }
            else
            {
                btnStart.IsEnabled = true;
                //MessageBox.Show("Все заняли свои места");
            }


        }

        private void tboxCapacityStadium_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
            ShowData();

        }

    }
}
