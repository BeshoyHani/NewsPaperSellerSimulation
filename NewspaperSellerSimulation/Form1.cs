using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewspaperSellerModels;
using NewspaperSellerTesting;
using System.IO;

namespace NewspaperSellerSimulation
{
    public partial class Form1 : Form
    {
        readonly SimulationSystem SimulationSystem = new SimulationSystem();
        readonly SimulationCore SimulationCore;
        public Form1()
        {
            InitializeComponent();
            /*
             
             CHOOSE Test Case
             
             */

            GetDataFromFile("TestCase1");
            // Start Simulation
            SimulationCore = new SimulationCore(SimulationSystem);
            SimulationCore.RunServer();
            GUI();
            // End Simulation

            /*
             
             Change Test Case
             
             */
            string testingResult = TestingManager.Test(SimulationSystem, Constants.FileNames.TestCase1);
            MessageBox.Show(testingResult);
        }
        //Read Data input from file
        public void GetDataFromFile(string TestCase)
        {
            
            String Path = "TestCases\\";
            // Choosing test case
            Path += TestCase+".txt";
            string[] lines = File.ReadAllLines(Path);
            SimulationSystem.NumOfNewspapers = int.Parse(lines[1]);
            SimulationSystem.NumOfRecords = int.Parse(lines[4]);
            SimulationSystem.PurchasePrice = decimal.Parse(lines[7]);
            SimulationSystem.ScrapPrice = decimal.Parse(lines[10]);
            SimulationSystem.SellingPrice = decimal.Parse(lines[13]);
            SimulationSystem.DayTypeDistributions= AddDayTypeDis(lines,16);

            for (int i = 19; i < lines.Length; i++)
            {
                DemandDistribution demandDistribution = new DemandDistribution();
                string[] line = lines[i].Split(',', (char)StringSplitOptions.RemoveEmptyEntries);
                demandDistribution.Demand = int.Parse(line[0]);
                demandDistribution.DayTypeDistributions = AddDayTypeDis(lines, i);
                SimulationSystem.DemandDistributions.Add(demandDistribution);
                
            }
        }

        public List<DayTypeDistribution> AddDayTypeDis(string[] line, int line_number) {
            
            List<DayTypeDistribution> dayTypeDistributionslist = new List<DayTypeDistribution>();         
            string[] DayTypeProb = line[line_number].Split(',', (char)StringSplitOptions.RemoveEmptyEntries);
            List<string> lines = new List<string>(DayTypeProb);
            if (lines.Count == 4)
            {
                lines.RemoveAt(0);
            }
            for (int i = 0; i < 3; i++)
            {

                DayTypeDistribution dayTypeDistribution = new DayTypeDistribution
                {
                    DayType = (i == 0) ? Enums.DayType.Good : (i == 1) ? Enums.DayType.Fair : Enums.DayType.Poor,
                    Probability = decimal.Parse(lines[i])
                };
                dayTypeDistributionslist.Add(dayTypeDistribution);
            }

            return dayTypeDistributionslist;
        }

        public void GUI()
        {

            List<SimulationCase> ST = SimulationSystem.SimulationTable;
            for (int i = 0; i < ST.Count; i++)
            {
                DGV_SimTable.Rows.Add(
                  ST[i].DayNo, ST[i].RandomNewsDayType, ST[i].NewsDayType, ST[i].RandomDemand,
                  ST[i].Demand, ST[i].SalesProfit, ST[i].LostProfit, ST[i].ScrapProfit, ST[i].DailyNetProfit
                    );
            }
            DGV_SimTable.Rows.Add();
            DGV_SimTable.Rows[ST.Count].Cells[5].Value = "$"  + SimulationSystem.PerformanceMeasures.TotalSalesProfit;
            DGV_SimTable.Rows[ST.Count].Cells[6].Value = "$" + SimulationSystem.PerformanceMeasures.TotalLostProfit;
            DGV_SimTable.Rows[ST.Count].Cells[7].Value = "$" + SimulationSystem.PerformanceMeasures.TotalScrapProfit;
            DGV_SimTable.Rows[ST.Count].Cells[8].Value = "$" + SimulationSystem.PerformanceMeasures.TotalNetProfit;



        }
    }
    
    }
